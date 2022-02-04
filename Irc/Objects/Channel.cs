using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irc.Commands;
using Irc.Enumerations;
using Irc.Interfaces;
using Irc.Objects;
using User = Irc.Objects.User;

namespace Irc.Worker.Ircx.Objects
{
    public class Channel: ChatItem, IChannel
    {
        public IList<User> InviteList = new List<User>();
        public IList<IChannelMember> Members = new List<IChannelMember>();

        public Channel(IObjectStore objectStore, IPropStore propStore): base(objectStore, propStore)
        {
        }
        public void SetName(string Name)
        {
            this.Name = Name;
        }

        public string GetName()
        {
            return this.Name;
        }

        public IChannelMember GetMember(User User)
        {
            foreach (var channelMember in Members)
            {
                if (channelMember.GetUser() == User)
                    return channelMember;
            }

            return null;
        }

        public bool IsOnChannel(User user)
        {
            foreach (var member in Members)
            {
                //if (CompareUserAddress(user, member.GetUser())) return true;
                if (CompareUserNickname(member.GetUser(), user)) return true;
            }

            return false;
        }

        private static bool CompareUserAddress(User user, User otherUser)
        {
            if (otherUser == user || otherUser.Address.GetUserHost() == user.Address.GetUserHost()) return true;
            return false;
        }

        private static bool CompareUserNickname(User user, User otherUser)
        {
            return otherUser.Address.Nickname.ToUpper() == user.Address.Nickname.ToUpper();
        }

        public bool Allows(User user)
        {
            if (IsOnChannel(user)) return false;
            return true;
        }

        public IChannel Join(User user)
        {
            var member = new Member(user, new ChatMemberModes());
            Members.Add(member);
            Send(Raw.RPL_JOIN_IRC(user, this));
            return this;
        }

        public IChannel SendTopic(User user)
        {
            user.Send(Raw.IRCX_RPL_TOPIC_332(user.Server, user, this, "Topic goes here"));
            return this;
        }

        public IChannel SendNames(User user)
        {
            Names.ProcessNamesReply(user, this);
            return this;
        }

        public IChannel Part(User user)
        {
            var member = Members.Where(m => m.GetUser() == user).FirstOrDefault();
            Send(Raw.RPL_PART_IRC(user, this));
            Members.Remove(member);
            return this;
        }

        public void SendMessage(User user, string message)
        {
            Send(Raw.RPL_PRIVMSG_CHAN(user, this, message), user, true);
        }

        public void Send(string message) => Send(message, null, false);
        public void Send(string message, User u, bool ExcludeSender)
        {
            foreach (var channelMember in Members)
            {
                if (channelMember.GetUser() != u || !ExcludeSender) channelMember.GetUser().Send(message);
            }
        }

        public void Send(string message, User u)
        {
            throw new NotImplementedException();
        }

        public IList<IChannelMember> GetMembers() => Members;
    }
}

using Irc.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Irc.Commands
{
    internal class Ison : Command, ICommand
    {
        public Ison() : base(1) { }
        public new EnumCommandDataType GetDataType() => EnumCommandDataType.Data;

        public new void Execute(ChatFrame chatFrame)
        {
            var server = chatFrame.Server;
            var user = chatFrame.User;
            var parameters = chatFrame.Message.Parameters;

            var nicknames = parameters.Distinct(StringComparer.InvariantCultureIgnoreCase).ToList();
            var foundNicknames = new List<string>();

            foreach (var nickname in nicknames)
            {
                var found = chatFrame.Server.GetUsers().FirstOrDefault(serverUser => serverUser.Name.ToUpperInvariant() == nickname.ToUpperInvariant()) != null;
                if (found) foundNicknames.Add(nickname);
            }

            user.Send(Constants.IrcRaws.IRC_RAW_303(server, user, string.Join(' ', foundNicknames)));
        }
    }
}

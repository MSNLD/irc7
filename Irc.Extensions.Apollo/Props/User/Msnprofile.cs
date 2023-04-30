using Irc.Enumerations;
using Irc.Extensions.Apollo.Objects.User;
using Irc.Extensions.Props;
using Irc.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Irc.Extensions.Apollo.Props.User
{
    internal class Msnprofile : PropRule
    {
        private readonly ApolloProfile profile;

        public Msnprofile(ApolloProfile profile) : base(ExtendedResources.UserPropMsnProfile, EnumChannelAccessLevel.ChatMember, EnumChannelAccessLevel.ChatMember, "0", true)
        {
            this.profile = profile;
        }

        public override string GetValue()
        {
            // TODO: No permissions reply
            return string.Empty;
        }

        public override EnumIrcError SetValue(string value, ChatObject chatObject)
        {
            // TODO: Need to reply bad value if not valid, or reject if already done more than once
            int.TryParse(value, out var profileCode);
            profile.SetProfileCode(profileCode);
            return EnumIrcError.OK;
        }
    }
}

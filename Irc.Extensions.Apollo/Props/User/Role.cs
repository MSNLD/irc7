using Irc.Constants;
using Irc.Enumerations;
using Irc.Extensions.Apollo.Objects.Server;
using Irc.Extensions.Apollo.Objects.User;
using Irc.Extensions.Props;
using Irc.Interfaces;
using Irc.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Irc.Extensions.Apollo.Props.User
{
    public class Role : PropRule
    {
        private readonly ApolloServer _apolloServer;

        public Role(ApolloServer apolloServer) : base(Resources.UserPropRole, EnumChannelAccessLevel.None, EnumChannelAccessLevel.ChatMember, Resources.GenericProps, "0", true)
        {
            this._apolloServer = apolloServer;
        }

        public override EnumIrcError EvaluateSet(IChatObject source, IChatObject target, string propValue)
        {
            _apolloServer.ProcessCookie((IUser)source, Resources.UserPropRole, propValue);
            return EnumIrcError.NONE;
        }
    }
}

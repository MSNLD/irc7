using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Ircx.Objects;
using CSharpTools;
using System.Reflection;

namespace Core.Ircx.Commands
{
    class PASS : Command
    {

        public PASS(CommandCode Code) : base(Code)
        {
            base.MinParamCount = 1;
            base.PreRegistration = true;
            base.DataType = CommandDataType.None;
            base.ForceFloodCheck = true;
        }

        public new COM_RESULT Execute(Frame Frame)
        {
            if (Frame.User.Auth == null) {
                Frame.User.Auth = new Authentication.ANON();
                Frame.User.Auth.UserCredentials = new Authentication.SSPCredentials();
                Frame.User.Auth.UserCredentials.Password = Frame.Message.Data[0];
            }
            return COM_RESULT.COM_SUCCESS;
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irc.Enumerations;
using Irc.Worker.Ircx;

namespace Irc.Commands
{
    public class Command : ICommand
    {
        protected int _requiredMinimumParameters;
        protected int _requiredMaximumParameters;
        public string GetName() => this.GetType().Name;
        public EnumCommandDataType GetDataType()
        {
            throw new NotImplementedException();
        }

        public void Execute(ChatFrame chatFrame)
        {
            throw new NotImplementedException();
        }

        public bool CheckParameters(ChatFrame chatFrame)
        {
            if (_requiredMinimumParameters < 0) return true;
            
            if (chatFrame.Message.Parameters.Count >= _requiredMinimumParameters) return true;

            return false;
        }
    }
}

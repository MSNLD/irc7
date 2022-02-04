using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irc.Worker;

namespace Irc.Objects
{
    internal class ChatMemberModes: IChatMemberModes
    {
        public string GetModeString()
        {
            throw new NotImplementedException();
        }

        public uint GetUserMode()
        {
            throw new NotImplementedException();
        }

        public byte GetModeChar()
        {
            throw new NotImplementedException();
        }

        public bool IsAdmin()
        {
            throw new NotImplementedException();
        }

        public bool IsOwner()
        {
            throw new NotImplementedException();
        }

        public bool IsHost()
        {
            throw new NotImplementedException();
        }

        public bool IsVoice()
        {
            throw new NotImplementedException();
        }

        public bool IsNormal()
        {
            throw new NotImplementedException();
        }

        public void SetAdmin(bool flag)
        {
            throw new NotImplementedException();
        }

        public void SetOwner(bool flag)
        {
            throw new NotImplementedException();
        }

        public void SetHost(bool flag)
        {
            throw new NotImplementedException();
        }

        public void SetVoice(bool flag)
        {
            throw new NotImplementedException();
        }

        public void UpdateFlag()
        {
            throw new NotImplementedException();
        }

        public void SetNormal()
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irc.Interfaces;
using Irc.Objects;

namespace Irc.Extensions.Objects
{
    public class ExtendedMemberModes: MemberModes, IMemberModes
    {
        public ExtendedMemberModes()
        {
            modes.Add(ExtendedResources.MemberModeOwner, 0);
        }

        public new char GetListedMode()
        {
            if (IsOwner()) return ExtendedResources.MemberModeFlagOwner;
            
            return base.GetListedMode();
        }

        public new char GetModeChar()
        {
            if (IsOwner()) return ExtendedResources.MemberModeOwner;

            return base.GetModeChar();
        }

        public bool IsOwner()
        {
            return GetModeChar(ExtendedResources.MemberModeOwner) > 0;
        }

        public bool IsNormal()
        {
            return !IsOwner() && base.IsNormal();
        }

        public void SetOwner(bool flag)
        {
            modes[ExtendedResources.MemberModeOwner] = flag ? 1 : 0;
        }

        public new void SetNormal()
        {
            SetOwner(false);
            base.SetNormal();
        }
    }
}

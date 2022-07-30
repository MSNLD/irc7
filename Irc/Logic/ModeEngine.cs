using Irc.Enumerations;
using Irc.Interfaces;
using Irc.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Irc.Logic
{
    public class ModeRule
    {
        private readonly char modeChar;
        private readonly EnumUserAccessLevel requiredLevel;

        public ModeRule(char modeChar, EnumUserAccessLevel accessLevel)
        {
            this.modeChar = modeChar;
            this.requiredLevel = accessLevel;
        }

        public bool Evaluate(EnumUserAccessLevel accessLevel)
        {
            return (accessLevel >= requiredLevel);
        }
    }

    public class ModeFrame
    {
        public ModeFrame()
        {

        }
        public ModeFrame(char mode, int modeValue, ModeResult modeResult)
        {
            Mode = mode;
            ModeValue = modeValue;
            ModeResult = modeResult;
        }

        public char Mode { get; set; }
        public int ModeValue { get; set; }
        public ModeResult ModeResult { get; set; }
    }

    public enum ModeResult
    {
        OK,
        NOTFOUND,
        NOACCESS
    }


    public class ModeEngine
    {
        private readonly IModeCollection modeCollection;
        Dictionary<char, ModeRule> modeRules = new Dictionary<char, ModeRule>();

        public ModeEngine(IModeCollection modeCollection)
        {
            this.modeCollection = modeCollection;
        }

        public void AddModeRule(char modeChar, ModeRule modeRule)
        {
            modeRules[modeChar] = modeRule;
        }

        public void Evaluate(ChatObject source, string modeString)
        {

        }

        public List<ModeFrame> Breakdown(ChatObject source, string modeString)
        {
            List<ModeFrame> modeFrames = new List<ModeFrame>();

            bool modeFlag = true;

            foreach (var c in modeString)
            {
                switch (c)
                {
                    case '+':
                    case '-':
                        {
                            modeFlag = (c == '+' ? true : false);
                            break;
                        }
                    default:
                        {
                            bool exists = modeCollection.HasMode(c);
                            int modeValue = exists ? modeCollection.GetModeChar(c) : -1;
                            modeRules.TryGetValue(c, out var modeRule);

                            var modeResult = exists ? ModeResult.NOACCESS : ModeResult.NOTFOUND;
                            if (modeRule != null)
                            {
                                if (modeRule.Evaluate(source.Level)) modeResult = ModeResult.OK;
                            }

                            var frame = new ModeFrame(c, modeValue, modeResult);
                            modeFrames.Add(frame);
                            break;
                        }
                }
            }

            return modeFrames;
        }
    }
}

﻿using Irc.Constants;
using Irc.Enumerations;
using Irc.Interfaces;
using Irc.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Irc.Modes
{
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

        public static void Breakdown(IUser source, ChatObject target, string modeString, Queue<string> modeParameters)
        {
            bool modeFlag = true;

            foreach (var c in modeString)
            {
                switch (c)
                {
                    case '+':
                    case '-':
                        {
                            modeFlag = c == '+' ? true : false;
                            break;
                        }
                    default:
                        {
                            var modeCollection = target.GetModes();
                            bool exists = modeCollection.HasMode(c);
                            int modeValue = exists ? modeCollection.GetModeChar(c) : -1;

                            var modeRule = modeCollection.GetMode(c);
                            if (modeRule == null)
                            {
                                // Unknown mode char
                                // :sky-8a15b323126 472 Sky S :is unknown mode char to me
                                source.Send(Raw.IRCX_ERR_UNKNOWNMODE_472(source.Server, source, c));
                                continue;
                            }

                            string parameter = string.Empty;
                            if (modeRule.RequiresParameter)
                            {
                                if (modeParameters.Count > 0) { parameter = modeParameters.Dequeue(); }
                                else
                                {
                                    // Not enough parameters
                                    //:sky-8a15b323126 461 Sky MODE +q :Not enough parameters
                                    //source.Send(Raw.IRCX_ERR_NEEDMOREPARAMS_461(source.Server, source, ))
                                    continue;
                                }
                            }

                            var result = modeRule.Evaluate((ChatObject)source, target, modeFlag, parameter);

                            switch (result)
                            {
                                case EnumModeResult.BADVALUE:
                                    {
                                        // -> sky-8a15b323126 MODE #test +l hello
                                        // < - :sky - 8a15b323126 461 Sky MODE +l :Not enough parameters
                                        break;
                                    }
                                case EnumModeResult.NOTOPER:
                                    {
                                        //:sky-8a15b323126 482 Sky3k #test :You're not channel operator
                                        source.Send(Raw.IRCX_ERR_CHANOPRIVSNEEDED_482(source.Server, source, target));
                                        break;
                                    }
                            }

                            break;
                        }
                }
            }
        }

        public static bool IrcOpCheck(ChatObject source, ChatObject target)
        {
            if (source is IUser && target is IUser)
            {
                IUser user1 = (IUser)source;
                IUser user2 = (IUser)target;

                if (!user1.IsAdministrator() && user2.IsAdministrator())
                {
                    // You are not Administrator
                    user1.Send(IrcRaws.IRC_RAW_908(user1.Server, user1));
                    return false;
                }
                else if (!user1.IsIrcOperator() && user2.IsIrcOperator())
                {
                    // You are not IRC Operator
                    user1.Send(IrcRaws.IRC_RAW_481(user1.Server, user1));
                    return false;
                }
                else return true;
            }
            else return false;
        }
    }
}
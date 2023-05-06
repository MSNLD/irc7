using Irc.Constants;
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
                            var modeCollection = target.Modes;
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

                            string parameter = null;
                            if (modeRule.RequiresParameter)
                            {
                                if (modeParameters != null && modeParameters.Count > 0) { parameter = modeParameters.Dequeue(); }
                                else
                                {
                                    // Not enough parameters
                                    //:sky-8a15b323126 461 Sky MODE +q :Not enough parameters
                                    source.Send(Raw.IRCX_ERR_NEEDMOREPARAMS_461(source.Server, source, $"{Resources.CommandMode} {c}"));
                                    continue;
                                }
                            }

                            var result = modeRule.Evaluate((ChatObject)source, target, modeFlag, parameter);

                            switch (result)
                            {
                                case EnumIrcError.ERR_NEEDMOREPARAMS:
                                    {
                                        // -> sky-8a15b323126 MODE #test +l hello
                                        // < - :sky - 8a15b323126 461 Sky MODE +l :Not enough parameters
                                        source.Send(Raw.IRCX_ERR_NEEDMOREPARAMS_461(source.Server, source, $"{Resources.CommandMode} {c}"));
                                        break;
                                    }
                                case EnumIrcError.ERR_NOCHANOP:
                                    {
                                        //:sky-8a15b323126 482 Sky3k #test :You're not channel operator
                                        source.Send(Raw.IRCX_ERR_CHANOPRIVSNEEDED_482(source.Server, source, target));
                                        break;
                                    }
                                case EnumIrcError.ERR_NOCHANOWNER:
                                    {
                                        //:sky-8a15b323126 482 Sky3k #test :You're not channel operator
                                        source.Send(Raw.IRCX_ERR_CHANQPRIVSNEEDED_485(source.Server, source, target));
                                        break;
                                    }
                                case EnumIrcError.ERR_NOIRCOP:
                                    {
                                        source.Send(Raw.IRCX_ERR_NOPRIVILEGES_481(source.Server, source));
                                        break;
                                    }
                                case EnumIrcError.ERR_NOTONCHANNEL:
                                    {
                                        source.Send(Raw.IRCX_ERR_NOTONCHANNEL_442(source.Server, source, target));
                                        break;
                                    }
                                    // TODO: The below should not happen
                                case EnumIrcError.ERR_NOSUCHNICK:
                                    {
                                        source.Send(Raw.IRCX_ERR_NOSUCHNICK_401(source.Server, source, target.Name));
                                        break;
                                    }
                                case EnumIrcError.ERR_NOSUCHCHANNEL:
                                    {
                                        source.Send(Raw.IRCX_ERR_NOSUCHCHANNEL_403(source.Server, source, target.Name));
                                        break;
                                    }
                                case EnumIrcError.ERR_CANNOTSETFOROTHER:
                                    {
                                        source.Send(Raw.IRCX_ERR_USERSDONTMATCH_502(source.Server, source));
                                        break;
                                    }
                                case EnumIrcError.ERR_UNKNOWNMODEFLAG:
                                    {
                                        source.Send(IrcRaws.IRC_RAW_501(source.Server, source));
                                        break;
                                    }
                                case EnumIrcError.ERR_NOPERMS:
                                    {
                                        source.Send(Raw.IRCX_ERR_SECURITY_908(source.Server, source));
                                        break;
                                    }
                                case EnumIrcError.ERR_KEYSET:
                                    {
                                        source.Send(Raw.IRCX_ERR_KEYSET_467(source.Server, source, (IChannel)target));
                                        break;
                                    }
                            }

                            break;
                        }
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Ircx.Objects;
using CSharpTools;
using System.Reflection;

namespace Core.Ircx.Commands
{
    class KILL : Command
    {

        public KILL(CommandCode Code) : base(Code)
        {
            base.MinParamCount = 1;
            base.DataType = CommandDataType.None;
            base.RegistrationRequired = true;
            base.PreRegistration = false;
        }

        public new COM_RESULT Execute(Frame Frame)
        {
            if (Frame.Message.Data.Count >= 1)
            {
                List<Obj> objs = Frame.Server.GetObjects(Frame.Message.Data[0]);

                if (objs.Count > 0)
                {
                    // Supports KILL OID1,OID2,OID3 Reason
                    // Can mix user / chan

                    string Reason = Resources.Null;
                    if (Frame.Message.Data.Count > 1)
                    {
                        Reason = Frame.Message.Data[1];
                    }

                    for (int i = 0; i < objs.Count; i++)
                    {
                        //Determine type
                        //Kill with / without reason

                        if (objs[i].ObjectType == ObjType.ChannelObject)
                        {
                            Frame.Channel = (Channel)objs[i];
                            ProcessChannelKill(Frame, Reason);
                        }
                        else if (objs[i].ObjectType == ObjType.UserObject)
                        {
                            User TargetUser = (User)objs[i];
                            ProcessKill(Frame, TargetUser, Reason);
                        }
                    }
                }
                else
                {
                    // invalid objects
                }
            }
            return COM_RESULT.COM_SUCCESS;
        }

        public bool ProcessKill(Frame Frame, User TargetUser, string Reason)
        {
            if ((Frame.User.Level >= UserAccessLevel.ChatGuide) && (Frame.User.Level >= TargetUser.Level)) { 
                List<UserChannelInfo> channels = TargetUser.ChannelList;

                string KillRaw = Raws.Create(Server: Frame.Server, Client: Frame.User, Raw: Raws.RPL_KILL_IRC, Data: new string[] { TargetUser.Address.Nickname, Reason });

                while (channels.Count > 0)
                {
                    Channel channel = channels[0].Channel;
                    //TargetUser.ChannelMode.SetNormal();
                    // Need to reset usermodes
                    channel.RemoveMember(TargetUser);
                    TargetUser.RemoveChannel(channel);

                    //Broadcast to channel after user has been removed
                    channel.Send(KillRaw, Frame.User, true);

                }
                if (Frame.User != TargetUser) { Frame.User.Send(KillRaw); } // To avoid double notify
                TargetUser.Send(KillRaw);
                TargetUser.Terminate();
                return true;
            }
            else
            {
                Frame.User.Send(Raws.Create(Server: Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_SECURITY_908));
                return false;
            }
        }
        public void ProcessChannelKill(Frame Frame, string Reason) {
            if (Frame.User.Level >= UserAccessLevel.ChatGuide)
            {
                List<ChannelMember> Members = Frame.Channel.MemberList;

                if (Members != null)
                {
                    if (Members.Count > 0)
                    {
                        // Dispose of all users first
                        if (Frame.Message.Data.Count >= 2) { Reason = Frame.Message.Data[1]; }

                        for (int x = 0; x < Members.Count; x++)
                        {
                            // Channel kill doesnt kill people of same level if they are in the chan
                            if (Frame.User.Level > Members[x].User.Level)
                            {

                            // Security is checked in the kill
                                if (ProcessKill(Frame, Members[x].User, Reason)) { x--; }
                            }
                            //else
                            //{
                                // Some permissions error
                                //Frame.User.Send(Raws.Create(Server: Frame.Server, Frame.Channel, Frame.User, Raw: Raws.IRCX_ERR_SECURITY_908));
                            //}
                        }
                    }
                    // Only if empty then remove the channel
                    if ((Members.Count == 0) && (Frame.Channel.Modes.Registered.Value != 0x1))
                    {
                        // Remove Channel
                        string KillRaw = Raws.Create(Server: Frame.Server, Client: Frame.User, Raw: Raws.RPL_KILL_IRC, Data: new string[] { Frame.Channel.Name, Reason });
                        Frame.Server.RemoveObject(Frame.Channel);
                        Frame.User.Send(KillRaw);
                    }
                }
                else
                {
                    Frame.User.Send(Raws.Create(Server: Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_NOSUCHNICK_401, Data: new string[] { Resources.Null }));
                }

            }
            else
            {
                //not an operator
                Frame.User.Send(Raws.Create(Server: Frame.Server, Channel: Frame.Channel, Client: Frame.User, Raw: Raws.IRCX_ERR_NOPRIVILEGES_481));
            }
        }
    }
}

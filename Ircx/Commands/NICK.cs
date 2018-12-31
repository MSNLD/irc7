using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Ircx.Objects;
using CSharpTools;
using System.Reflection;

namespace Core.Ircx.Commands
{
    public class NICK : Command
    {
        public NICK(CommandCode Code) : base(Code)
        {
            base.MinParamCount = 1;
            base.DataType = CommandDataType.Standard;
            base.ForceFloodCheck = true;
        }

        public new COM_RESULT Execute(Frame Frame)
        {
            // Is Nickname supported in current auth?
            ValidateNicknameResult result = ValidateNickname(Frame.User, Frame.Message.Data[0]);
            if (result == ValidateNicknameResult.VALID)
            {


                // Below is after register
                if (Frame.User.Registered)
                {

                    // Only guests can change their nickname
                    if (!Frame.User.Guest)
                    {
                        // Nickname changes not permitted
                        Frame.User.Send(Raws.Create(Server: Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_NONICKCHANGES_439, Data: new String8[] { Frame.Message.Data[0] }));
                        return COM_RESULT.COM_SUCCESS;
                    }

                    if (Frame.User.Level >= UserAccessLevel.ChatGuide)
                    {
                        String8 tempNick = new String8(Frame.Message.Data[0].Length + 1);
                        tempNick.append((byte)'\'');
                        tempNick.append(Frame.Message.Data[0].bytes, 0, Frame.Message.Data[0].Length);
                        Frame.Message.Data[0] = tempNick;
                    }

                    bool bIsInUse = false;
                    for (int c = 0; c < Frame.User.ChannelList.Count; c++)
                    {
                        if (Frame.User.ChannelList[c].Channel.Members.GetMemberByName(Frame.Message.Data[0]) != null) { bIsInUse = true; break; }
                    }

                    if (!bIsInUse)
                    {
                        String8 NicknameChangeRaw = Raws.Create(Server: Frame.Server, Client: Frame.User, Raw: Raws.RPL_NICK, Data: new String8[] { Frame.Message.Data[0] });
                        Frame.User.Send(NicknameChangeRaw);
                        Frame.User.BroadcastToChannels(NicknameChangeRaw, true);
                    }
                    else
                    {
                        // Nickname is in use 
                        Frame.User.Send(Raws.Create(Server: Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_NICKINUSE_433, Data: new String8[] { Frame.Message.Data[0] }));
                        return COM_RESULT.COM_SUCCESS;
                    }
                }
                Frame.Server.UpdateUserNickname(Frame.User, Frame.Message.Data[0]);

            }
            else if (result == ValidateNicknameResult.IDENTICAL) ; // no output
            else
            {
                if (Frame.User.Authenticated)
                {
                    Frame.User.Send(Raws.Create(Server: Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_ERRONEOUSNICK_432, Data: new String8[] { Frame.Message.Data[0] }));
                }
            }
            return COM_RESULT.COM_SUCCESS;
        }
        public static void UpdateNickname(Server Server, User User, String8 Nickname)
        {
            String8 NicknameChangeRaw = Raws.Create(Server, Client: User, Raw: Raws.RPL_NICK, Data: new String8[] { Nickname });
            User.Send(NicknameChangeRaw);
            User.BroadcastToChannels(NicknameChangeRaw, true);
        }
        public enum ValidateNicknameResult { VALID, INVALID, IDENTICAL };

        public static ValidateNicknameResult ValidateNickname(User User, String8 Nickname)
        {
            // Nicknames cannot begin with numbers as that denotes an OID
            if (Nickname.bytes[0] >= 48 && Nickname.bytes[0] <= 57) { return ValidateNicknameResult.INVALID; }
            else if (Nickname.Length > Program.Config.MaxNickname) { return  ValidateNicknameResult.INVALID; }

            if ((User.Name == Nickname) && (User.Registered)) { return ValidateNicknameResult.IDENTICAL; }

            string NicknameMask;
            if (User.Level >= UserAccessLevel.ChatGuide)
            {
                NicknameMask = Core.Authentication.ANON.IRCOpNickMask;
            }
            else if (User.Auth != null)
            {
                NicknameMask = User.Auth.GetNickMask();
            }
            else
            {
                NicknameMask = Resources.RegExNickname;
            }

            return (String8RegEx.EvaluateAbs(NicknameMask, Nickname, true, 0, Nickname.length) == true ? ValidateNicknameResult.VALID : ValidateNicknameResult.INVALID);
        }
    }
}

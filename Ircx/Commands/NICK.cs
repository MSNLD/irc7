using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Ircx.Objects;
using CSharpTools;
using System.Reflection;
using System.Text;
using Core.CSharpTools;

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
                        Frame.User.Send(Raws.Create(Server: Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_NONICKCHANGES_439, Data: new string[] { Frame.Message.Data[0] }));
                        return COM_RESULT.COM_SUCCESS;
                    }

                    if (Frame.User.Level >= UserAccessLevel.ChatGuide)
                    {
                        StringBuilder tempNick = new StringBuilder(Frame.Message.Data[0].Length + 1);
                        tempNick.Append('\'');
                        tempNick.Append(Frame.Message.Data[0].ToString());
                        Frame.Message.Data[0] = tempNick.ToString();
                    }

                    bool bIsInUse = false;
                    for (int c = 0; c < Frame.User.ChannelList.Count; c++)
                    {
                        if (Frame.User.ChannelList[c].Channel.Members.GetMemberByName(Frame.Message.Data[0]) != null) { bIsInUse = true; break; }
                    }

                    if (!bIsInUse)
                    {
                        string NicknameChangeRaw = Raws.Create(Server: Frame.Server, Client: Frame.User, Raw: Raws.RPL_NICK, Data: new string[] { Frame.Message.Data[0] });
                        Frame.User.Send(NicknameChangeRaw);
                        Frame.User.BroadcastToChannels(NicknameChangeRaw, true);
                    }
                    else
                    {
                        // Nickname is in use 
                        Frame.User.Send(Raws.Create(Server: Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_NICKINUSE_433, Data: new string[] { Frame.Message.Data[0] }));
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
                    Frame.User.Send(Raws.Create(Server: Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_ERRONEOUSNICK_432, Data: new string[] { Frame.Message.Data[0] }));
                }
            }
            return COM_RESULT.COM_SUCCESS;
        }
        public static void UpdateNickname(Server Server, User User, string Nickname)
        {
            string NicknameChangeRaw = Raws.Create(Server, Client: User, Raw: Raws.RPL_NICK, Data: new string[] { Nickname });
            User.Send(NicknameChangeRaw);
            User.BroadcastToChannels(NicknameChangeRaw, true);
        }
        public enum ValidateNicknameResult { VALID, INVALID, IDENTICAL };

        public static ValidateNicknameResult ValidateNickname(User User, string Nickname)
        {
            // Nicknames cannot begin with numbers as that denotes an OID
            if (Nickname[0] >= 48 && Nickname[0] <= 57) { return ValidateNicknameResult.INVALID; }
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

            return (StringBuilderRegEx.EvaluateAbs(NicknameMask, Nickname.ToString(), true, 0, Nickname.Length) == true ? ValidateNicknameResult.VALID : ValidateNicknameResult.INVALID);
        }
    }
}

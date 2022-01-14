using System.Linq;
using System.Text;
using Irc.ClassExtensions.CSharpTools;
using Irc.Constants;
using Irc.Extensions.Access;
using Irc.Extensions.Security.Packages;
using Irc.Worker.Ircx.Objects;

namespace Irc.Worker.Ircx.Commands;

public class NICK : Command
{
    public enum ValidateNicknameResult
    {
        VALID,
        INVALID,
        IDENTICAL
    }

    public NICK(CommandCode Code) : base(Code)
    {
        MinParamCount = 1;
        DataType = CommandDataType.Standard;
        ForceFloodCheck = true;
    }

    public new bool Execute(Frame Frame)
    {
        // Is Nickname supported in current auth?
        var result = ValidateNickname(Frame.User, Frame.Message.Parameters[0]);
        if (result == ValidateNicknameResult.VALID)
        {
            // Below is after register
            if (Frame.User.Registered)
            {
                // Only guests can change their nickname
                if (!Frame.User.Guest)
                {
                    // Nickname changes not permitted
                    Frame.User.Send(RawBuilder.Create(Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_NONICKCHANGES_439,
                        Data: new[] {Frame.Message.Parameters[0]}));
                    return true;
                }

                if (Frame.User.Level >= UserAccessLevel.ChatGuide)
                {
                    var tempNick = new StringBuilder(Frame.Message.Parameters[0].Length + 1);
                    tempNick.Append('\'');
                    tempNick.Append(Frame.Message.Parameters[0]);
                    Frame.Message.Parameters[0] = tempNick.ToString();
                }

                var bIsInUse = false;
                foreach (var channel in Frame.User.Channels.Keys.ToList())
                {
                    if (channel.Members.FirstOrDefault(member => member.User.Name == Frame.Message.Parameters[0]) != null)
                    {
                        bIsInUse = true;
                        break;
                    }
                }

                if (!bIsInUse)
                {
                    var NicknameChangeRaw = RawBuilder.Create(Frame.Server, Client: Frame.User, Raw: Raws.RPL_NICK,
                        Data: new[] {Frame.Message.Parameters[0]});
                    Frame.User.Send(NicknameChangeRaw);
                    Frame.User.BroadcastToChannels(NicknameChangeRaw, true);
                }
                else
                {
                    // Nickname is in use 
                    Frame.User.Send(RawBuilder.Create(Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_NICKINUSE_433,
                        Data: new[] {Frame.Message.Parameters[0]}));
                    return true;
                }
            }

            Frame.User.UpdateUserNickname(Frame.Message.Parameters[0]);
        }
        else if (result == ValidateNicknameResult.IDENTICAL)
        {
            ; // no output
        }
        else
        {
            if (Frame.User.Authenticated)
                Frame.User.Send(RawBuilder.Create(Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_ERRONEOUSNICK_432,
                    Data: new[] {Frame.Message.Parameters[0]}));
        }

        return true;
    }

    public static void UpdateNickname(Server Server, User User, string Nickname)
    {
        var NicknameChangeRaw = RawBuilder.Create(Server, Client: User, Raw: Raws.RPL_NICK, Data: new[] {Nickname});
        User.Send(NicknameChangeRaw);
        User.BroadcastToChannels(NicknameChangeRaw, true);
    }

    public static ValidateNicknameResult ValidateNickname(User User, string Nickname)
    {
        // Nicknames cannot begin with numbers as that denotes an OID
        if (Nickname[0] >= 48 && Nickname[0] <= 57)
            return ValidateNicknameResult.INVALID;
        if (Nickname.Length > Program.Config.MaxNickname) return ValidateNicknameResult.INVALID;

        if (User.Name == Nickname && User.Registered) return ValidateNicknameResult.IDENTICAL;

        string NicknameMask;
        if (User.Level >= UserAccessLevel.ChatGuide)
            NicknameMask = Resources.IrcOpNickMask;
        else if (User.Guest)
            NicknameMask = Resources.GuestNicknameMask;
        else
            NicknameMask = Resources.NicknameMask;

        return StringBuilderRegEx.EvaluateAbs(NicknameMask, Nickname, true, 0, Nickname.Length)
            ? ValidateNicknameResult.VALID
            : ValidateNicknameResult.INVALID;
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Irc.ClassExtensions.CSharpTools;
using Irc.Constants;
using Irc.Worker.Ircx.Objects;

namespace Irc.Worker.Ircx.Commands;

internal class LISTX : Command
{
    public LISTX(CommandCode Code) : base(Code)
    {
        MinParamCount = 1;
        DataType = CommandDataType.None;
        RegistrationRequired = true;
        ForceFloodCheck = true;
    }

    public new COM_RESULT Execute(Frame Frame)
    {
        var server = Frame.Server;
        var message = Frame.Message;
        var user = Frame.User;

        var Channels = new List<Channel>();
        if (message.Data.Count > 1)
        {
            //
            if (Channel.IsChannel(message.Data[1]))
            {
                //process as list of channels
                var ChannelNames = Tools.CSVToArray(message.Data[1]);
                //LISTX [<query list>] <mask>
                //<query list> One or more query terms separated by spaces or commas.
                //<mask>         Sequence of characters that is used to select a matching channel name or topic. The character * and ? are used for wildcard searches.
                //      <#Select channels with less than # members.
                //      >#Select channels with more than # members.
                //      C<#Select channels created less than # minutes ago.
                //      C>#Select channels created greater than # minutes ago.
                //      L=<mask>Select channels with language property matching the mask string.
                //      N=<mask>Select channels with name matching the mask string.
                //      R=0Select unregistered channels.
                //      R=1Select registered channels.
                //      S=<mask>Select channels with subject matching the mask string.
                //      T<#Select channels with a topic changed less than # minutes ago.
                //      T>#Select channels with a topic changed greater than # minutes ago.
                //      T=<mask>Select channels that topic matches the mask string.
                //      <query limit>Maximum number of channels to be returned.
                //      <mask>Sequence of characters that is used to select a matching channel 
                //      name or topic. The character * and ? are used for wildcard searches. The 
                //      Directory Server recognize * and ? as plain text. All masks specified are 
                //      assumed to have wildcards at the beginning and end. The Chat Servers do 
                //      support wildcards in masks.
                for (var i = 0; i < ChannelNames.Count; i++)
                    if (!Channel.IsChannel(ChannelNames[i]))
                    {
                        //<- :SERVER 900 Administrator LISTX :Bad command
                        user.Send(RawBuilder.Create(server, Client: user, Raw: Raws.IRCX_ERR_BADCOMMAND_900,
                            Data: new[] {message.Data[0]}));
                        return COM_RESULT.COM_SUCCESS;
                    }
                    else
                    {
                        var objType = IrcHelper.IdentifyObject(ChannelNames[i]);
                        var c = server.Channels.FindObj(ChannelNames[i], objType);
                        if (c != null) Channels.Add(c);
                    }
            }
            else
            {
                var iRegisteredFlag = 0; // 0 = Neutral, -1 = False, 1 = True (not trying to be like VB!!!)
                var iMaxListEntries = int.MaxValue;
                int iMinMembers = 0, iMaxMembers = int.MaxValue;
                int iMinutesBefore = int.MaxValue, iMinutesAfter = 0;
                int iMinutesBeforeTopic = int.MaxValue, iMinutesAfterTopic = 0;
                var LanguageMask = Resources.Null;
                var NameMask = Resources.Null;
                var SubjectMask = Resources.Null;
                var TopicMask = Resources.Null;

                bool bCheckMembers = false,
                    bCheckCreation = false,
                    bCheckLanguage = false,
                    bCheckName = false,
                    bCheckRegistered = false,
                    bCheckSubject = false,
                    bCheckTopicChanged = false,
                    bCheckTopic = false;

                //process as list of commands
                var Params = Tools.CSVToArray(message.Data[1], true, Address.MaxFieldLen);

                for (var i = 0; i < Params.Count; i++)
                {
                    var bMatch = false;
                    if (Params[i].Length >= 2)
                    {
                        switch (Params[i][0])
                        {
                            case '<':
                            {
                                //      <#Select channels with less than # members.
                                iMinMembers = Tools.Str2Int(Params[i], 1);
                                bCheckMembers = true;
                                if (iMinMembers == -1)
                                {
                                    /* Bad Command */
                                    user.Send(RawBuilder.Create(server, Client: user, Raw: Raws.IRCX_ERR_BADCOMMAND_900,
                                        Data: new[] {message.Data[0]}));
                                    return COM_RESULT.COM_SUCCESS;
                                }

                                break;
                            }
                            case '>':
                            {
                                //      >#Select channels with more than # members.
                                iMaxMembers = Tools.Str2Int(Params[i], 1);
                                bCheckMembers = true;
                                if (iMaxMembers == -1)
                                {
                                    /* Bad Command */
                                    user.Send(RawBuilder.Create(server, Client: user, Raw: Raws.IRCX_ERR_BADCOMMAND_900,
                                        Data: new[] {message.Data[0]}));
                                    return COM_RESULT.COM_SUCCESS;
                                }

                                break;
                            }
                            case 'C':
                            {
                                //      C<#Select channels created less than # minutes ago.
                                //      C>#Select channels created greater than # minutes ago.
                                bCheckCreation = true;
                                var iNumber = Tools.Str2Int(Params[i], 2);
                                if (iNumber == -1)
                                {
                                    /* Bad Command */
                                    user.Send(RawBuilder.Create(server, Client: user, Raw: Raws.IRCX_ERR_BADCOMMAND_900,
                                        Data: new[] {message.Data[0]}));
                                    return COM_RESULT.COM_SUCCESS;
                                }

                                if (Params[i][1] == (byte) '<')
                                {
                                    iMinutesBefore = iNumber;
                                }
                                else if (Params[i][1] == (byte) '>')
                                {
                                    iMinutesAfter = iNumber;
                                }
                                else
                                {
                                    /* Bad Command */
                                    user.Send(RawBuilder.Create(server, Client: user, Raw: Raws.IRCX_ERR_BADCOMMAND_900,
                                        Data: new[] {message.Data[0]}));
                                    return COM_RESULT.COM_SUCCESS;
                                }

                                break;
                            }
                            case 'L':
                            {
                                //      L=<mask>Select channels with language property matching the mask string.
                                bCheckLanguage = true;
                                if (Params[i][1] == (byte) '=' && Params[i].Length > 2)
                                {
                                    LanguageMask = new string(Params[i].Substring(2).ToUpper());
                                }
                                else
                                {
                                    /* bad command */
                                    user.Send(RawBuilder.Create(server, Client: user, Raw: Raws.IRCX_ERR_BADCOMMAND_900,
                                        Data: new[] {message.Data[0]}));
                                    return COM_RESULT.COM_SUCCESS;
                                }

                                break;
                            }
                            case 'N':
                            {
                                //      N=<mask>Select channels with name matching the mask string.
                                bCheckName = true;
                                if (Params[i][1] == (byte) '=' && Params[i].Length > 2)
                                {
                                    NameMask = new string(Params[i].Substring(2).ToUpper());
                                }
                                else
                                {
                                    /* bad command */
                                    user.Send(RawBuilder.Create(server, Client: user, Raw: Raws.IRCX_ERR_BADCOMMAND_900,
                                        Data: new[] {message.Data[0]}));
                                    return COM_RESULT.COM_SUCCESS;
                                }

                                break;
                            }
                            case 'R':
                            {
                                //      R=1Select registered channels.
                                bCheckRegistered = true;
                                if (Params[i][1] == (byte) '=' && Params[i].Length == 3)
                                {
                                    if (Params[i][2] == (byte) '1')
                                    {
                                        iRegisteredFlag = 1;
                                    }
                                    else if (Params[i][2] == (byte) '0')
                                    {
                                        iRegisteredFlag = 0;
                                    }
                                    else
                                    {
                                        /* bad command */
                                        user.Send(RawBuilder.Create(server, Client: user, Raw: Raws.IRCX_ERR_BADCOMMAND_900,
                                            Data: new[] {message.Data[0]}));
                                        return COM_RESULT.COM_SUCCESS;
                                    }
                                }
                                else
                                {
                                    /* bad command */
                                    user.Send(RawBuilder.Create(server, Client: user, Raw: Raws.IRCX_ERR_BADCOMMAND_900,
                                        Data: new[] {message.Data[0]}));
                                    return COM_RESULT.COM_SUCCESS;
                                }

                                break;
                            }
                            case 'S':
                            {
                                //      S=<mask>Select channels with subject matching the mask string.
                                bCheckSubject = true;
                                if (Params[i][1] == (byte) '=' && Params[i].Length > 2)
                                {
                                    SubjectMask = new string(Params[i].Substring(2).ToUpper());
                                }
                                else
                                {
                                    /* bad command */
                                    user.Send(RawBuilder.Create(server, Client: user, Raw: Raws.IRCX_ERR_BADCOMMAND_900,
                                        Data: new[] {message.Data[0]}));
                                    return COM_RESULT.COM_SUCCESS;
                                }

                                break;
                            }
                            case 'T':
                            {
                                //      T<#Select channels with a topic changed less than # minutes ago.
                                //      T>#Select channels with a topic changed greater than # minutes ago.
                                //      T=<mask>Select channels that topic matches the mask string.
                                if (Params[i].Length > 2)
                                {
                                    if (Params[i][1] == (byte) '<')
                                    {
                                        bCheckTopicChanged = true;
                                        var iNumber = Tools.Str2Int(Params[i], 2);
                                        iMinutesBefore = iNumber;
                                    }
                                    else if (Params[i][1] == (byte) '>')
                                    {
                                        bCheckTopicChanged = true;
                                        var iNumber = Tools.Str2Int(Params[i], 2);
                                        iMinutesAfter = iNumber;
                                    }
                                    else if (Params[i][1] == (byte) '=')
                                    {
                                        bCheckTopic = true;
                                        TopicMask = new string(Params[i].Substring(2).ToUpper());
                                    }
                                    else
                                    {
                                        /* Bad Command */
                                        user.Send(RawBuilder.Create(server, Client: user, Raw: Raws.IRCX_ERR_BADCOMMAND_900,
                                            Data: new[] {message.Data[0]}));
                                        return COM_RESULT.COM_SUCCESS;
                                    }
                                }
                                else
                                {
                                    /* Bad Command */
                                    user.Send(RawBuilder.Create(server, Client: user, Raw: Raws.IRCX_ERR_BADCOMMAND_900,
                                        Data: new[] {message.Data[0]}));
                                    return COM_RESULT.COM_SUCCESS;
                                }

                                break;
                            }
                            default:
                            {
                                if (Params[i][0] >= 48 && Params[i][0] <= 57)
                                {
                                    //process as a number
                                    var maxListEntries = Tools.Str2Int(Params[i]);
                                    if (maxListEntries == -1)
                                    {
                                        //<- :SERVER 900 Administrator LISTX :Bad command
                                        user.Send(RawBuilder.Create(server, Client: user, Raw: Raws.IRCX_ERR_BADCOMMAND_900,
                                            Data: new[] {message.Data[0]}));
                                        return COM_RESULT.COM_SUCCESS;
                                    }

                                    if (maxListEntries > 1)
                                        iMaxListEntries = maxListEntries;
                                    else
                                        maxListEntries = int.MaxValue;
                                }
                                else
                                {
                                    //<- :SERVER 900 Administrator LISTX :Bad command
                                    user.Send(RawBuilder.Create(server, Client: user, Raw: Raws.IRCX_ERR_BADCOMMAND_900,
                                        Data: new[] {message.Data[0]}));
                                    return COM_RESULT.COM_SUCCESS;
                                }

                                break;
                            }
                        }
                    }
                    else
                    {
                        if (Params[i][0] >= 48 && Params[i][0] <= 57)
                        {
                            //process as a number
                            var maxListEntries = Tools.Str2Int(Params[i]);
                            if (maxListEntries == -1)
                            {
                                //<- :SERVER 900 Administrator LISTX :Bad command
                                user.Send(RawBuilder.Create(server, Client: user, Raw: Raws.IRCX_ERR_BADCOMMAND_900,
                                    Data: new[] {message.Data[0]}));
                                return COM_RESULT.COM_SUCCESS;
                            }

                            if (maxListEntries > 1)
                            {
                                iMaxListEntries = maxListEntries;
                            }
                            else
                            {
                                maxListEntries = int.MaxValue;
                            }
                        }
                        else
                        {
                            //<- :SERVER 900 Administrator LISTX :Bad command
                            user.Send(RawBuilder.Create(server, Client: user, Raw: Raws.IRCX_ERR_BADCOMMAND_900,
                                Data: new[] {message.Data[0]}));
                            return COM_RESULT.COM_SUCCESS;
                        }
                    }
                }

                /* Find channel logic here */


                for (var i = 0; i < server.Channels.Length; i++)
                {
                    bool bFoundMembers = true,
                        bFoundCreation = true,
                        bFoundName = true,
                        bFoundLanguage = true,
                        bFoundRegistered = true,
                        bFoundTopic = true,
                        bFoundTopicChanged = true,
                        bFoundSubject = true;
                    if (bCheckMembers)
                    {
                        if (server.Channels.IndexOf(i).MemberList.Count <= iMinMembers &&
                            server.Channels.IndexOf(i).MemberList.Count >= iMaxMembers)
                            bFoundMembers = true;
                        else
                            bFoundMembers = false;
                    }

                    if (bCheckCreation)
                    {
                        long iTime = (DateTime.UtcNow.Ticks - Resources.epoch) / TimeSpan.TicksPerSecond,
                            iAge = iTime - server.Channels.IndexOf(i).Properties.CreationDate;
                        iAge = iAge / 60;
                        if (iAge <= iMinutesBefore && iAge >= iMinutesAfter)
                            bFoundCreation = true;
                        else
                            bFoundCreation = false;
                    }

                    if (bCheckName)
                    {
                        if (StringBuilderRegEx.EvaluateString(NameMask, server.Channels.IndexOf(i).Name.ToUpper(), true))
                            bFoundName = true;
                        else
                            bFoundName = false;
                    }

                    if (bCheckLanguage)
                    {
                        if (StringBuilderRegEx.EvaluateString(LanguageMask,
                                server.Channels.IndexOf(i).Properties.Get("Language"), true))
                            bFoundLanguage = true;
                        else
                            bFoundLanguage = false;
                    }

                    if (bCheckRegistered)
                    {
                        if (iRegisteredFlag == server.Channels.IndexOf(i).Modes.Registered.Value)
                            bFoundRegistered = true;
                        else
                            bFoundRegistered = false;
                    }

                    if (bCheckSubject)
                    {
                        if (StringBuilderRegEx.EvaluateString(SubjectMask, server.Channels.IndexOf(i).Properties.Get("Subject"),
                                true))
                            bFoundSubject = true;
                        else
                            bFoundSubject = false;
                    }

                    if (bCheckTopic)
                    {
                        if (StringBuilderRegEx.EvaluateString(TopicMask, server.Channels.IndexOf(i).Properties.Get("Topic"),
                                true))
                            bFoundTopic = true;
                        else
                            bFoundTopic = false;
                    }

                    if (bCheckTopicChanged)
                    {
                        long iTime = (DateTime.UtcNow.Ticks - Resources.epoch) / TimeSpan.TicksPerSecond,
                            iAge = iTime - server.Channels.IndexOf(i).Properties.TopicLastChanged;
                        if (iTime < iMinutesBeforeTopic && iTime > iMinutesAfterTopic)
                            bFoundTopicChanged = true;
                        else
                            bFoundTopicChanged = false;
                    }

                    if (bFoundMembers && bFoundCreation && bFoundName && bFoundLanguage && bFoundRegistered &&
                        bFoundSubject && bFoundTopic && bFoundTopicChanged) Channels.Add(server.Channels.IndexOf(i));
                }
            }
        }
        else
        {
            Channels = server.Channels.ChatObjects.Cast<Channel>().ToList();
        }


        user.Send(RawBuilder.Create(server, Client: user, Raw: Raws.IRCX_RPL_LISTXSTART_811));

        for (var i = 0; i < Channels.Count; i++)
            user.Send(RawBuilder.Create(server, Client: user, Raw: Raws.IRCX_RPL_LISTXLIST_812,
                Data: new[]
                {
                    Channels[i].Name, Channels[i].Modes.ChannelModeShortString, Channels[i].Properties.Get("Topic")
                }, IData: new[] {Channels[i].MemberList.Count, Channels[i].Modes.UserLimit.Value}));
        user.Send(RawBuilder.Create(server, Client: user, Raw: Raws.IRCX_RPL_LISTXEND_817));
        return COM_RESULT.COM_SUCCESS;
    }
}
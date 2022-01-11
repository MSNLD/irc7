using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Ircx.Objects;
using CSharpTools;
using System.Reflection;
using System.Text;

namespace Core.Ircx.Commands
{
    class CREATE : Command
    {
        public CREATE(CommandCode Code) : base(Code)
        {
            //new syntax
            //create <category> <channel> <topic> [<mode> <limit>] <region> <language> <ownerkey> 0
            //CREATE CATEGORY   CHANNEL	  %TOPIC  MODES	  		    REGION  LANGUAGE	OWNERKEY  0

            //:server 613 [yournick] :newserver newport
            //:server 702 [yournick] :Channel not found
            //:server 708 [yournick] :Input flooding. Closing link.

            base.RegistrationRequired = true;
            base.MinParamCount = 8;
            base.DataType = CommandDataType.Join;
            base.ForceFloodCheck = true;
        }

        public new COM_RESULT Execute(Frame Frame)
        {
            Server server = Frame.Server;
            User user = Frame.User;
            Message message = Frame.Message;

            if (Frame.User.Level < UserAccessLevel.ChatUser)
            {
                //guests cannot create
                Frame.User.Send(Raws.Create(Server: Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_UNKNOWNCOMMAND_421, Data: new string[] { Frame.Message.Command }));
                return COM_RESULT.COM_SUCCESS;
            }

            if ((user.Profile.Ircvers > 0) && (user.Profile.Ircvers < 9))
            {
                if (user.Channels.ChannelList.Count > 0) { user.Send(Raws.Create(Server: server, Channel: user.Channels.ChannelList[0].Channel, Client: user, Raw: Raws.IRCX_ERR_TOOMANYCHANNELS_405)); return COM_RESULT.COM_SUCCESS; }
            }
         
            int currentIndex = 0;

            string CatCode = new string(message.Data[currentIndex++].ToString().ToUpper());
            Category Cat = ResolveCategoryByString(CatCode);
            if (Cat == Category.None)
            {
                //:server 701 [yournick] :Category not found
                user.Send(Raws.Create(Server: server, Client: user, Raw: Raws.IRCX_RPL_FINDS_NOSUCHCAT_701));
                return COM_RESULT.COM_SUCCESS;
            }

            string ChannelName = message.Data[currentIndex++];
            if (!Channel.IsValidChannelFormat(ChannelName))
            {
                //:server 706 [yournick] :Channel name is not valid
                user.Send(Raws.Create(Server: server, Client: user, Raw: Raws.IRCX_RPL_FINDS_INVALIDCHANNEL_706));
                return COM_RESULT.COM_SUCCESS;
            }

            string Topic = message.Data[currentIndex++];

            //Validate Mode
            //If - then no Limit
            //Else Mode + Limit
            string ModeString = message.Data[currentIndex++];
            string Limit = Resources.DefaultCreateUserLimit;
            if ((ModeString.Length == 1) && (ModeString[0] == 45))
            {
                ModeString = Resources.DefaultChannelModes;
            }
            else
            {
                Limit = message.Data[currentIndex++];

                //Validate Limit
                //Validate Modes
                if (!IsModeValid(user, ModeString, Limit))
                {
                    //:server 902 [yournick] :Badly formed parameters IRCX_ERR_BADLYFORMEDPARAMS_902
                    BadlyFormed(server, user);
                    return COM_RESULT.COM_SUCCESS;
                }
            }

            string Region = message.Data[currentIndex++].ToString().ToUpper();
            CountryLanguageZone Zone = CountryLanguageZone.ENUS;
            Zone = ResolveCountry(Region);
            if (Zone == CountryLanguageZone.None)
            {
                //:server 902 [yournick] :Badly formed parameters IRCX_ERR_BADLYFORMEDPARAMS_902
                BadlyFormed(server, user);
                return COM_RESULT.COM_SUCCESS;
            }
            //check for en-us etc

            //Validate Locale (A language const from 1 - 24)
            string Locale = message.Data[currentIndex++];
            int locale = CSharpTools.Tools.Str2Int(Locale);
            if ((locale < 1) || (locale > 24))
            {
                //:server 902 [yournick] :Badly formed parameters IRCX_ERR_BADLYFORMEDPARAMS_902
                BadlyFormed(server, user);
                return COM_RESULT.COM_SUCCESS;
            }

            //Validate Ownerkey
            string OwnerKey = Resources.Null;

            if (currentIndex < message.Data.Count)
            {
                OwnerKey = message.Data[currentIndex++];
            }

            if ((OwnerKey.Length <= 0) || (OwnerKey.Length > 31))
            {
                //:server 902 [yournick] :Badly formed parameters IRCX_ERR_BADLYFORMEDPARAMS_902
                BadlyFormed(server, user);
                return COM_RESULT.COM_SUCCESS;
            }

            if (currentIndex < message.Data.Count)
            {
                if (message.Data[currentIndex++] != Resources.Zero)
                {
                    //:server 902 [yournick] :Badly formed parameters IRCX_ERR_BADLYFORMEDPARAMS_902
                    BadlyFormed(server, user);
                    return COM_RESULT.COM_SUCCESS;
                }
            }
            else
            {
                //:server 902 [yournick] :Badly formed parameters IRCX_ERR_BADLYFORMEDPARAMS_902
                BadlyFormed(server, user);
                return COM_RESULT.COM_SUCCESS;
            }

            List<Channel> channels = Frame.Server.Channels.GetChannels(Frame.Server, Frame.User, ChannelName, false);
            if (channels.Count != 0) { 
                user.Send(Raws.Create(Server: server, Client: user, Raw: Raws.IRCX_RPL_FINDS_CHANNELEXISTS_705));
                return COM_RESULT.COM_SUCCESS;
            }
            else
            {
                //Create
                //Format event create reply
                //Send to MasterServer (via Load Balance function)
                //Send 613 to client
                Channel c = Frame.Server.AddChannel(ChannelName);
                c.Properties.Subject.Value = CreateSubject(TimeZone.ServerTime, Zone, Cat, false);
                c.Properties.Topic.Value = Topic;
                c.Properties.Language.Value = Locale;

                // Apply modes to channel
                int limit = CSharpTools.Tools.Str2Int(Limit);
                for (int i = 0; i < ModeString.Length; i++)
                {
                    Mode m = c.Modes.ResolveMode((byte)ModeString[i]);
                    if (m.ModeChar == (byte)'l')
                    {
                        m.Value = limit; // Mode itself is limit
                    }
                    else
                    {
                        m.Value = 0x1; // Mode is on
                    }
                }

                if (user.Modes.Secure.Value == 1)
                {
                    c.Modes.Subscriber.Value = 1;
                }

                c.Modes.UpdateModes(null);

                //c.HostKey = Resources.Null;

                c.Properties.Ownerkey.Value = OwnerKey;
                
                JOIN.ProcessJoinChannel(Frame, c, OwnerKey);
            }
            return COM_RESULT.COM_SUCCESS;
        }

        public void BadlyFormed(Server server, User user)
        {
            user.Send(Raws.Create(Server: server, Client: user, Raw: Raws.IRCX_ERR_BADLYFORMEDPARAMS_902));
        }

        public bool IsModeValid(User user, string ModeString, string Limit)
        {
            bool bModeAuthOnly = false, bModeHidden = false, bModeModerated = false, bModePrivate = false, bModeSecret = false,
            bModeTopicOp = false, bModeNoWhisper = false, bModeAuditorium = false, bModeNoGuestWhisper = false, bModeSpecialGuest = false,
            bModeNoExtern = false, bModeLimit = false;

            int limit = CSharpTools.Tools.Str2Int(Limit);
            if (limit <= 0) { return false; }

            for (int i = 0; i < ModeString.Length; i++)
            {
                switch (ModeString[i])
                {
                    case 'a':
                        {
                            if (bModeAuthOnly) { return false; }
                            else { bModeAuthOnly = true; }
                            break;
                        }
                    case 'm':
                        {
                            if (bModeModerated) { return false; }
                            else { bModeModerated = true; }
                            break;
                        }
                    case 'n':
                        {
                            if (bModeNoExtern) { return false; }
                            else { bModeNoExtern = true; }
                            break;
                        }
                    case 't':
                        {
                            if (bModeTopicOp) { return false; }
                            else { bModeTopicOp = true; }
                            break;
                        }
                    case 'l':
                        {
                            if (bModeLimit) { return false; }
                            else { bModeLimit = true; }
                            break;
                        }
                    case 'w':
                        {
                            if (bModeNoWhisper) { return false; }
                            else { bModeNoWhisper = true; }
                            break;
                        }
                    case 'W':
                        {
                            if (bModeNoGuestWhisper) { return false; }
                            else { bModeNoGuestWhisper = true; }
                            break;
                        }
                    case 'h':
                        {
                            if ((bModeHidden) || (bModeSecret) || (bModePrivate)) { return false; }
                            else { bModeHidden = true; }
                            break;
                        }
                    case 'p':
                        {
                            if ((bModeHidden) || (bModeSecret) || (bModePrivate)) { return false; }
                            else { bModePrivate = true; }
                            break;
                        }
                    case 's':
                        {
                            if ((bModeHidden) || (bModeSecret) || (bModePrivate)) { return false; }
                            else { bModeSecret = true; }
                            break;
                        }
                    case 'g':
                        {
                            if (bModeSpecialGuest) { return false; }
                            else { bModeSpecialGuest = true; }
                            break;
                        }
                    default: { return false; }
                }
            }

            if (((bModeAuthOnly) || (bModeAuditorium) || (bModeSpecialGuest)) && (user.Level < UserAccessLevel.ChatGuide)) { return false; }
            else { return true; } //limit has to exist as well
        }

        public static string ResolveCategory(Category Cat)
        {
            switch (Cat)
            {
                case Category.Teens: { return Resources.ChannelCategoryTeens;  }
                case Category.Computing: { return Resources.ChannelCategoryComputing;  }
                case Category.Events: { return Resources.ChannelCategoryEvents;  }
                case Category.General: { return Resources.ChannelCategoryGeneral;  }
                case Category.Health: { return Resources.ChannelCategoryHealth;  }
                case Category.CityChats: { return Resources.ChannelCategoryCityChats;  }
                case Category.Entertainment: { return Resources.ChannelCategoryEntertainment;  }
                case Category.Interests: { return Resources.ChannelCategoryInterests;  }
                case Category.Lifestyles: { return Resources.ChannelCategoryLifestyles;  }
                case Category.Music: { return Resources.ChannelCategoryMusic;  }
                case Category.Peers: { return Resources.ChannelCategoryPeers;  }
                case Category.News: { return Resources.ChannelCategoryNews;  }
                case Category.Religion: { return Resources.ChannelCategoryReligion;  }
                case Category.Romance: { return Resources.ChannelCategoryRomance;  }
                case Category.Sports: { return Resources.ChannelCategorySports;  }
                case Category.Unlisted: { return Resources.ChannelCategoryUnlisted;  }
            }
            return Resources.Null;
        }
        public static Category ResolveCategoryByString(string Cat)
        {
            if (Cat == Resources.ChannelCategoryTeens) { return Category.Teens; }
            else if (Cat == Resources.ChannelCategoryComputing) { return Category.Computing; }
            else if (Cat == Resources.ChannelCategoryEvents) { return Category.Events; }
            else if (Cat == Resources.ChannelCategoryGeneral) { return Category.General; }
            else if (Cat == Resources.ChannelCategoryHealth) { return Category.Health; }
            else if (Cat == Resources.ChannelCategoryCityChats) { return Category.CityChats; }
            else if (Cat == Resources.ChannelCategoryEntertainment) { return Category.Entertainment; }
            else if (Cat == Resources.ChannelCategoryInterests) { return Category.Interests; }
            else if (Cat == Resources.ChannelCategoryLifestyles) { return Category.Lifestyles; }
            else if (Cat == Resources.ChannelCategoryMusic) { return Category.Music; }
            else if (Cat == Resources.ChannelCategoryPeers) { return Category.Peers; }
            else if (Cat == Resources.ChannelCategoryNews) { return Category.News; }
            else if (Cat == Resources.ChannelCategoryReligion) { return Category.Religion; }
            else if (Cat == Resources.ChannelCategoryRomance) { return Category.Romance; }
            else if (Cat == Resources.ChannelCategorySports) { return Category.Sports; }
            else if (Cat == Resources.ChannelCategoryUnlisted) { return Category.Unlisted; }
            else { return Category.None; }
        }
        public static string ResolveTimeZone(TimeZone Zone)
        {
            switch (Zone)
            {
                case TimeZone.ServerTime: { return Resources.TimeRegionServerTime;  }
                case TimeZone.EST: { return Resources.TimeRegionEST;  }
                case TimeZone.GMT: { return Resources.TimeRegionGMT;  }
            }
            return Resources.Null;
        }
        public static string ResolveCountry(CountryLanguageZone Country)
        {
            switch (Country)
            {
                case CountryLanguageZone.ENUS: { return Resources.ChannelCountryLanguageENUS;  }
                case CountryLanguageZone.ENCA: { return Resources.ChannelCountryLanguageENCA;  }
                case CountryLanguageZone.ENGB: { return Resources.ChannelCountryLanguageENGB;  }
                case CountryLanguageZone.ENUK: { return Resources.ChannelCountryLanguageENUK;  }
                case CountryLanguageZone.FRCA: { return Resources.ChannelCountryLanguageFRCA;  }
            }
            return Resources.Null;
        }
        public static CountryLanguageZone ResolveCountry(string Country)
        {
            if (Country == Resources.ChannelCountryLanguageENUS) { return CountryLanguageZone.ENUS; }
            else if (Country == Resources.ChannelCountryLanguageENCA) { return CountryLanguageZone.ENCA; }
            else if (Country == Resources.ChannelCountryLanguageENGB) { return CountryLanguageZone.ENGB; }
            else if (Country == Resources.ChannelCountryLanguageENUK) { return CountryLanguageZone.ENUK; }
            else if (Country == Resources.ChannelCountryLanguageFRCA) { return CountryLanguageZone.FRCA; }
            else if (Country == Resources.ChannelCountryLanguageENAU) { return CountryLanguageZone.ENAU; }

            return CountryLanguageZone.None;
        }

        public static string CreateSubject(TimeZone Zone, CountryLanguageZone Country, Category Cat, bool DST)
        {
            //1:+ST!EN-US!AV
            //1:+ST!EN-GB!AV
            //1:-ST!EN-US!TN
            StringBuilder Value = new StringBuilder(14);
            Value.Append('1');
            Value.Append(':');
            Value.Append(DST ? '+' : '-');
            Value.Append(ResolveTimeZone(Zone));
            Value.Append('!');
            Value.Append(ResolveCountry(Country));
            Value.Append('!');
            Value.Append(ResolveCategory(Cat));
            return Value.ToString();
        }

        public enum TimeZone { ServerTime, GMT, EST };
        public enum CountryLanguageZone { ENUS, ENCA, ENGB, ENUK, FRCA, ENAU, None };
        public enum Category { Teens, Computing, Events, General, Health, CityChats, Entertainment, Interests, Lifestyles, Music, Peers, News, Religion, Romance, Sports, Unlisted, None };

    };
}

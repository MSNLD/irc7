using System;
using System.Text;
using Irc.ClassExtensions.CSharpTools;
using Irc.Worker.Ircx.Objects;

namespace Irc.Worker.Ircx;

public class RawBuilder
{
    public static string Create(Server Server = null, Channel Channel = null, Client Client = null, string Raw = null,
        string[] Data = null, int[] IData = null, bool Newline = true)
    {
        var RawText = new StringBuilder(Raw);
        int dataLen = 0, offsetd = 0, offseti = 0, remainder = 0;

        var output = new StringBuilder(512);
        remainder = 510 - Raw.Length;

        for (var i = 0; i < Raw.Length; i++)
            switch (RawText.ToByteArray()[i])
            {
                case (byte) '%':
                {
                    i++;
                    if (i < Raw.Length)
                        switch (RawText.ToByteArray()[i])
                        {
                            case (byte) 's':
                            {
                                var len = Data[offsetd].Length;
                                if (len > 0)
                                {
                                    if (len > remainder) len = remainder;
                                    remainder -= output.AppendByteArrayAsChars(Data[offsetd++].ToByteArray(), 0, len);
                                }

                                break;
                            }
                            case (byte) 'S':
                            {
                                //put all of string array in
                                while (offsetd < Data.Length)
                                {
                                    var s = Convert.ToString(Data[offsetd++]);
                                    output.Append(s);
                                    remainder -= s.Length;
                                }

                                break;
                            }
                            case (byte) 'd':
                            {
                                // TODO: Check this works
                                var s = IData[offseti++].ToString();
                                output.Append(s);
                                remainder -= s.Length;
                                break;
                            }
                            case (byte) 'x':
                            {
                                // TODO: Check this works
                                var s = IData[offseti++].ToString("X");
                                output.Append(s);
                                remainder -= s.Length;
                                break;
                            }
                            case (byte) 'o':
                            {
                                // TODO: Check this works
                                var s = IData[offseti++].ToString("X9");
                                output.Append(s);
                                remainder -= s.Length;
                                break;
                            }
                            case (byte) 'l':
                            {
                                remainder -= output.AppendByteAsChar((byte) IData[offseti++]);
                                break;
                            }
                            case (byte) 'h':
                            {
                                output.Append(Server.Name);
                                remainder -= Server.Name.Length;
                                break;
                            }
                            case (byte) 'n':
                            {
                                output.Append(Client.Name);
                                remainder -= Client.Name.Length;
                                break;
                            }
                            //case (byte)'a': { output += User.Nickname(); break; }
                            //case (byte)'i': { string sValue = (string)parameters[0]; output += sValue; parameters.RemoveAt(0); break; }
                            case (byte) 'c':
                            {
                                output.Append(Channel.Name);
                                remainder -= Channel.Name.Length;
                                break;
                            }
                            case (byte) 'u':
                            {
                                output.Append(Client.Address._address[2]);
                                remainder -= Client.Address._address[2].Length;
                                break;
                            }
                            default:
                            {
                                output.Append('%');
                                remainder -= output.AppendByteAsChar(RawText.ToByteArray()[i]) - 1;
                                break;
                            }
                        }

                    break;
                }
                default:
                {
                    remainder -= output.AppendByteAsChar(RawText.ToByteArray()[i]);
                    break;
                }
            }

        if (Data != null) //append overflow
            while (offsetd < Data.Length)
                output.Append(Data[offsetd++]);

        if (Newline) output.Append(Resources.CRLF);
        return StringBuilderExtensions.FromBytes(output.ToByteArray(), 0, output.Length).ToString();
    }
}
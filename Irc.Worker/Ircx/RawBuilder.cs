using System;
using System.Text;
using Irc.ClassExtensions.CSharpTools;
using Irc.Constants;
using Irc.Worker.Ircx.Objects;

namespace Irc.Worker.Ircx;

public class RawBuilder
{
    public static string Create(Server Server = null, Channel Channel = null, Client Client = null, string Raw = null,
        string[] Data = null, int[] IData = null, bool Newline = true)
    {
        int dataLen = 0, offsetd = 0, offseti = 0, remainder = 0;

        var output = new StringBuilder(512);
        remainder = 510 - Raw.Length;

        for (var i = 0; i < Raw.Length; i++)
            switch (Raw[i])
            {
                case '%':
                {
                    i++;
                    if (i < Raw.Length)
                        switch (Raw[i])
                        {
                            case 's':
                            {
                                var len = Data[offsetd].Length;
                                if (len > 0)
                                {
                                    if (len > remainder) len = remainder;
                                    output.Append(Data[offsetd++].Substring(0, len));
                                    remainder -= len;
                                }

                                break;
                            }
                            case 'S':
                            {
                                //put all of string array in
                                while (offsetd < Data.Length)
                                {
                                    output.Append(Data[offsetd]);
                                    remainder -= Data[offsetd++].Length;
                                }

                                break;
                            }
                            case 'd':
                            {
                                output.Append(IData[offseti].ToString());
                                remainder -= IData[offseti++].ToString().Length;
                                break;
                            }
                            case 'x':
                            {
                                var value = IData[offseti++].ToString("X");
                                remainder -= value.Length;
                                output.Append(value);
                                break;
                            }
                            case 'o':
                            {
                                var value = IData[offseti++].ToString("X9");
                                remainder -= value.Length;
                                output.Append(value);
                                break;
                            }
                            case 'l':
                            {
                                remainder -= 1;
                                output.Append((char) IData[offseti++]);
                                break;
                            }
                            case 'h':
                            {
                                var value = Server.Name;
                                remainder -= value.Length;
                                output.Append(value);
                                break;
                            }
                            case 'n':
                            {
                                var value = Client.Name;
                                remainder -= value.Length;
                                output.Append(value);
                                break;
                            }
                            //case (byte)'a': { output += User.Nickname(); break; }
                            //case (byte)'i': { string sValue = (string)parameters[0]; output += sValue; parameters.RemoveAt(0); break; }
                            case 'c':
                            {
                                var value = Channel.Name;
                                remainder -= value.Length;
                                output.Append(value);
                                break;
                            }
                            case 'u':
                            {
                                remainder -= Client.Address.GetAddress().Length;
                                output.Append(Client.Address.GetAddress());
                                break;
                            }
                            default:
                            {
                                output.Append('%');
                                output.Append(Raw[i]);
                                remainder -= 2;
                                break;
                            }
                        }

                    break;
                }
                default:
                {
                    output.Append(Raw[i]);
                    remainder--;
                    break;
                }
            }

        if (Data != null) //Append overflow
            while (offsetd < Data.Length)
                output.Append(Data[offsetd++]);

        if (Newline) output.Append(Resources.CRLF);
        return new string(output.ToString());
    }
}
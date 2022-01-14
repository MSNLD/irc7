using System.Collections.Generic;
using Irc.ClassExtensions.CSharpTools;

namespace Irc.Worker.Ircx;

public class Message
{
    /*
       <message>  ::= [':' <prefix> <SPACE> ] <command> <params> <crlf>
        <prefix>   ::= <servername> | <nick> [ '!' <user> ] [ '@' <host> ]
        <command>  ::= <letter> { <letter> } | <number> <number> <number>
        <SPACE>    ::= ' ' { ' ' }
        <params>   ::= <SPACE> [ ':' <trailing> | <middle> <params> ]

        <middle>   ::= <Any *non-empty* sequence of octets not including SPACE
                       or NUL or CR or LF, the first of which may not be ':'>
        <trailing> ::= <Any, possibly *empty*, sequence of octets not including
                         NUL or CR or LF>

        <crlf>     ::= CR LF
     */

    // TODO: To get rid of below
    public int ParamOffset;


    private readonly string _message;
    private string _prefix;
    private string _command;
    private List<string> _params = new();
    public List<string> Parameters
    {
        get { return _params; }
    }
    public string OriginalText
    {
        get { return _message; }
    }

    public Message(string message)
    {
        _message = message;
        parse();
    }

    public string GetPrefix => _prefix;
    public string GetCommand() => _command;
    public List<string> GetParameters() => _params;

    private bool getPrefix(string prefix)
    {
        if (prefix.StartsWith(':'))
        {
            _prefix = prefix.Substring(1);
            return true;
        }

        return false;
    }

    private bool getCommand(string command)
    {
        if (!string.IsNullOrWhiteSpace(command))
        {
            _command = command;
            return true;
        }

        return false;
    }

    private void parse()
    {
        if (string.IsNullOrWhiteSpace(_message)) return;

        var parts = _message.Split(' ');

        if (parts.Length > 0)
        {
            var index = 0;
            var cursor = 0;

            if (getPrefix(parts[index]))
            {
                index++;
                cursor = _prefix.Length + 1;
            }

            if (getCommand(parts[index]))
            {
                index++;
                cursor += _command.Length + 1;
            }

            for (; index < parts.Length; index++)
            {
                if (parts[index].StartsWith(':'))
                {
                    cursor++;
                    _params.Add(_message.Substring(cursor));
                    break;
                }

                _params.Add(parts[index]);
                cursor += parts[index].Length + 1;
            }
        }
    }
}
using Irc.Commands;
using Irc.Interfaces;

namespace Irc;

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


    private readonly IProtocol _protocol;
    private readonly string _message;
    private string _prefix;
    private ICommand _command;
    private string _commandName;
    private List<string> _params = new();
    public List<string> Parameters
    {
        get { return _params; }
    }
    public string OriginalText
    {
        get { return _message; }
    }

    public Message(IProtocol protocol, string message)
    {
        _protocol = protocol;
        _message = message;
        parse();
    }

    public string GetPrefix => _prefix;
    public ICommand GetCommand() => _command;
    public string GetCommandName() => _commandName;
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
            _commandName = command;
            _command = _protocol.GetCommand(command);
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
                cursor += parts[index].Length + 1;
                index++;
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
﻿using Irc.Commands;
using Irc.Interfaces;

namespace Irc;

public class Message : IMessage
{
    private readonly IProtocol _protocol;
    private ICommand _command;
    private string _commandName;

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

    public Message(IProtocol protocol, string message)
    {
        _protocol = protocol;
        OriginalText = message;
        parse();
    }

    public List<string> Parameters { get; } = new();

    public string OriginalText { get; }

    public string GetPrefix { get; private set; }

    public ICommand GetCommand()
    {
        return _command;
    }

    public string GetCommandName()
    {
        return _commandName;
    }

    public List<string> GetParameters()
    {
        return Parameters;
    }

    private bool getPrefix(string prefix)
    {
        if (prefix.StartsWith(':'))
        {
            GetPrefix = prefix.Substring(1);
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
        if (string.IsNullOrWhiteSpace(OriginalText)) return;

        var parts = OriginalText.Split(' ');

        if (parts.Length > 0)
        {
            var index = 0;
            var cursor = 0;

            if (getPrefix(parts[index]))
            {
                index++;
                cursor = GetPrefix.Length + 1;
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
                    Parameters.Add(OriginalText.Substring(cursor));
                    break;
                }

                Parameters.Add(parts[index]);
                cursor += parts[index].Length + 1;
            }
        }
    }
}
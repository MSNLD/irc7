using System;
using System.Collections.Generic;
using Irc.ClassExtensions.CSharpTools;
using Irc.Constants;
using Irc.Extensions.Security;
using Irc.Helpers.CSharpTools;
using Irc.Objects;

namespace Irc.Worker.Ircx.Objects;

public class Client : ChatObject
{
    private bool _isGuest;
    public Address Address;
    public SupportPackage Auth;
    public FloodProfile FloodProfile;
    public long LastActive;
    public long LastIdle;
    public long LastPing;
    public long LoggedOn;
    public bool WaitPing;
    public string RemoteIP = Resources.Wildcard;

    public Client(): base(new PropCollection())
    {
        base.Properties.Set(Resources.UserPropOid, Id.ToUnformattedString());
        LastActive = DateTime.UtcNow.Ticks;
        LastPing = DateTime.UtcNow.Ticks;

        IsConnected = true;
        Address = new Address();
        FloodProfile = new FloodProfile();
    }

    public bool Guest => Auth != null ? Auth.Guest : false;
    public bool Registered { get; private set; }

    public bool Authenticated
    {
        get
        {
            if (Auth != null)
                return Auth.Authenticated;
            return false;
        }
    }
    public FrameBuffer BufferIn { get; } = new();

    public Queue<string> BufferOut { get; } = new();

    public void Receive(Frame frame)
    {
        WaitPing = false;
        LastActive = DateTime.UtcNow.Ticks;
        LastPing = LastActive; // Reset last ping as communication has taken place
        FloodProfile.currentInputBytes += (uint)frame.Message.OriginalText.Length;
        Debug.Out(ShortId + ":RX: " + frame.Message.OriginalText);
        BufferIn.Queue.Enqueue(frame);
    }

    public bool IsConnected { get; private set; }

    public Queue<Frame> InputQueue => BufferIn.Queue;

    public void Send(string data)
    {
        Debug.Out(ShortId + ":TX: " + data);
        BufferOut.Enqueue($"{data}\r\n");
    }
    
    public bool Process(Frame Frame)
    {
        // to be moved somewhere better
        //Frame iFrame = base.BufferIn.Queue.Dequeue();
        if (Frame.Command != null)
        {
            if (Frame.Command.DataType == CommandDataType.Standard || Frame.Command.DataType == CommandDataType.Data ||
                Frame.Command.DataType == CommandDataType.Join) LastIdle = LastActive;

            return Frame.Command.Execute(Frame);
        }

        Frame.User.Send(RawBuilder.Create(Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_UNKNOWNCOMMAND_421,
            Data: new[] {Frame.Message.GetCommand() }));
        // No such command
        return true;
    }

    public void Register()
    {
        Registered = true;
        LoggedOn = DateTime.UtcNow.Ticks;
    }

    public void Unregister()
    {
        Registered = false;
    }

    public void Terminate()
    {
        IsConnected = false;
    }
}
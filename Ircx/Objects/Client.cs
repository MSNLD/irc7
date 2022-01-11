using System;
using System.Collections.Generic;
using Core.Authentication;
using CSharpTools;

namespace Core.Ircx.Objects;

public class Client : Obj
{
    private bool _isGuest;
    public Address Address;
    public SSP Auth;
    public FloodProfile FloodProfile;
    public long LastActive;
    public long LastIdle;
    public long LastPing;
    public long LoggedOn;
    public bool WaitPing;

    public Client(ObjType objType) : base(objType)
    {
        LastActive = DateTime.UtcNow.Ticks;
        LastPing = DateTime.UtcNow.Ticks;

        IsConnected = true;
        Address = new Address();
        FloodProfile = new FloodProfile();
    }

    public bool Guest => Auth != null ? Auth.guest : false;
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

    public bool IsConnected { get; private set; }

    public Queue<Frame> InputQueue => BufferIn.Queue;

    public void Send(string data)
    {
        Debug.Out(OIDX8 + ":TX: " + data);
        BufferOut.Enqueue(data);
    }

    public void Receive(Frame frame)
    {
        WaitPing = false;
        LastActive = DateTime.UtcNow.Ticks;
        LastPing = LastActive; // Reset last ping as communication has taken place
        FloodProfile.currentInputBytes += (uint) frame.Message.rawData.Length;
        base.Receive(frame);
    }

    public COM_RESULT Process(Frame Frame)
    {
        // to be moved somewhere better
        //Frame iFrame = base.BufferIn.Queue.Dequeue();
        if (Frame.Command != null)
        {
            if (Frame.Command.DataType == CommandDataType.Standard || Frame.Command.DataType == CommandDataType.Data ||
                Frame.Command.DataType == CommandDataType.Join) LastIdle = LastActive;

            return Frame.Command.Execute(Frame);
        }

        Frame.User.Send(Raws.Create(Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_UNKNOWNCOMMAND_421,
            Data: new[] {Frame.Message.Command}));
        // No such command
        return COM_RESULT.COM_SUCCESS;
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
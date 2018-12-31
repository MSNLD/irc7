using System;
using System.Collections.Generic;
using System.Text;
using CSharpTools;
using Core.Net;
using Core.Authentication;

namespace Core.Ircx.Objects
{
    public class Client: Obj
    {
        bool _isGuest;
        bool _isRegistered;
        bool _isConnected;
        public SSP Auth;
        public Address Address;
        public bool WaitPing;
        public long LastPing;
        public long LastActive;
        public long LastIdle;
        public long LoggedOn;
        public FloodProfile FloodProfile;

        public bool Guest { get { return (Auth != null ? Auth.guest : false); } }
        public bool Registered { get { return _isRegistered; } }
        public bool Authenticated
        {
            get
            {
                if (Auth != null) { return Auth.Authenticated; }
                else { return false; }
            }
        }

        public bool IsConnected { get { return _isConnected; } }

        public Client(ObjType objType): base(objType)
        {
            LastActive = DateTime.UtcNow.Ticks;
            LastPing = DateTime.UtcNow.Ticks;

            _isConnected = true;
            Address = new Address();
            FloodProfile = new FloodProfile();
        }
        public Queue<Frame> InputQueue { get { return BufferIn.Queue; } }

        public void Send(String8 data)
        {
            Debug.Out(base.OIDX8.chars + ":TX: " + data.chars);
            base.BufferOut.Enqueue(data);
        }
        public void Receive(Frame frame)
        {
            WaitPing = false;
            LastActive = DateTime.UtcNow.Ticks;
            LastPing = LastActive; // Reset last ping as communication has taken place
            FloodProfile.currentInputBytes += (uint)frame.Message.rawData.Length;
            base.Receive(frame);
        }
        public COM_RESULT Process(Frame Frame)
        {
            // to be moved somewhere better
            //Frame iFrame = base.BufferIn.Queue.Dequeue();
            if (Frame.Command != null)
            {
                if ((Frame.Command.DataType == CommandDataType.Standard) || (Frame.Command.DataType == CommandDataType.Data) || (Frame.Command.DataType == CommandDataType.Join))
                {
                    LastIdle = LastActive;
                }

                return Frame.Command.Execute(Frame);
            }
            else
            {
                Frame.User.Send(Raws.Create(Server: Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_UNKNOWNCOMMAND_421, Data: new String8[] { Frame.Message.Command }));
                // No such command
            }
            return COM_RESULT.COM_SUCCESS;
        }
        public void Register()
        {
            _isRegistered = true;
            LoggedOn = DateTime.UtcNow.Ticks;
        }
        public void Unregister()
        {
            _isRegistered = false;
        }
        public void Terminate()
        {
            _isConnected = false;
        }
    }
}

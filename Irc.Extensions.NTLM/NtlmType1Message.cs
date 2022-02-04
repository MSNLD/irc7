using System.Runtime.InteropServices;
using Irc.ClassExtensions.CSharpTools;
using Irc.Extensions.NTLM;
using Irc.Helpers.CSharpTools;

public class NtlmType1Message
{
    private NTLMShared.NTLMSSPMessageType1 _messageType1;
    private Dictionary<NtlmFlag, bool> _enumeratedFlags = new();
    public byte[] Signature { get; private set; }
    public string SuppliedWorkstation { get; private set; }
    public string SuppliedDomain { get; private set; }
    public Version ClientVersion { get; private set; }

    public Dictionary<NtlmFlag, bool> EnumeratedFlags => _enumeratedFlags;
    public uint Flags => _messageType1.Flags;

    public NtlmType1Message(string message)
    {
        Parse(message);
    }

    private void Parse(string message)
    {
        if (string.IsNullOrWhiteSpace(message)) 
            throw new ArgumentException("Message cannot be blank");

        if (message.Length < Marshal.SizeOf<NTLMShared.NTLMSSPMessageType1>()) 
            throw new ArgumentException("Message does not meet minimum length requirements");

        if (!message.StartsWith(NTLMShared.NTLMSignature)) 
            throw new ArgumentException("Message does not meet minimum requirements");

        _messageType1 = message.ToByteArray().Deserialize<NTLMShared.NTLMSSPMessageType1>();

        Signature = _messageType1.Signature;
        ClientVersion = new Version(_messageType1.OSVersionInfo.Major, _messageType1.OSVersionInfo.Minor,
            _messageType1.OSVersionInfo.BuildNumber, _messageType1.OSVersionInfo.Reserved);

        var suppliedWorkstationOffset = _messageType1.SuppliedWorkstation.Offset;
        var suppliedWorkstationLength = _messageType1.SuppliedWorkstation.Length;

        if (message.Length >= (suppliedWorkstationOffset + suppliedWorkstationLength))
            SuppliedWorkstation = message.Substring(_messageType1.SuppliedWorkstation.Offset, _messageType1.SuppliedWorkstation.Length);

        var suppliedDomainOffset = _messageType1.SuppliedDomain.Offset;
        var suppliedDomainLength = _messageType1.SuppliedDomain.Length;

        if (message.Length >= (suppliedDomainOffset + suppliedDomainLength))
            SuppliedDomain = message.Substring(_messageType1.SuppliedDomain.Offset, _messageType1.SuppliedDomain.Length);

        EnumerateFlags();
    }

    private void EnumerateFlags()
    {
        foreach (var flag in Enum.GetValues<NtlmFlag>())
        {
            _enumeratedFlags.Add(flag, ((uint)flag & _messageType1.Flags) != 0);
        }
    }
}
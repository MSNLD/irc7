using System.Formats.Asn1;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using Irc.ClassExtensions.CSharpTools;
using Irc.Extensions.NTLM;
using Irc.Extensions.NTLM.Cryptography;
using Irc.Helpers.CSharpTools;

public class NtlmType3Message
{
    private NTLMShared.NTLMSSPMessageType3 _messageType3;
    private Dictionary<NtlmFlag, bool> _flags = new();

    private string _data;
    private byte[] _byteData;

    private string _targetNameData;
    private string _userNameData;
    private string _workstationNameData;
    private string _lmResponseData;
    private string _ntlmResponseData;
    private string _sessionKeyData;

    private NTLMShared.NTLMSSPSecurityBuffer SessionKeySecBuf;
    private NTLMShared.NTLMSSPOSVersion OSVersionInfo;
    public uint Flags;

    public NtlmType3Message(string data)
    {
        _data = data;
        _byteData = _data.ToByteArray();
        Parse(_byteData);
    }

    public void Parse(byte[] data)
    {
        _messageType3 = data.Deserialize<NTLMShared.NTLMSSPMessageType3>();
        CopySecurityBuffers();
        EnumerateFlags();
    }

    private void CopySecurityBuffers()
    {
        if (_messageType3.LMResponse.Length > 0)
            _lmResponseData = _data.Substring(_messageType3.LMResponse.Offset, _messageType3.LMResponse.Length);

        if (_messageType3.NTLMResponse.Length > 0)
            _ntlmResponseData = _data.Substring(_messageType3.NTLMResponse.Offset, _messageType3.NTLMResponse.Length);

        if (_messageType3.TargetName.Length > 0)
            _targetNameData = _data.Substring(_messageType3.TargetName.Offset, _messageType3.TargetName.Length);

        if (_messageType3.UserName.Length > 0)
            _userNameData = _data.Substring(_messageType3.UserName.Offset, _messageType3.UserName.Length);

        if (_messageType3.WorkstationName.Length > 0)
            _workstationNameData = _data.Substring(_messageType3.WorkstationName.Offset, _messageType3.WorkstationName.Length);

        var legagyNTLM = (_messageType3.LMResponse.Offset == 52 || _messageType3.NTLMResponse.Offset == 52 ||
                          _messageType3.TargetName.Offset == 52 || _messageType3.UserName.Offset == 52 ||
                          _messageType3.WorkstationName.Offset == 52);

        if (legagyNTLM)
        {
            Flags = (uint)(NtlmFlag.NTLMSSP_NEGOTIATE_NTLM | NtlmFlag.NTLMSSP_NEGOTIATE_OEM);
        }
        else
        {
            SessionKeySecBuf = _data.Substring(52, Marshal.SizeOf<NTLMShared.NTLMSSPSecurityBuffer>()).ToByteArray()
                .Deserialize<NTLMShared.NTLMSSPSecurityBuffer>();
            Flags = _data.Substring(60, sizeof(uint)).ToByteArray().Deserialize<uint>();
            OSVersionInfo = _data.Substring(64, Marshal.SizeOf<NTLMShared.NTLMSSPOSVersion>()).ToByteArray()
                .Deserialize<NTLMShared.NTLMSSPOSVersion>();

            if (SessionKeySecBuf.Length > 0 && (SessionKeySecBuf.Offset + SessionKeySecBuf.Length <= _sessionKeyData.Length))
                _sessionKeyData = _data.Substring(SessionKeySecBuf.Offset, SessionKeySecBuf.Length);
        }
    }

    private void EnumerateFlags()
    {
        foreach (var flag in Enum.GetValues<NtlmFlag>())
        {
            _flags.Add(flag, ((uint)flag & Flags) != 0);
        }
    }
    
    public bool VerifySecurityContext(string challenge, string password)
    {
        NtlmResponses response = new NtlmResponses();

        if (_flags[NtlmFlag.NTLMSSP_NEGOTIATE_NTLM2])
        {
            try
            {
                bool authenticated = _ntlmResponseData.StartsWith(response.NtlmV2Response(_userNameData,
                    password.ToUnicodeString(), challenge, _ntlmResponseData));
                if (!authenticated)
                {
                    authenticated =
                        _ntlmResponseData.StartsWith(response.Ntlm2SessionResponse(password.ToUnicodeString(),
                            challenge,
                            _lmResponseData));
                }

                return authenticated;
            }
            catch (Exception)
            {

            }
        }
        else if (_flags[NtlmFlag.NTLMSSP_NEGOTIATE_NTLM])
        {
            try
            {
                var authenticated = false;
                authenticated = (_ntlmResponseData == response.NtlmResponse(password.ToUnicodeString(), challenge));
                if (!authenticated) return (_lmResponseData == response.LmResponse(password, challenge));
                //if (_flags[NtlmFlag.NTLMSSP_NEGOTIATE_NTLM])
                //{
                //return 
                //}
                //else // (_flags[NtlmFlag.NTLMSSP_NEGOTIATE_LM_KEY])
                //{
                    
                //}
            }
            catch (Exception)
            {

            }
        }

        return false;
    }
}
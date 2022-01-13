using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using Irc.ClassExtensions.CSharpTools;
using Irc.Extensions.Apollo.Security.Credentials;
using Irc.Extensions.Apollo.Security.Packages;
using Irc.Extensions.Security;

// ReSharper disable once CheckNamespace
namespace Irc.Extensions.Security.Packages;

public class GateKeeper : SupportPackage
{
    private static string _signature = "GKSSP\0";

    // Credit to JD for discovering the below key through XOR'ing (Discovered 2017/05/04)
    private static readonly string key = "SRFMKSJANDRESKKC";
    private char[] challenge = new char[8];
    
    protected GateKeeperToken ServerToken, ClientToken;

    public GateKeeper()
    {
        Guest = true;
        ServerSequence = EnumSupportPackageSequence.SSP_INIT;
    }

    public override EnumSupportPackageSequence InitializeSecurityContext(string data, string ip)
    {
        var lit = StringBuilderExtensions.ToLiteral(data);
        if (lit.Length >= 0x10)
            if (lit.ToString().StartsWith(_signature))
            {
                ClientToken = GateKeeperTokenHelper.InitializeFromBytes(lit.ToByteArray());
                if ((EnumSupportPackageSequence)ClientToken.Sequence == ServerSequence && ClientToken.Version >= 2 && ClientToken.Version <= 3)
                {
                    ServerSequence = EnumSupportPackageSequence.SSP_EXT; //expecting a SSP_EXT reply after challenge is sent
                    ServerVersion = ClientToken.Version;
                    return EnumSupportPackageSequence.SSP_OK;
                }
            }

        return EnumSupportPackageSequence.SSP_FAILED;
    }

    public override EnumSupportPackageSequence AcceptSecurityContext(string data, string ip)
    {
        var lit = StringBuilderExtensions.ToLiteral(data);
        if (lit.Length >= 0x20)
            if (lit.ToString().StartsWith(_signature))
            {
                ClientToken = GateKeeperTokenHelper.InitializeFromBytes(lit.ToByteArray());
                var _clientVersion = ClientToken.Version;
                var _clientStage = (EnumSupportPackageSequence)ClientToken.Sequence;
                if (_clientStage == ServerSequence && _clientVersion >= 2 && _clientVersion <= 3)
                {
                    var context = StringBuilderExtensions.FromBytes(lit.ToByteArray(), 16, 32).ToString();
                    if (VerifySecurityContext(challenge, context.ToByteArray(), ip, ServerVersion))
                    {
                        //Note that I need to improve the below code. Guid needs a ToByteArray() function
                        StringBuilder guidBinary;

                        if (lit.Length >= 0x30)
                        {
                            guidBinary = StringBuilderExtensions.FromBytes(lit.ToByteArray(), 32, 48);
                            Guid = new Guid(guidBinary.ToByteArray());
                        }
                        else
                        {
                            guidBinary = StringBuilderExtensions.FromBytes(Guid.NewGuid().ToByteArray(), 0, 16);
                            Guid = new Guid(guidBinary.ToByteArray());
                        }

                        if (Guid != Guid.Empty || Guest == false)
                        {
                            var memberIdLow = BitConverter.ToUInt64(guidBinary.ToByteArray(), 0);
                            var memberIdHigh = BitConverter.ToUInt64(guidBinary.ToByteArray(), 8);
                            ServerSequence = EnumSupportPackageSequence.SSP_AUTHENTICATED;
                            Authenticated = true;
                            return EnumSupportPackageSequence.SSP_OK;
                        }
                    }
                }
            }

        return EnumSupportPackageSequence.SSP_FAILED;
    }

    public override string GetDomain()
    {
        return nameof(GateKeeper);
    }

    public bool VerifySecurityContext(char[] challenge, byte[] context, string ip, uint version)
    {
        var md5 = new HMACMD5(key.ToByteArray());
        var ctx = new StringBuilder(challenge.Length + ip.Length);
        ctx.Append(challenge);
        if (version == 3) ctx.Append(ip);
        var h1 = md5.ComputeHash(ctx.ToByteArray(), 0, ctx.Length);
        return h1.SequenceEqual(context);
    }

    public override SupportPackage CreateInstance(ICredentialProvider credentialProvider)
    {
        return new GateKeeper();
    }

    public override string CreateSecurityChallenge(EnumSupportPackageSequence stage)
    {
        if (stage == EnumSupportPackageSequence.SSP_SEC)
        {
            ServerToken = GateKeeperTokenHelper.CreateGateKeeperToken(_signature);
            Array.Copy(Guid.NewGuid().ToByteArray(), 0, challenge, 0, 8);
          
            var message =
                new StringBuilder(Marshal.SizeOf(ServerToken) +
                                  challenge.Length); //create new message with full size
            ServerToken.Version = ClientToken.Version;
            ServerToken.Sequence = 2;
            message.AppendByteArrayAsChars(GateKeeperTokenHelper.GetBytes(ServerToken));
            message.Append(challenge);
            return message.ToString();
        }

        return null;
    }
}
using System.Security.Cryptography;
using System.Text;
using Irc.ClassExtensions.CSharpTools;
using Newtonsoft.Json;

namespace Irc.Extensions.Apollo.Security.Credentials;

public class Passport : ICredentialProvider
{
    public static int Version = 3;
    public static string Signature = "IRC7v1PP";
    private static byte[] _key;

    public Passport(string key)
    {
            _key = Encoding.ASCII.GetBytes(key);
    }

    public string Encrypt(object o, Base64.B64MapType BaseMap, bool IncludeVersion)
    {
        return Encrypt(o, null, BaseMap, IncludeVersion);
    }

    public string Encrypt(object o, byte[] iv, Base64.B64MapType BaseMap, bool IncludeVersion)
    {
        if (iv == null) iv = new byte[16];

        var json = JsonConvert.SerializeObject(o);
        var jsonBytes = new StringBuilder(json);

        var aes = Aes.Create();
        aes.BlockSize = 128;
        aes.KeySize = 128;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        var encryptor = aes.CreateEncryptor(_key, iv);
        StringBuilder final;

        using (var msEncrypt = new MemoryStream())
        {
            using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
            {
                csEncrypt.Write(jsonBytes.ToByteArray(), 0, jsonBytes.Length);
                csEncrypt.FlushFinalBlock();
                final = StringBuilderExtensions.FromBytes(msEncrypt.ToArray());
            }
        }

        var b64 = Base64.Encode(final.ToString(), BaseMap, 0);
        return (IncludeVersion ? Version.ToString() : "") + b64;
    }

    public Ticket Decrypt(StringBuilder cookie)
    {
        Ticket t;

        var encrypted = Base64.Decode(cookie.ToString(), Base64.B64MapType.MSPassport, true);

        var aes = Aes.Create();
        aes.BlockSize = 128;
        aes.KeySize = 128;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;


        var decryptor = aes.CreateDecryptor(_key, new byte[16]);
        string s;

        using (var msDecrypt = new MemoryStream(encrypted.ToByteArray()))
        {
            using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
            {
                try
                {
                    using (var srDecrypt = new StreamReader(csDecrypt))
                    {
                        s = srDecrypt.ReadToEnd();
                    }
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        try
        {
            t = JsonConvert.DeserializeObject<Ticket>(s);
        }
        catch (Exception)
        {
            return null;
        }

        return t;
    }

    public Profile Decrypt(StringBuilder cookie, byte[] IV)
    {
        Profile p;

        var encrypted = Base64.Decode(cookie.ToString(), Base64.B64MapType.MSPassport, true);

        var aes = Aes.Create();
        aes.BlockSize = 128;
        aes.KeySize = 128;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;


        var decryptor = aes.CreateDecryptor(_key, IV);
        string s;

        using (var msDecrypt = new MemoryStream(encrypted.ToByteArray()))
        {
            using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
            {
                try
                {
                    using (var srDecrypt = new StreamReader(csDecrypt))
                    {
                        s = srDecrypt.ReadToEnd();
                    }
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        try
        {
            p = JsonConvert.DeserializeObject<Profile>(s);
        }
        catch (Exception)
        {
            return null;
        }

        return p;
    }

    public RegCookie DecryptRegCookie(string cookie)
    {
        RegCookie r;

        var encrypted = Base64.Decode(cookie, Base64.B64MapType.MSRegCookie, false);

        var aes = Aes.Create();
        aes.BlockSize = 128;
        aes.KeySize = 128;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;


        var decryptor = aes.CreateDecryptor(_key, new byte[16]);
        string s;

        using (var msDecrypt = new MemoryStream(encrypted.ToByteArray()))
        {
            using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
            {
                try
                {
                    using (var srDecrypt = new StreamReader(csDecrypt))
                    {
                        s = srDecrypt.ReadToEnd();
                    }
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        try
        {
            r = JsonConvert.DeserializeObject<RegCookie>(s);
        }
        catch (Exception)
        {
            return null;
        }

        return r;
    }


    public string CreatePassportID(string provider, string id)
    {
        var md5 = MD5.Create();
        md5.Initialize();
        var x = md5.ComputeHash(Encoding.ASCII.GetBytes(id));
        var x2 = md5.ComputeHash(Encoding.ASCII.GetBytes(provider));

        var iv = new byte[16];
        var final = new byte[16];

        var aes = Aes.Create();
        aes.BlockSize = 128;
        aes.KeySize = 128;
        aes.Mode = CipherMode.CBC;


        var encryptor = aes.CreateEncryptor(x2, iv);


        using (var msEncrypt = new MemoryStream())
        {
            using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
            {
                csEncrypt.Write(x, 0, 16);
                final = msEncrypt.ToArray();
            }
        }

        var a = new byte[8];
        var b = new byte[8];
        var c = new byte[8];

        Array.Copy(final, 0, a, 0, 8);
        Array.Copy(final, 8, b, 0, 8);

        for (var i = 0; i < 8; i++) c[i] = (byte) (a[i] ^ b[i]);

        return BitConverter.ToString(c, 0, 8).Replace("-", "");
    }
}
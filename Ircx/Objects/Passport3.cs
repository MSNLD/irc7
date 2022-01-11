using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Core.CSharpTools;
using CSharpTools;
using Newtonsoft.Json;

namespace Core.Ircx.Objects;

public class PassportTicket
{
    public long issueDate;
    public byte[] iv;
    public string puid;
    public int version;

    public PassportTicket()
    {
        version = Passport3.Version;
    }
}

public class PassportProfile
{
    public string origId;
    public string profileId;
    public string provider;
    public int version;

    public PassportProfile()
    {
        version = Passport3.Version;
    }
}

public class RegCookie
{
    public long issueDate;
    public string nickname;
    public string salt;
    public int version;

    public RegCookie()
    {
        version = Passport3.Version;
    }
}

public class Passport3
{
    public static int Version = 3;
    public static string Signature = "IRC7v1PP";
    private static readonly byte[] Key = Encoding.ASCII.GetBytes(Program.Config.PassportKey);

    public static string Encrypt(object o, Base64.B64MapType BaseMap, bool IncludeVersion)
    {
        return Encrypt(o, null, BaseMap, IncludeVersion);
    }

    public static string Encrypt(object o, byte[] iv, Base64.B64MapType BaseMap, bool IncludeVersion)
    {
        if (iv == null) iv = new byte[16];

        var json = JsonConvert.SerializeObject(o);
        var jsonBytes = new StringBuilder(json);

        var aes = Aes.Create();
        aes.BlockSize = 128;
        aes.KeySize = 128;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        var encryptor = aes.CreateEncryptor(Key, iv);
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

    public static PassportTicket Decrypt(StringBuilder cookie)
    {
        PassportTicket t;

        var encrypted = Base64.Decode(cookie.ToString(), Base64.B64MapType.MSPassport, true);

        var aes = Aes.Create();
        aes.BlockSize = 128;
        aes.KeySize = 128;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;


        var decryptor = aes.CreateDecryptor(Key, new byte[16]);
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
                catch (Exception e)
                {
                    return null;
                }
            }
        }

        try
        {
            t = JsonConvert.DeserializeObject<PassportTicket>(s);
        }
        catch (Exception e)
        {
            return null;
        }

        return t;
    }

    public static PassportProfile Decrypt(StringBuilder cookie, byte[] IV)
    {
        PassportProfile p;

        var encrypted = Base64.Decode(cookie.ToString(), Base64.B64MapType.MSPassport, true);

        var aes = Aes.Create();
        aes.BlockSize = 128;
        aes.KeySize = 128;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;


        var decryptor = aes.CreateDecryptor(Key, IV);
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
                catch (Exception e)
                {
                    return null;
                }
            }
        }

        try
        {
            p = JsonConvert.DeserializeObject<PassportProfile>(s);
        }
        catch (Exception e)
        {
            return null;
        }

        return p;
    }

    public static RegCookie DecryptRegCookie(string cookie)
    {
        RegCookie r;

        var encrypted = Base64.Decode(cookie, Base64.B64MapType.MSRegCookie, false);

        var aes = Aes.Create();
        aes.BlockSize = 128;
        aes.KeySize = 128;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;


        var decryptor = aes.CreateDecryptor(Key, new byte[16]);
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
                catch (Exception e)
                {
                    return null;
                }
            }
        }

        try
        {
            r = JsonConvert.DeserializeObject<RegCookie>(s);
        }
        catch (Exception e)
        {
            return null;
        }

        return r;
    }


    public static string CreatePassportID(string provider, string id)
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
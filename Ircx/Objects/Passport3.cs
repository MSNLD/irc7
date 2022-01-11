using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using CSharpTools;
using Core.CSharpTools;

namespace Core.Ircx.Objects
{
    public class PassportTicket
    {
        public int version;
        public byte[] iv;
        public string puid;
        public long issueDate;

        public PassportTicket()
        {
            version = Passport3.Version;
        }
    }
    public class PassportProfile
    {
        public int version;
        public string profileId;
        public string provider;
        public string origId;

        public PassportProfile()
        {
            version = Passport3.Version;
        }
    }
    public class RegCookie
    {
        public int version;
        public string nickname;
        public long issueDate;
        public string salt;

        public RegCookie()
        {
            version = Passport3.Version;
        }
    }

    public class Passport3
    {
        public static int Version = 3;
        public static string Signature = "IRC7v1PP";
        static byte[] Key = ASCIIEncoding.ASCII.GetBytes(Program.Config.PassportKey);

        public static string Encrypt(Object o, Base64.B64MapType BaseMap, bool IncludeVersion) { return Encrypt(o, null, BaseMap, IncludeVersion); }
        public static string Encrypt(Object o, byte[] iv, Base64.B64MapType BaseMap, bool IncludeVersion)
        {
            if (iv == null) { iv = new byte[16]; }

            string json = JsonConvert.SerializeObject(o);
            StringBuilder jsonBytes = new StringBuilder(json);

            Aes aes = Aes.Create();
            aes.BlockSize = 128;
            aes.KeySize = 128;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            ICryptoTransform encryptor = aes.CreateEncryptor(Key, iv);
            StringBuilder final;

            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    csEncrypt.Write(jsonBytes.ToByteArray(), 0, jsonBytes.Length);
                    csEncrypt.FlushFinalBlock();
                    final = StringBuilderExtensions.FromBytes(msEncrypt.ToArray());
                }
            }

            string b64 = Base64.Encode(final.ToString(), BaseMap, 0);
            return (IncludeVersion == true ? Version.ToString() : "") + b64.ToString();
        }

        public static PassportTicket Decrypt(StringBuilder cookie)
        {

            PassportTicket t;

            string encrypted = Base64.Decode(cookie.ToString(), Base64.B64MapType.MSPassport, true);

            Aes aes = Aes.Create();
            aes.BlockSize = 128;
            aes.KeySize = 128;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;


            ICryptoTransform decryptor = aes.CreateDecryptor(Key, new byte[16]);
            string s;

            using (MemoryStream msDecrypt = new MemoryStream(encrypted.ToByteArray()))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    try
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            s = srDecrypt.ReadToEnd();

                        }
                    }
                    catch (Exception e) { return null; }
                }
            }

            try { t = JsonConvert.DeserializeObject<PassportTicket>(s); }
            catch (Exception e) { return null; }

            return t;


        }
        public static PassportProfile Decrypt(StringBuilder cookie, byte[] IV)
        {

            PassportProfile p;

            string encrypted = Base64.Decode(cookie.ToString(), Base64.B64MapType.MSPassport, true);

            Aes aes = Aes.Create();
            aes.BlockSize = 128;
            aes.KeySize = 128;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;


            ICryptoTransform decryptor = aes.CreateDecryptor(Key, IV);
            string s;

            using (MemoryStream msDecrypt = new MemoryStream(encrypted.ToByteArray()))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    try
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            s = srDecrypt.ReadToEnd();

                        }
                    }
                    catch (Exception e) { return null; }
                }
            }

            try { p = JsonConvert.DeserializeObject<PassportProfile>(s); }
            catch (Exception e) { return null; }

            return p;


        }

        public static RegCookie DecryptRegCookie(string cookie)
        {

            RegCookie r;

            string encrypted = Base64.Decode(cookie.ToString(), Base64.B64MapType.MSRegCookie, false);

            Aes aes = Aes.Create();
            aes.BlockSize = 128;
            aes.KeySize = 128;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;


            ICryptoTransform decryptor = aes.CreateDecryptor(Key, new byte[16]);
            string s;

            using (MemoryStream msDecrypt = new MemoryStream(encrypted.ToByteArray()))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    try
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            s = srDecrypt.ReadToEnd();

                        }
                    }
                    catch (Exception e) { return null; }
                }
            }

            try { r = JsonConvert.DeserializeObject<RegCookie>(s); }
            catch (Exception e) { return null; }

            return r;


        }


        public static String CreatePassportID(string provider, string id)
        {
            MD5 md5 = MD5.Create();
            md5.Initialize();
            byte[] x = md5.ComputeHash(System.Text.ASCIIEncoding.ASCII.GetBytes(id));
            byte[] x2 = md5.ComputeHash(System.Text.ASCIIEncoding.ASCII.GetBytes(provider));

            byte[] iv = new byte[16];
            byte[] final = new byte[16];

            Aes aes = Aes.Create();
            aes.BlockSize = 128;
            aes.KeySize = 128;
            aes.Mode = CipherMode.CBC;


            ICryptoTransform encryptor = aes.CreateEncryptor(x2, iv);


            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    csEncrypt.Write(x, 0, 16);
                    final = msEncrypt.ToArray();
                }
            }

            byte[] a = new byte[8];
            byte[] b = new byte[8];
            byte[] c = new byte[8];

            Array.Copy(final, 0, a, 0, 8);
            Array.Copy(final, 8, b, 0, 8);

            for (int i = 0; i < 8; i++)
            {
                c[i] = (byte)(a[i] ^ b[i]);
            }

            return BitConverter.ToString(c, 0, 8).Replace("-", "");
        }

    }
}

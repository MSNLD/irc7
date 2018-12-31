using System;
using System.Collections.Generic;
using System.Text;
using CSharpTools;
using System.IO;
using System.Security.Cryptography;

namespace Core.Authentication
{
    class DES3
    {
        TripleDES DES;
        public String8 Key;
        public String8 IV;


        public DES3()
        {
            DES = TripleDES.Create();
            DES.Mode = CipherMode.CBC;
        }
        public String8 Encrypt(String8 data)
        {
            DES.BlockSize = 128;
            DES.Key = Key.bytes;
            DES.IV = IV.bytes;
            using (MemoryStream MS = new MemoryStream())
            {
                using (CryptoStream CS = new CryptoStream(MS, DES.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    CS.Write(data.bytes, 0, data.Capacity);
                    CS.FlushFinalBlock();
                }
                return new String8(MS.ToArray());
            }
        }
        public String8 Decrypt(String8 data)
        {
            if ((data.length > 0) && (data.length % 8 == 0))
            {
                if (System.Security.Cryptography.TripleDES.IsWeakKey(Key.bytes)) { return null; }

                DES.Key = Key.bytes;
                DES.IV = IV.bytes;
                using (MemoryStream MS = new MemoryStream())
                {
                    using (CryptoStream CS = new CryptoStream(MS, DES.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        CS.Write(data.bytes, 0, data.length);
                        using (CS)
                        {
                            try
                            {
                                CS.FlushFinalBlock();

                            }
                            catch (Exception)
                            {
                                return null;
                            }
                        }
                    }
                    return new String8(MS.ToArray());
                }
            }
            return null;
        }
    }
    class DES
    {

        public DES()
        {
        }

        public String8 CreateKey()
        {
            String8 Key = new String8(Guid.NewGuid().ToByteArray(), 8, 16);
            while (TripleDES.IsWeakKey(Key.bytes)) { Key = new String8(Guid.NewGuid().ToByteArray(), 8, 16); }
            return Key;
        }

        public String8 Encrypt(String8 data, String8 key)
        {
            if (key.length < 8) { key = new String8(createDESkey(key.bytes)); }
            if (TripleDES.IsWeakKey(key.bytes)) { return null; }

            SymmetricAlgorithm DES;
            DES = TripleDES.Create();
            DES.Mode = CipherMode.ECB;
            DES.Padding = PaddingMode.None;

            using (MemoryStream MS = new MemoryStream())
            {

                byte[] bytData = data.bytes;
                byte[] bytIV = { 0, 0, 0, 0, 0, 0, 0, 0 };
                byte[] bytKey = key.bytes;
                DES.Key = bytKey;
                DES.IV = bytIV;
                ICryptoTransform ICT = DES.CreateEncryptor();
                using (CryptoStream CS = new CryptoStream(MS, ICT, CryptoStreamMode.Write))
                {
                    CS.Write(bytData, 0, bytData.Length);
                    CS.FlushFinalBlock();
                }

                return new String8(MS.ToArray());
            }
        }
        private byte[] oddParity(byte[] bytes)
        {
            for (int i = 0; i <= bytes.Length - 1; i++)
            {
                byte b = bytes[i];
                bool needsParity = (((b >> 7) ^ (b >> 6) ^ (b >> 5) ^
                                            (b >> 4) ^ (b >> 3) ^ (b >> 2) ^
                                            (b >> 1)) & 0x01) == 0;
                if (needsParity)
                {
                    bytes[i] |= 0x01;
                }
                else
                {
                    bytes[i] &= 0xfe;
                }
            }
            return bytes;
        }
        private byte[] createDESkey(byte[] keyBytes)
        {
            byte[] material = new byte[8];
            material[0] = (byte)keyBytes[0];
            material[1] = (byte)(keyBytes[0] << 7 | (keyBytes[1] & 0xff) >> 1);
            material[2] = (byte)(keyBytes[1] << 6 | (keyBytes[2] & 0xff) >> 2);
            material[3] = (byte)(keyBytes[2] << 5 | (keyBytes[3] & 0xff) >> 3);
            material[4] = (byte)(keyBytes[3] << 4 | (keyBytes[4] & 0xff) >> 4);
            material[5] = (byte)(keyBytes[4] << 3 | (keyBytes[5] & 0xff) >> 5);
            material[6] = (byte)(keyBytes[5] << 2 | (keyBytes[6] & 0xff) >> 6);
            material[7] = (byte)(keyBytes[6] << 1);
            return oddParity(material);
        }
    }
}

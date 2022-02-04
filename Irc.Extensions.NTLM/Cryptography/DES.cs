using System.Security.Cryptography;
using Irc.ClassExtensions.CSharpTools;
using Irc.Helpers.CSharpTools;

namespace Irc.Extensions.NTLM.Cryptography
{
    class DesEncryptor
    {

        public DesEncryptor()
        {
        }

        public byte[] CreateKey()
        {
            byte[] key = new byte[8];
            do
            {
                Array.Copy(key, 0, Guid.NewGuid().ToByteArray(), 16, 8);
            } while (DES.IsWeakKey(key));
            return key;
        }

        public string Encrypt(string data, byte[] key)
        {
            SymmetricAlgorithm symmetricAlgorithm;

            symmetricAlgorithm = DES.Create();
            symmetricAlgorithm.Mode = CipherMode.ECB;
            symmetricAlgorithm.Padding = PaddingMode.None;

            using (MemoryStream MS = new MemoryStream())
            {

                byte[] bytData = data.ToByteArray();
                byte[] bytIV = {0, 0, 0, 0, 0, 0, 0, 0};
                byte[] bytKey = key;
                symmetricAlgorithm.Key = bytKey;
                symmetricAlgorithm.IV = bytIV;
                ICryptoTransform ICT = symmetricAlgorithm.CreateEncryptor();
                using (CryptoStream CS = new CryptoStream(MS, ICT, CryptoStreamMode.Write))
                {
                    CS.Write(bytData, 0, bytData.Length);
                    CS.FlushFinalBlock();
                }

                return MS.ToArray().ToAsciiString();
            }
        }
        public byte[] Encrypt(byte[] data, byte[] key)
        {
            SymmetricAlgorithm symmetricAlgorithm;

            symmetricAlgorithm = DES.Create();
            symmetricAlgorithm.Mode = CipherMode.ECB;
            symmetricAlgorithm.Padding = PaddingMode.None;

            using (MemoryStream MS = new MemoryStream())
            {

                byte[] bytData = data;
                byte[] bytIV = { 0, 0, 0, 0, 0, 0, 0, 0 };
                byte[] bytKey = key;
                symmetricAlgorithm.Key = bytKey;
                symmetricAlgorithm.IV = bytIV;
                ICryptoTransform ICT = symmetricAlgorithm.CreateEncryptor();
                using (CryptoStream CS = new CryptoStream(MS, ICT, CryptoStreamMode.Write))
                {
                    CS.Write(bytData, 0, bytData.Length);
                    CS.FlushFinalBlock();
                }

                return MS.ToArray();
            }
        }


    }
}

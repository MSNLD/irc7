using System.Runtime.InteropServices;

namespace Irc.Helpers.CSharpTools
{
    public static class SerializationExtensions
    {
        public static byte[] Serialize<T>(this Object serializableObject)
        {
            int ptrMessageSize = Marshal.SizeOf<T>();
            byte[] serialBytes = new byte[ptrMessageSize];
            IntPtr pBuf = Marshal.AllocHGlobal(ptrMessageSize);
            try
            {
                Marshal.StructureToPtr(serializableObject, pBuf, false);
                for (int i = 0; i < ptrMessageSize; i++)
                {
                    serialBytes[i] = Marshal.ReadByte(pBuf, i);
                }

                return serialBytes;
            }
            catch (Exception e)
            {
                return null;
            }
            finally
            {
                Marshal.FreeHGlobal(pBuf);
            }
        }
        public static T Deserialize<T>(this byte[] bytes)
        {
            int size = Marshal.SizeOf<T>();
            IntPtr pBuf = IntPtr.Zero;
            try
            {
                pBuf = Marshal.AllocHGlobal(size);
                Marshal.Copy(bytes, 0, pBuf, size);
                return Marshal.PtrToStructure<T>(pBuf);
            }
            catch (Exception e)
            {
                return default(T);
            }
            finally
            {
                if (pBuf != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(pBuf);
                }
            }
        }
    }
}

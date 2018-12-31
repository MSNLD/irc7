using Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpTools
{
    public static class Debug
    {
        private static bool bDebug;
        private static bool bVerbose;
        public static bool Enabled { get { return bDebug; } }
        public static bool Verbose { get { return bVerbose; } }
        public static void Out(string s)
        {
            if (bVerbose)
            {
                Console.WriteLine(s);
            }
        }
        public static void Enable()
        {
            bDebug = true;
        }
        public static void Disable()
        {
            bDebug = false;
        }
        public static void EnableVerbose() { bVerbose = true; }
        public static void DisableVerbose() { bVerbose = false; }
    }
}

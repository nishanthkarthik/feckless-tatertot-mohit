using System;
using System.Diagnostics;
using System.IO;

namespace HeatExchange.CoolPropHelper
{
    class XPlatHelper
    {
        public static void DeterminePlatform()
        {
            string startupDirEndingWithSlash = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\";
            string resolvedDomainTimeFileName = startupDirEndingWithSlash + "CoolProp.dll";
            if (!File.Exists(resolvedDomainTimeFileName))
            {
                if (Environment.Is64BitProcess)
                {
                    if (File.Exists(startupDirEndingWithSlash + "CoolProp_x64.dll"))
                        File.Copy(startupDirEndingWithSlash + "CoolProp_x64.dll", resolvedDomainTimeFileName, true);
                }
                else
                {
                    if (File.Exists(startupDirEndingWithSlash + "CoolProp_x86.dll"))
                        File.Copy(startupDirEndingWithSlash + "CoolProp_x86.dll", resolvedDomainTimeFileName, true);
                }
            }
        }

        public static void OpenImageTables()
        {
            string startupDirEndingWithSlash = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\";
            string s1 = startupDirEndingWithSlash + "Assets\\S1.png";
            string s2 = startupDirEndingWithSlash + "Assets\\S2.png";
            string s3 = startupDirEndingWithSlash + "Assets\\S3.png";
            Process.Start(s1);
            Process.Start(s2);
            Process.Start(s3);
        }
    }
}

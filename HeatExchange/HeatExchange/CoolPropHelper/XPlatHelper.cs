using System;
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
                        File.Copy(startupDirEndingWithSlash + "CoolProp_x64.dll", resolvedDomainTimeFileName);
                }
                else
                {
                    if (File.Exists(startupDirEndingWithSlash + "CoolProp_x86.dll"))
                        File.Copy(startupDirEndingWithSlash + "CoolProp_x86.dll", resolvedDomainTimeFileName);
                }
            }
        }
    }
}

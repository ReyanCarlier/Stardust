using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Stardust.Memory
{
    public static class CPUInfo
    {
        public static float GetCPUDetails()
        {
            float result;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                using PerformanceCounter pc = new("Processor", "% Processor Time", "_Total");
                _ = pc.NextValue();
                Thread.Sleep(1000);
                return pc.NextValue();
            }
            else
            {
                result = 0;
            }

            return result;
        }
    }
}

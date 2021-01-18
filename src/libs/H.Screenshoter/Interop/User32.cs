using System.Runtime.InteropServices;
using PInvoke;

namespace H.Utilities.Interop
{
    internal static class User32
    {
        [DllImport("user32", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern bool EnumDisplayMonitors(
            nint hdc,
            nint lpRect, 
            MonitorEnumProc callback,
            nint dwData);

        internal delegate bool MonitorEnumProc(
            nint hDesktop,
            nint hdc, 
            ref RECT pRect,
            nint dwData);

        [DllImport("user32", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern bool GetMonitorInfo(
            nint hMonitor, 
            ref MonitorInfoEx info);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 4)]
        internal struct MonitorInfoEx
        {
            public uint cbSize;
            public RECT rcMonitor;
            public RECT rcWork;
            public uint dwFlags;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public char[] szDevice;

            public static MonitorInfoEx Create()
            {
                return new()
                {
                    cbSize = (uint) Marshal.SizeOf(typeof(MonitorInfoEx)),
                };
            }
        }
    }
}

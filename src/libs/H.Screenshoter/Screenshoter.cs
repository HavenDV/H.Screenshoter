using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using H.Utilities.Extensions;
using H.Utilities.Utilities;
using PInvoke;

namespace H.Utilities
{
    /// <summary>
    /// 
    /// </summary>
    public static class Screenshoter
    {
        #region PInvokes

        [DllImport("user32", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool EnumDisplayMonitors(
            nint hdc,
            nint lpRect, 
            MonitorEnumProc callback,
            nint dwData);
        
        private delegate bool MonitorEnumProc(
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

        #endregion

        #region Public methods

        /// <summary>
        /// Returns list of physical display rectangles(without considering DPI).
        /// X and Y can be negative.
        /// </summary>
        /// <returns></returns>
        public static IReadOnlyCollection<Rectangle> GetPhysicalScreens()
        {
            var list = new List<Rectangle>();
            bool Callback(nint hDesktop, nint hdc, ref RECT rect, nint dwData)
            {
                var info = MonitorInfoEx.Create();
                GetMonitorInfo(hDesktop, ref info).Check();

                var settings = DEVMODE.Create();
                User32.EnumDisplaySettings(
                    info.szDevice,
                    User32.ENUM_CURRENT_SETTINGS,
                    ref settings);

                var x = settings.dmPosition.x;
                var y = settings.dmPosition.y;
                var width = settings.dmPelsWidth;
                var height = settings.dmPelsHeight;

                list.Add(new Rectangle(x, y, (int)width, (int)height));

                return true;
            }

            EnumDisplayMonitors(0, 0, Callback, 0).Check();

            return list;
        }

        /// <summary>
        /// Returns rectangle of physical display(without considering DPI).
        /// X and Y can be negative.
        /// </summary>
        /// <returns></returns>
        public static Rectangle GetPhysicalScreenRectangle()
        {
            return GetPhysicalScreens().Aggregate(Rectangle.Union);
        }

        /// <summary>
        /// Creates screenshot of all available screens. <br/>
        /// If <paramref name="cropRectangle"/> is not null, returns image of this region.
        /// </summary>
        /// <param name="cropRectangle"></param>
        /// <returns></returns>
        public static Bitmap Shot(Rectangle? cropRectangle = null)
        {
            var rectangle = (cropRectangle ?? GetPhysicalScreenRectangle()).Normalize();
            
            var window = User32.GetDesktopWindow();
            using var dc = User32.GetWindowDC(window);
            using var toDc = Gdi32.CreateCompatibleDC(dc);
            var hBmp = Gdi32.CreateCompatibleBitmap(dc, rectangle.Width, rectangle.Height);
            var hOldBmp = Gdi32.SelectObject(toDc, hBmp);

            // ReSharper disable once BitwiseOperatorOnEnumWithoutFlags
            Gdi32.BitBlt(toDc.DangerousGetHandle(), 
                0, 
                0,
                rectangle.Width,
                rectangle.Height, 
                dc.DangerousGetHandle(),
                rectangle.X,
                rectangle.Y, 
                (int)(CopyPixelOperation.CaptureBlt | CopyPixelOperation.SourceCopy));

            var bitmap = Image.FromHbitmap(hBmp);
            Gdi32.SelectObject(toDc, hOldBmp);
            Gdi32.DeleteObject(hBmp).Check();
            Gdi32.DeleteDC(toDc).Check(); //?
            User32.ReleaseDC(window, dc.DangerousGetHandle()).Check(); //?

            return bitmap;
        }

        /// <summary>
        /// Calls <seealso cref="Task.Run(Action, CancellationToken)"/> for <seealso cref="Shot"/>.
        /// </summary>
        /// <returns></returns>
        public static async Task<Bitmap> ShotAsync(
            Rectangle? rectangle = null, 
            CancellationToken cancellationToken = default)
        {
            return await Task.Run(() => Shot(rectangle), cancellationToken)
                .ConfigureAwait(false);
        }

        #endregion
    }
}

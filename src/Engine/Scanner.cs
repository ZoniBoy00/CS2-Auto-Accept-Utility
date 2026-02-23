using System;
using System.Drawing;
using System.Drawing.Imaging;
using CS2AutoAccept.Infrastructure;
using CS2AutoAccept.Core;
using CS2AutoAccept.Services.Scanning;
using CS2AutoAccept.Services.Input;

namespace CS2AutoAccept.Engine
{
    public sealed class Scanner
    {
        public bool ScanAndAccept(IntPtr hWnd)
        {
            if (!NativeMethods.GetWindowRect(hWnd, out NativeMethods.RECT rect)) 
                return false;

            if (rect.Width <= 640 || rect.Height <= 360) 
                return false;

            // Define search region based on normalized coordinates
            int scanX = rect.Left + (int)(rect.Width * Constants.SEARCH_X);
            int scanY = rect.Top + (int)(rect.Height * Constants.SEARCH_Y);
            int scanW = (int)(rect.Width * Constants.SEARCH_W);
            int scanH = (int)(rect.Height * Constants.SEARCH_H);

            using var capture = CaptureScreenRegion(scanX, scanY, scanW, scanH);
            if (capture == null) return false;

            // Delegate detection to PixelEngine and movement to MouseHumanizer
            if (PixelEngine.FindButtonCentroid(capture, out int localX, out int localY))
            {
                MouseHumanizer.MoveAndClick(scanX + localX, scanY + localY);
                return true;
            }

            return false;
        }

        private static Bitmap? CaptureScreenRegion(int x, int y, int w, int h)
        {
            try
            {
                var bmp = new Bitmap(w, h, PixelFormat.Format32bppArgb);
                using var g = Graphics.FromImage(bmp);
                g.CopyFromScreen(x, y, 0, 0, new Size(w, h), CopyPixelOperation.SourceCopy);
                return bmp;
            }
            catch { return null; }
        }
    }
}

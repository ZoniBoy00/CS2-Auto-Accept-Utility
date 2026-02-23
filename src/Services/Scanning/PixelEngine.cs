using System;
using System.Drawing;
using System.Drawing.Imaging;
using CS2AutoAccept.Core;

namespace CS2AutoAccept.Services.Scanning
{
    public static class PixelEngine
    {
        /// <summary>
        /// Analyzes a bitmap using LockBits to find the center (centroid) of matching pixels.
        /// </summary>
        public static unsafe bool FindButtonCentroid(Bitmap bmp, out int centerX, out int centerY)
        {
            centerX = centerY = 0;
            
            BitmapData data = bmp.LockBits(
                new Rectangle(0, 0, bmp.Width, bmp.Height), 
                ImageLockMode.ReadOnly, 
                PixelFormat.Format32bppArgb);
            
            try
            {
                byte* scan0 = (byte*)data.Scan0;
                int stride = data.Stride;

                long sumX = 0;
                long sumY = 0;
                int matches = 0;

                // Scan every 4th pixel for speed/efficiency
                const int step = 4;

                for (int y = 0; y < bmp.Height; y += step)
                {
                    byte* row = scan0 + (y * stride);
                    for (int x = 0; x < bmp.Width; x += step)
                    {
                        // Internal BGRA format
                        byte b = row[x * 4];
                        byte g = row[x * 4 + 1];
                        byte r = row[x * 4 + 2];

                        if (IsColorMatch(r, g, b))
                        {
                            sumX += x;
                            sumY += y;
                            matches++;
                        }
                    }
                }

                if (matches >= Constants.MIN_PIXEL_THRESHOLD)
                {
                    centerX = (int)(sumX / matches);
                    centerY = (int)(sumY / matches);
                    return true;
                }
            }
            finally
            {
                bmp.UnlockBits(data);
            }

            return false;
        }

        private static bool IsColorMatch(byte r, byte g, byte b)
        {
            Color target = Constants.TargetGreen;
            return Math.Abs(r - target.R) <= Constants.COLOR_TOLERANCE && 
                   Math.Abs(g - target.G) <= Constants.COLOR_TOLERANCE && 
                   Math.Abs(b - target.B) <= Constants.COLOR_TOLERANCE;
        }
    }
}

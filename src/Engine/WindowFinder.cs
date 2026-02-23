using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CS2AutoAccept.Infrastructure;
using CS2AutoAccept.Core.Models;

namespace CS2AutoAccept.Engine
{
    public static class WindowFinder
    {
        public static GameWindow GetCS2Info()
        {
            try
            {
                using var proc = Process.GetProcessesByName("cs2").FirstOrDefault();
                
                if (proc == null || proc.MainWindowHandle == IntPtr.Zero)
                    return new GameWindow();

                IntPtr handle = proc.MainWindowHandle;
                
                if (NativeMethods.GetWindowRect(handle, out NativeMethods.RECT rect))
                {
                    var bounds = new Rectangle(rect.Left, rect.Top, rect.Width, rect.Height);
                    var screen = Screen.FromHandle(handle);
                    
                    return new GameWindow
                    {
                        Handle = handle,
                        Bounds = bounds,
                        ScreenName = screen.DeviceName.Replace("\\\\.\\", ""),
                        AspectRatio = GetAspectRatioLabel(bounds.Width, bounds.Height)
                    };
                }
            }
            catch { /* Process might have closed */ }

            return new GameWindow();
        }

        private static string GetAspectRatioLabel(int width, int height)
        {
            if (width <= 0 || height <= 0) return "N/A";

            int gcd = CalculateGCD(width, height);
            int x = width / gcd;
            int y = height / gcd;
            
            return (x, y) switch
            {
                (16, 9)  => "16:9",
                (16, 10) => "16:10",
                (8, 5)   => "16:10",
                (4, 3)   => "4:3",
                (5, 4)   => "5:4",
                _ => $"{x}:{y}"
            };
        }

        private static int CalculateGCD(int a, int b)
        {
            while (b != 0) { (a, b) = (b, a % b); }
            return a;
        }
    }
}

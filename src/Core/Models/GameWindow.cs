using System;
using System.Drawing;

namespace CS2AutoAccept.Core.Models
{
    public sealed class GameWindow
    {
        public IntPtr Handle { get; init; } = IntPtr.Zero;
        public bool IsRunning => Handle != IntPtr.Zero;
        public string ScreenName { get; init; } = "N/A";
        public Rectangle Bounds { get; init; } = Rectangle.Empty;
        public string Resolution => Bounds.Width > 0 ? $"{Bounds.Width}x{Bounds.Height}" : "N/A";
        public string AspectRatio { get; init; } = "N/A";
    }
}

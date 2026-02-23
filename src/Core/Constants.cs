using System.Drawing;

namespace CS2AutoAccept.Core
{
    public static class Constants
    {
        // --- DETECTION SETTINGS ---
        // Target: CS2 'Accept' button green RGB(54, 183, 82)
        public static readonly Color TargetGreen = Color.FromArgb(255, 54, 183, 82);
        
        // Scan Zone (Normalized percentages to support all resolutions)
        public const float SEARCH_X = 0.35f; 
        public const float SEARCH_Y = 0.35f; 
        public const float SEARCH_W = 0.30f; 
        public const float SEARCH_H = 0.20f; 

        // Matching Tolerance
        public const int COLOR_TOLERANCE = 15;
        public const int MIN_PIXEL_THRESHOLD = 8; // Stability check

        // --- TIMING / ENGINE ---
        public const int ENGINE_POLLING_MS = 500;
        public const int UI_REFRESH_MS = 100;
        public const int COOLDOWN_MS = 25000; // Delay after match accepted

        // --- MOUSE MOVEMENT ---
        public const int MIN_MOUSE_STEPS = 25;
        public const int MAX_MOUSE_STEPS = 45;
    }
}

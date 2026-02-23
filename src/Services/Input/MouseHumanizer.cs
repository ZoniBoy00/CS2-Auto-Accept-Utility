using System;
using System.Threading;
using CS2AutoAccept.Infrastructure;
using CS2AutoAccept.Core;

namespace CS2AutoAccept.Services.Input
{
    public static class MouseHumanizer
    {
        private static readonly Random _rng = new Random();

        /// <summary>
        /// Moves the cursor to the target using a Bezier curve with organic jitter and physics.
        /// </summary>
        public static void MoveAndClick(int targetX, int targetY)
        {
            NativeMethods.GetCursorPos(out NativeMethods.POINT currentPos);
            
            double startX = currentPos.X;
            double startY = currentPos.Y;

            // Randomize control point for organic curvature
            double ctrlX = (startX + targetX) / 2 + _rng.Next(-120, 121);
            double ctrlY = (startY + targetY) / 2 + _rng.Next(-120, 121);

            int steps = _rng.Next(Constants.MIN_MOUSE_STEPS, Constants.MAX_MOUSE_STEPS);
            
            for (int i = 1; i <= steps; i++)
            {
                double t = (double)i / steps;
                
                // Quadratic Bezier interpolation
                double nextX = Math.Pow(1 - t, 2) * startX + 2 * (1 - t) * t * ctrlX + Math.Pow(t, 2) * targetX;
                double nextY = Math.Pow(1 - t, 2) * startY + 2 * (1 - t) * t * ctrlY + Math.Pow(t, 2) * targetY;

                // Micro-jitter for realism
                nextX += _rng.NextDouble() * 1.6 - 0.8;
                nextY += _rng.NextDouble() * 1.6 - 0.8;

                NativeMethods.SetCursorPos((int)nextX, (int)nextY);
                
                // Velocity curve (human-like acceleration/deceleration)
                int delay = (i < steps * 0.15) ? _rng.Next(10, 18) : 
                            (i > steps * 0.85) ? _rng.Next(18, 28) : 
                            _rng.Next(4, 12);
                
                Thread.Sleep(delay);
            }

            // Final precision adjustment
            NativeMethods.SetCursorPos(targetX, targetY);
            Thread.Sleep(_rng.Next(180, 400)); 
            
            // Execute simulated physical click
            NativeMethods.mouse_event(NativeMethods.MOUSEEVENTF_LEFTDOWN, 0, 0, 0, UIntPtr.Zero);
            Thread.Sleep(_rng.Next(65, 135)); 
            NativeMethods.mouse_event(NativeMethods.MOUSEEVENTF_LEFTUP, 0, 0, 0, UIntPtr.Zero);
        }
    }
}

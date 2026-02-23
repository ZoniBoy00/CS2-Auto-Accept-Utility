using System;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;
using CS2AutoAccept.Core.Models;
using CS2AutoAccept.Core;

namespace CS2AutoAccept.UI
{
    public static class TUI
    {
        private static readonly object _lock = new();
        private static readonly List<string> _history = new();
        private const int MAX_LOGS = 6;

        private static readonly Process _proc = Process.GetCurrentProcess();
        private static DateTime _perfTimer = DateTime.UtcNow;
        private static TimeSpan _procTimer = TimeSpan.Zero;
        private static double _cpu = 0;

        // ANSI Styles
        private const string Reset = "\u001b[0m";
        private const string Cyan = "\u001b[36m";
        private const string Magenta = "\u001b[35m";
        private const string Green = "\u001b[32m";
        private const string Yellow = "\u001b[33m";
        private const string Red = "\u001b[31m";
        private const string White = "\u001b[37m";
        private const string Bold = "\u001b[1m";
        private const string Dim = "\u001b[2m";

        public static void Initialize()
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.Title = "CS2 Auto-Accept Utility v1.0.0";
            try 
            { 
               Console.SetWindowSize(90, 30);
               Console.SetBufferSize(90, 30);
               Console.CursorVisible = false; 
            } catch { }
            
            RedrawEverything();
        }

        public static void UpdateDisplay(GameWindow game, int successes, string action, int tick)
        {
            RefreshStats();

            lock (_lock)
            {
                // 1. Update Spinner/Status at a safe spot
                string[] frames = { "⠋", "⠙", "⠹", "⠸", "⠼", "⠴", "⠦", "⠧", "⠇", "⠏" };
                Print(4, 5, $"{Magenta}{frames[tick % frames.Length]}{Reset} {Bold}ENGINE STATUS: RUNNING{Reset}");

                // 2. Update Metrics
                Print(7, 8, $"{Cyan}SUCCESSES:{Reset} {successes.ToString().PadRight(10)}");
                Print(7, 9, $"{Cyan}CPU LOAD: {Reset} {_cpu:F1}%".PadRight(15));
                Print(7, 10, $"{Cyan}MEMORY:   {Reset} {(_proc.WorkingSet64 / 1024.0 / 1024.0):F1} MB".PadRight(18));

                // 3. Update Game Info
                string status = game.IsRunning ? $"{Green}ONLINE{Reset}" : $"{Red}OFFLINE{Reset}";
                Print(46, 8, $"{Cyan}PROCESS:  {Reset} {status.PadRight(15)}");
                Print(46, 9, $"{Cyan}DISPLAY:  {Reset} {game.ScreenName.PadRight(20)}");
                Print(46, 10, $"{Cyan}RES:      {Reset} {game.Resolution.PadRight(15)}");
                Print(46, 11, $"{Cyan}ASPECT:   {Reset} {game.AspectRatio.PadRight(10)}");

                // 4. Update Action Bar
                Print(21, 14, $"{action.PadRight(55)}");

                // 5. Update Log Area
                for (int i = 0; i < MAX_LOGS; i++)
                {
                    string msg = (i < _history.Count) ? _history[_history.Count - 1 - i] : "";
                    Print(7, 19 + i, $"{msg.PadRight(75)}");
                }
            }
        }

        public static void AddLog(string msg)
        {
            lock (_lock)
            {
                _history.Add($"[{DateTime.Now:HH:mm:ss}] {msg}");
                if (_history.Count > MAX_LOGS) _history.RemoveAt(0);
            }
        }

        private static void RefreshStats()
        {
            var now = DateTime.UtcNow;
            if ((now - _perfTimer).TotalMilliseconds > 500)
            {
                var currentTotal = _proc.TotalProcessorTime;
                _cpu = ((currentTotal - _procTimer).TotalMilliseconds / (Environment.ProcessorCount * (now - _perfTimer).TotalMilliseconds)) * 100;
                _perfTimer = now;
                _procTimer = currentTotal;
            }
        }

        private static void RedrawEverything()
        {
            Console.Clear();
            
            // Outer Frame
            DrawBox(1, 1, 88, 28, Dim, "");

            // Top Header - Banner centered and moved up
            Print(25, 2, $"{Magenta}{Bold}╔═╗╔═╗  ╔═╗╦ ╦╔╦╗╔═╗  ╔═╗╔═╗╔═╗╔═╗╔═╗╔╦╗{Reset}");
            Print(25, 3, $"{Magenta}{Bold}║  ╚═╗  ╠═╣║ ║ ║ ║ ║  ╠═╣║  ║  ║╣ ╠═╝ ║ {Reset}");
            Print(25, 4, $"{Magenta}{Bold}╚═╝╚═╝  ╩ ╩╚═╝ ╩ ╚═╝  ╩ ╩╚═╝╚═╝╚═╝╩   ╩ {Reset}");

            // Engine & Game Sections
            DrawBox(4, 7, 38, 6, Cyan, " SYSTEM METRICS ");
            DrawBox(43, 7, 42, 6, Cyan, " GAME INSTANCE ");

            // Action Bar
            DrawBox(4, 13, 81, 3, Yellow, " CURRENT ACTION ");
            Print(6, 14, $"{Bold}STATUS >{Reset}");

            // Log Area
            DrawBox(4, 17, 81, 9, Dim, " EVENT LOG ");

            // Footer
            Print(4, 26, $"{Dim}[CTRL+C] TO QUIT{Reset}");
            Print(72, 26, $"{Dim}v1.0.0-STABLE{Reset}");
        }

        private static void DrawBox(int x, int y, int w, int h, string color, string title)
        {
            // Top border with title
            Console.SetCursorPosition(x, y);
            Console.Write(color + "╔═");
            if (!string.IsNullOrEmpty(title))
            {
                Console.Write(Reset + Bold + title + Reset + color);
                Console.Write(new string('═', w - 4 - title.Length));
            }
            else
            {
                Console.Write(new string('═', w - 4));
            }
            Console.Write("═╗" + Reset);

            // Sides
            for (int i = 1; i < h - 1; i++)
            {
                Print(x, y + i, $"{color}║{new string(' ', w - 2)}║{Reset}");
            }

            // Bottom
            Print(x, y + h - 1, $"{color}╚{new string('═', w - 2)}╝{Reset}");
        }

        private static void Print(int x, int y, string text)
        {
            Console.SetCursorPosition(x, y);
            Console.Write(text);
        }

        public static void Shutdown()
        {
            Console.Clear();
            Console.CursorVisible = true;
            Console.WriteLine($"{Yellow}Finalizing tasks and shutting down...{Reset}");
        }
    }
}

using System;
using System.Threading;
using System.Threading.Tasks;
using CS2AutoAccept.Engine;
using CS2AutoAccept.UI;
using CS2AutoAccept.Core;
using CS2AutoAccept.Core.Models;

namespace CS2AutoAccept
{
    public static class Program
    {
        private static int _successCount;
        private static string _statusText = "Initializing...";
        private static GameWindow _activeWindow = new GameWindow();
        private static readonly Scanner _scanner = new Scanner();

        public static async Task Main()
        {
            TUI.Initialize();

            using var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (_, e) => { e.Cancel = true; cts.Cancel(); };

            // Logic monitoring task
            var engineTask = RunEngineLoop(cts.Token);

            // UI render task
            try
            {
                int frame = 0;
                while (!cts.Token.IsCancellationRequested)
                {
                    TUI.UpdateDisplay(_activeWindow, _successCount, _statusText, frame++);
                    await Task.Delay(Constants.UI_REFRESH_MS, cts.Token);
                }
            }
            catch (OperationCanceledException) { }

            await engineTask;
            TUI.Shutdown();
        }

        private static async Task RunEngineLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    _activeWindow = WindowFinder.GetCS2Info();

                    if (_activeWindow.IsRunning)
                    {
                        _statusText = "\u001b[36mMonitoring Match Invites...\u001b[0m";
                        if (_scanner.ScanAndAccept(_activeWindow.Handle))
                        {
                            Interlocked.Increment(ref _successCount);
                            TUI.AddLog("Match Accepted!");
                            
                            // Prevent spam during loading screen
                            await Task.Delay(Constants.COOLDOWN_MS, token);
                        }
                    }
                    else
                    {
                        _statusText = "\u001b[33mSearching for CS2 Process...\u001b[0m";
                    }

                    await Task.Delay(Constants.ENGINE_POLLING_MS, token);
                }
                catch (OperationCanceledException) { break; }
                catch (Exception ex) 
                { 
                    TUI.AddLog($"System Error: {ex.Message}");
                    await Task.Delay(2000, token);
                }
            }
        }
    }
}

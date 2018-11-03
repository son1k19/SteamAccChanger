using System;
using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using Newtonsoft.Json;

namespace SteamAccChanger
{
    internal class Utils
    {
        public static int AccountsRequestedToday { get; set; } = -1;

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int cmdShow);

        public static void MaximizeConsole()
        {
            ShowWindow(Process.GetCurrentProcess().MainWindowHandle, 3);
        }

        public static SteamGeneratorResponse GetNewAccount()
        {
            if (AccountsRequestedToday == -1)
            {
                ConsEx.Write("Please wait for API limitations update..\n", ConsoleColor.Red);
                return null;
            }
            if (AccountsRequestedToday == 30)
            {
                ConsEx.Write(
                    "You requested 30 accounts today. That's an API limitation. You can't request more.\n",
                    ConsoleColor.Red);
                return null;
            }

            string sResp;

            try
            {
                sResp = new WebClient().DownloadString("http://accgen.jdevcloud.com/steam/api");

            }
            catch
            {
                return null;
            }

            var steamGeneratorResponse = JsonConvert.DeserializeObject<SteamGeneratorResponse>(sResp);

            UpdateAPILimitations();

            return steamGeneratorResponse.Success == 0 ? null : steamGeneratorResponse;
        }

        public static void LoginToSteam(SteamGeneratorResponse account)
        {
            ConsEx.Write("Waiting for Steam.. ", ConsoleColor.Yellow);

            SpinWait.SpinUntil(() => Process.GetProcessesByName("steam").Length != 0);

            ConsEx.WriteRaw("found!\n", ConsoleColor.Green);

            Thread.Sleep(1000); // Steam really loves subprocesses.

            var steamProcesses = Process.GetProcessesByName("steam");
            var steamProcessPath = steamProcesses[0].MainModule.FileName;

            foreach (var steamProcess in steamProcesses)
            {
                using (steamProcess)
                {
                    steamProcess.Kill();
                }
            }

            Process.Start(new ProcessStartInfo
            {
                FileName = steamProcessPath,
                Arguments = $"-login {account.Username} {account.Password} " +
                            $"{(Program.Settings.GetConfigEntry<bool>("should_auto_start_game") ? $"-applaunch {Program.Settings.GetConfigEntry<long>("auto_start_gameid")}" : "")}" +
                            $"{(Program.Settings.GetConfigEntry<bool>("speed_up_steam") ? "-noverifyfiles" : "")}" +
                            $"{(Program.Settings.GetConfigEntry<bool>("silent_steam_launch") ? "-silent" : "")}",
                UseShellExecute = false
            });
        }

        public static void UpdateAPILimitations(bool returnOnFail = false)
        {
            int result;
            var wResp = string.Empty;
            
            try
            {
                wResp = new WebClient().DownloadString("http://accgen.jdevcloud.com/steam/api?stats=1");
                result = int.Parse(wResp);
            }
            catch
            {
                // Verification pending?

                SteamGeneratorResponse sGResp = null;

                try
                {
                    sGResp = JsonConvert.DeserializeObject<SteamGeneratorResponse>(wResp);
                }
                catch
                {
                    if (returnOnFail) return;

                    ReportFatalError(false);
                }

                if (sGResp == null)
                {
                    if (returnOnFail) return;

                    ReportFatalError(false);
                }

                if (sGResp.Success == -1)
                {
                    if (returnOnFail) return;

                    ReportFatalError(false);
                }

                if (sGResp.Success == 0)
                {
                    if (returnOnFail) return;

                    if (sGResp.ErrorMessage.ToLowerInvariant().Contains("verification"))
                    {
                        ConsEx.WriteRaw("\n\n");
                        ConsEx.Write("Hey. You need to get through captcha verification.\n");
                        ConsEx.Write("I'll open a website for you..\n");

                        Process.Start("http://accgen.undo.it/");

                        ConsEx.Write("Check console title for more info..");

                        while (true)
                        {
                            for (var i = 10; i >= 0; i--)
                            {
                                Console.Title = $"Steam Account Switcher ({i} seconds until re-checking..)";
                                Thread.Sleep(1000);
                            }

                            UpdateAPILimitations(true);

                            if (AccountsRequestedToday == -1) continue;

                            // Console is kinda broken right now. Why fix?
                            // ez

                            Process.Start(Environment.GetCommandLineArgs()[0]);
                            Environment.Exit(0);
                        }
                    }

                    ReportFatalError(true, sGResp.ErrorMessage);
                }

                return;
            }

            Console.Title = $"Steam Account Switcher ({AccountsPerDay-result} accs left)";
            AccountsRequestedToday = result;
        }

        public static void ReportFatalError(bool known, string msg = null)
        {
            if (known)
            {
                ConsEx.WriteRaw("\n\n");

                ConsEx.Write("Unknown server sided error has occurred.\n", ConsoleColor.Red);
                ConsEx.WriteRaw("\t[ ", ConsoleColor.Cyan);
                ConsEx.WriteRaw(msg, ConsoleColor.Red);
                ConsEx.WriteRaw(" ]\n\n", ConsoleColor.Cyan);
            }
            else
            {
                ConsEx.Write("Some unhandled internal error has occurred. Please create an issue on GitHub.\n", ConsoleColor.Red);
                ConsEx.Write("https://github.com/0x25CBFC4F/SteamAccChanger/issues/\n\n", ConsoleColor.Red);
                ConsEx.Write($"{new StackTrace()}", ConsoleColor.Red);
            }

            SpinWait.SpinUntil(() => false); //nope. not gettin' outta 'ere.
        }

        /*
         * Due to recent actions taken by steam to prevent account generations,
         * a daily limit of generating 12 accounts here (12accs/24hours) is being imposed until a workaround is found.
         * Thanks for understanding.
         */

        public static int AccountsPerDay => 12;
    }
}

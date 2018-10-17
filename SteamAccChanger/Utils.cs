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

        public static void UpdateAPILimitations()
        {
            int result;

            try
            {
                result = int.Parse(new WebClient().DownloadString("http://accgen.jdevcloud.com/steam/api?stats=1"));
            }
            catch
            {
                return;
            }

            Console.Title = $"Steam Account Switcher ({AccountsPerDay-result} accs left)";
            AccountsRequestedToday = result;
        }

        public static int AccountsPerDay => 30;
    }
}

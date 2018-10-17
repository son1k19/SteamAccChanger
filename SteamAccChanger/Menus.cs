using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace SteamAccChanger
{
    internal class Menus
    {
        public static readonly List<Tuple<string, Func<bool>>> MainMenu = new List<Tuple<string, Func<bool>>>
        {
            new Tuple<string, Func<bool>>("Get new account", Program.CreateNewAccountMenu),
            new Tuple<string, Func<bool>>("Settings", Program.AutoGameStartMenu),
            new Tuple<string, Func<bool>>("Exit application", () => false)
        };

        public static readonly List<Tuple<Func<Tuple<string, ConsoleColor>>, Func<bool>>> SettingsDynamicMenu = new List<Tuple<Func<Tuple<string, ConsoleColor>>, Func<bool>>>
        {
            new Tuple<Func<Tuple<string, ConsoleColor>>, Func<bool>>(
                () =>
                {
                    var s = Program.Settings.GetConfigEntry<bool>("should_auto_start_game");

                    return Tuple.Create("Automaticly start game after re-login? " +
                                        $"[{(s ? "YES" : "NO")}]",
                                        s ? ConsoleColor.Green : ConsoleColor.Yellow);
                },
                () =>
                {
                    var s = Program.Settings.GetConfigEntry<bool>("should_auto_start_game");
                    Program.Settings.SetConfigEntry("should_auto_start_game", !s);
                    Program.Settings.SaveConfig();

                    return true;
                }),
            new Tuple<Func<Tuple<string, ConsoleColor>>, Func<bool>>(
                () =>
                {
                    var s = Program.Settings.GetConfigEntry<long>("auto_start_gameid");

                    return Tuple.Create($"Steam game ID: [{s}]", ConsoleColor.Green);
                },
                () =>
                {
                    ConsEx.WriteRaw("\n\n");
                    ConsEx.Write("> Enter game's steam ID: ", ConsoleColor.Cyan);
                    var r = Console.ReadLine();

                    long steamGameId; //out var is not possible due to language restrictions
                    var pResult = long.TryParse(r, out steamGameId);

                    if (!pResult)
                    {
                        ConsEx.WriteRaw("\n");
                        ConsEx.Write("Bro you sure you entered a number? ", ConsoleColor.Red);

                        Thread.Sleep(2000);

                        return true;
                    }

                    if (steamGameId < 0)
                    {
                        ConsEx.WriteRaw("\n");
                        ConsEx.Write("Steam game ID shouldn't be lower than zero, you know?.. ", ConsoleColor.Red);

                        Thread.Sleep(2000);

                        return true;
                    }

                    Program.Settings.SetConfigEntry("auto_start_gameid", steamGameId);
                    Program.Settings.SaveConfig();

                    return true;
                }),
            new Tuple<Func<Tuple<string, ConsoleColor>>, Func<bool>>(
                () =>
                {
                    var s = Program.Settings.GetConfigEntry<bool>("speed_up_steam");

                    return Tuple.Create("Speed up Steam loading? " +
                                        $"[{(s ? "YES" : "NO")}]",
                                        s ? ConsoleColor.Green : ConsoleColor.Yellow);
                },
                () =>
                {
                    var s = Program.Settings.GetConfigEntry<bool>("speed_up_steam");
                    Program.Settings.SetConfigEntry("speed_up_steam", !s);
                    Program.Settings.SaveConfig();

                    return true;
                }),
            new Tuple<Func<Tuple<string, ConsoleColor>>, Func<bool>>(
                () =>
                {
                    var s = Program.Settings.GetConfigEntry<bool>("silent_steam_launch");

                    return Tuple.Create("Start Steam silently? " +
                                        $"[{(s ? "YES" : "NO")}]",
                        s ? ConsoleColor.Green : ConsoleColor.Yellow);
                },
                () =>
                {
                    var s = Program.Settings.GetConfigEntry<bool>("silent_steam_launch");
                    Program.Settings.SetConfigEntry("silent_steam_launch", !s);
                    Program.Settings.SaveConfig();

                    return true;
                }),
            new Tuple<Func<Tuple<string, ConsoleColor>>, Func<bool>>(
                () =>
                {
                    var s = Program.Settings.GetConfigEntry<bool>("maximize_console_on_start");

                    return Tuple.Create("Maximize console on start? " +
                                        $"[{(s ? "YES" : "NO")}]",
                        s ? ConsoleColor.Green : ConsoleColor.Yellow);
                },
                () =>
                {
                    var s = Program.Settings.GetConfigEntry<bool>("maximize_console_on_start");
                    Program.Settings.SetConfigEntry("maximize_console_on_start", !s);
                    Program.Settings.SaveConfig();

                    return true;
                }),
            new Tuple<Func<Tuple<string, ConsoleColor>>, Func<bool>>(
                () => Tuple.Create("Go back", ConsoleColor.Yellow),
                () => false)
        };

        public static readonly List<Tuple<string, Func<bool>>> CreateNewAccountMenu =
            new List<Tuple<string, Func<bool>>>
            {
                new Tuple<string, Func<bool>>("Login me to a throwaway account", () =>
                {
                    ConsEx.WriteRaw("\n\n");

                    ConsEx.Write("Requesting new account..\n");

                    var r = Utils.GetNewAccount();

                    if (r == null)
                    {
                        ConsEx.Write("Account request failed.\n", ConsoleColor.Red);

                        Thread.Sleep(3000);

                        return true;
                    }

                    Utils.LoginToSteam(r);
                    return true;
                }),
                new Tuple<string, Func<bool>>("Login me & save account", () =>
                {
                    ConsEx.WriteRaw("\n\n");

                    ConsEx.Write("Requesting new account..\n");

                    var r = Utils.GetNewAccount();

                    if (r == null)
                    {
                        ConsEx.Write("Account request failed.\n", ConsoleColor.Red);

                        Thread.Sleep(3000);

                        return true;
                    }

                    Utils.LoginToSteam(r);

                    var fileSaveDialog = new SaveFileDialog()
                    {
                        FileName = $"{r.Username.ToLower()}_steam.txt",
                        Filter = "Whatever..?|*.*"
                    };

                    if (fileSaveDialog.ShowDialog() != DialogResult.OK)
                    {
                        return true;
                    }

                    File.WriteAllText(fileSaveDialog.FileName, 
                        $"Username: {r.Username}\r\n" +
                        $"Password: {r.Password}\r\n" +
                        $"E-Mail: {r.EMail}");

                    return true;
                }),
                new Tuple<string, Func<bool>>("Login me & copy to clipboard", () =>
                {
                    ConsEx.WriteRaw("\n\n");

                    ConsEx.Write("Requesting new account..\n");

                    var r = Utils.GetNewAccount();

                    if (r == null)
                    {
                        ConsEx.Write("Account request failed.\n", ConsoleColor.Red);

                        Thread.Sleep(3000);

                        return true;
                    }

                    Utils.LoginToSteam(r);

                    Clipboard.SetText($"Username: {r.Username}\r\n" +
                                      $"Password: {r.Password}\r\n" +
                                      $"E-Mail: {r.EMail}");

                    return true;
                }),
                new Tuple<string, Func<bool>>("Go back", () => false)
            };
    }
}

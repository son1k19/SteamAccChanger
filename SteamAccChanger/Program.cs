using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ConfigurationManager;

/*
 * This whole code is antipasta itself. (C)
 */

namespace SteamAccChanger
{
    internal class Program
    {
        public static readonly ApplicationSettings Settings = new ApplicationSettings(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)  + "\\steam_acc_changer.json");
        
        [STAThread]
        private static void Main()
        {
            Settings.LoadConfigCreateIfNeeded(new Dictionary<string, object>
            {
                { "should_auto_start_game", false },
                { "auto_start_gameid", 0L },
                { "speed_up_steam", false },
                { "silent_steam_launch", true },
                { "maximize_console_on_start", true }
            });

            SetupConsole();

            Console.Clear();
            ConsEx.DrawLogo();

            ConsEx.HandleMenu(Menus.MainMenu);
        }

        public static void SetupConsole()
        {
            Console.Title = "Steam Account Switcher (Updating..)";
            Console.ForegroundColor = ConsoleColor.White;

            if (Settings.GetConfigEntry<bool>("maximize_console_on_start"))
            {
                Utils.MaximizeConsole();
            }

            Task.Run(() => Utils.UpdateAPILimitations());
        }

        public static bool AutoGameStartMenu()
        {
            ConsEx.WriteRaw("\n\n");
            ConsEx.Write("[ Settings ]\n\n\n");

            ConsEx.HandleDynamicMenu(Menus.SettingsDynamicMenu);

            return true;
        }

        public static bool CreateNewAccountMenu()
        {
            ConsEx.WriteRaw("\n\n");
            ConsEx.Write("[ Get new account ]\n\n\n");

            ConsEx.HandleMenu(Menus.CreateNewAccountMenu);

            return true;
        }
    }
}

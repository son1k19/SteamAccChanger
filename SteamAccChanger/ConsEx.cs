using System;
using System.Collections.Generic;

namespace SteamAccChanger
{
    internal class ConsEx
    {
        public static void DrawLogo()
        {
            WriteRaw("\n\n");
            Write("\t[ Steam Account Switcher ]\n", ConsoleColor.Cyan);
            Write("\t\t~ Welcome! ~\n\n");
        }

        public static void HandleMenu(IList<Tuple<string, Func<bool>>> menu)
        {
            while (true)
            {
                Console.Clear();
                DrawLogo();

                WriteRaw("\n\n");

                var menuIndex = 0;

                foreach (var menuEntry in menu)
                {
                    menuIndex++;

                    Write("[", ConsoleColor.Blue);
                    WriteRaw($"{menuIndex}");
                    WriteRaw("]", ConsoleColor.Blue);

                    WriteRaw($" > {menuEntry.Item1} <\n", ConsoleColor.Green);
                }

                WriteRaw("\n\n");

                var userSelection = ReadUserInput(menu.Count);
            
                var r = menu[userSelection - 1]?.Item2?.Invoke();

                if (r == false) // Can't compare like a normal bool because of Nullable<>
                {
                    break;
                }
            }
        }

        public static void HandleDynamicMenu(IList<Tuple<Func<Tuple<string, ConsoleColor>>, Func<bool>>> menu)
        {
            while (true)
            {
                Console.Clear();
                DrawLogo();

                WriteRaw("\n\n");

                var menuIndex = 0;

                foreach (var menuEntry in menu)
                {
                    menuIndex++;

                    var r = menuEntry.Item1?.Invoke();

                    if (r == null)
                    {
                        Write(" >> MENU ITEM IS INVALID <<\n");
                        continue;
                    }

                    Write("[", ConsoleColor.Blue);
                    WriteRaw($"{menuIndex}");
                    WriteRaw("]", ConsoleColor.Blue);

                    WriteRaw($" > {r.Item1} <\n", r.Item2);
                }

                WriteRaw("\n\n");

                var userSelection = ReadUserInput(menu.Count);
            
                var res = menu[userSelection - 1]?.Item2?.Invoke();

                if (res == false)
                {
                    break;
                }
            }
        }

        public static void Write(string text, ConsoleColor color = ConsoleColor.White)
        {
            WriteRaw($"\t\t{text}", color);
        }

        public static void WriteRaw(string text, ConsoleColor color = ConsoleColor.White)
        {
            var prevColor = Console.ForegroundColor;
            Console.ForegroundColor = color;

            Console.Write(text);

            Console.ForegroundColor = prevColor;
        }

        private static int ReadUserInput(int maxValue = -1)
        {
            Write("> [ ]", ConsoleColor.Blue);
            WriteRaw("\b\b");

            while (true)
            {
                var input = Console.ReadKey(true).KeyChar;

                if (!char.IsNumber(input))
                {
                    continue;
                }

                var userValue = int.Parse(input.ToString());

                if (userValue < 1)
                {
                    continue;
                }

                if (maxValue > -1)
                {
                    if (userValue > maxValue)
                    {
                        continue;
                    }
                }

                Console.Write(input);
                return userValue;
            }
        }
    }
}

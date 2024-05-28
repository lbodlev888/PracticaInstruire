using System;
using System.Data.SQLite;
using System.IO;
using System.Linq;

namespace Practica
{
    internal class Program
    {
        public static string DATABASE = "Magazin.db";
        public static void printMessage(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.White;
        }
        public static int getOption(string menu, string errorMsg, int[] options = null)
        {
            Console.Write(menu);
            int option = 0;
            try
            {
                option = int.Parse(Console.ReadLine());
                if (!options.Contains(option))
                {
                    printMessage(errorMsg, ConsoleColor.Red);
                    return getOption(menu, errorMsg, options);
                }
                return option;
            }
            catch (FormatException)
            {
                printMessage(errorMsg, ConsoleColor.Red);
                return getOption(menu, errorMsg, options);
            }
        }
        public static string getPassword()
        {
            var pass = string.Empty;
            ConsoleKey key;
            do
            {
                var keyInfo = Console.ReadKey(intercept: true);
                key = keyInfo.Key;

                if (key == ConsoleKey.Backspace && pass.Length > 0)
                {
                    Console.Write("\b \b");
                    pass = pass.Remove(pass.Length - 1);
                }
                else if (!char.IsControl(keyInfo.KeyChar))
                {
                    Console.Write("*");
                    pass += keyInfo.KeyChar;
                }
            } while (key != ConsoleKey.Enter);
            Console.WriteLine();
            return pass;
        }
        static void adminWork(int option)
        {
            switch(option)
            {
                case 1:
                    new Magazin();
                    break;
                case 2:
                    Magazin.deleteShop();
                    break;
                case 3:
                    Magazin.updateMagazin();
                    break;
                case 4:
                    User.setprivilege();
                    break;
                case 5:
                    User.deleteUser();
                    break;
            }
        }
        static void moderatorWork(int option)
        {
            switch(option)
            {
                case 1:
                    new Piesa();
                    break;
                case 2:
                    Piesa.updatePart();
                    break;
                case 3:
                    Piesa.deletePart();
                    break;
            }
        }
        static void userWork(int option, User session)
        {
            switch(option)
            {
                case 1:
                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Magazin.getShops();
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine();
                    break;
                case 2:
                    int[] shops = Magazin.getShops().ToArray();
                    string message = "Ce magazin alegi?", errorMsg = "Nu exista asa magazin";
                    int selMag = getOption(message, errorMsg, shops);
                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Piesa.getParts(selMag);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine();
                    break;
                case 3:
                    new Comanda(session.getUser());
                    break;
            }
        }
        static void Main()
        {
            if (!File.Exists(DATABASE)) SQLiteConnection.CreateFile(DATABASE);
            int[] options = { 1, 2, 3 };
            string intro = "\tBine ai venit\n1 - Creare cont nou\n2 - Logare cu un cont existent\n3 - Iesire\nCe doresti sa faci? ";
            string optionError = "\nEroare: Nu exista o astfel de optiune\n";
            int option = getOption(intro, optionError, options);
            switch (option)
            {
                case 1:
                    new User(true);
                    break;
                case 2:
                    Console.Write("Nume de utilizator: ");
                    string username = Console.ReadLine();
                    Console.Write("Parola: ");
                    string password = getPassword();
                    User session = User.checkPassword(username, password);
                    if (session == null)
                    {
                        Main();
                        return;
                    }
                    printMessage(session.ToString(), ConsoleColor.Yellow);
                    string message = "";
                    switch(session.type)
                    {
                        case 0:
                            message = "1 - Arata magazinele\n2 - Arata piesele unui magazin specific\n" +
                                "3 - Comanda o piesa\n4 - Meniul principal\nCe doresti sa faci? ";
                            options = new int[] { 1, 2, 3, 4 };
                            option = getOption(message, optionError, options);
                            if(option == 4)
                            {
                                session = null;
                                Main();
                                return;
                            }
                            userWork(option, session);
                            break;
                        case 1:
                            message = "1 - Adauga o piesa\n2 - Modifica o piesa existenta\n" +
                                "3 - Sterge o piesa existenta\n4 - Meniul principal\nCe doresti sa faci? ";
                            options = new int[] { 1, 2, 3, 4 };
                            option = getOption(message, optionError, options);
                            if(option == 6)
                            {
                                session = null;
                                Main();
                                return;
                            }
                            moderatorWork(option);
                            break;
                        case 2:
                            message = "1 - Adauga magazin\n2 - Sterge magazin\n3 - Modifica magazin\n4 - Privilegiaza utilizatorul\n5 - Sterge utilizatorul\n6 - Log out\nCe doresti sa faci? ";
                            options = new int[] { 1, 2, 3, 4, 5, 6 };
                            option = getOption(message, optionError, options);
                            if(option == 6)
                            {
                                session = null;
                                Main();
                                return;
                            }
                            adminWork(option);
                            break;
                    }
                    break;
                case 3:
                    return;
            }
            Main();
        }
    }
}
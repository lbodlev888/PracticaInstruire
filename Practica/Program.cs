﻿using System;
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
                case 6:
                    string[] tables = { "UsersType", "Users", "Piesa", "Magazin", "Comenzi" };
                    DatabaseHelper.exportDatabase(tables);
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
                    int[] options = Enumerable.Range(1, shops.Length).ToArray();
                    string message = "Ce magazin alegi? ", errorMsg = "Nu exista asa magazin";
                    int selMag = shops[getOption(message, errorMsg, options)-1];
                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    if (Piesa.getParts(selMag).Count == 0)
                        printMessage("Magazinul selectat nu are nici o piesa", ConsoleColor.Yellow);
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
            //DatabaseHelper.importDatabase();
            if (!File.Exists(DATABASE))
                SQLiteConnection.CreateFile(DATABASE);
            int[] options = { 1, 2, 3, 4 };
            string intro = "\tBine ai venit\n1 - Creare cont nou\n2 - Logare cu un cont existent\n3 - Importa baza de date\n4 - Iesire\nCe doresti sa faci? ";
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
                    while (true)
                    {
                        switch (session.type)
                        {
                            case 0:
                                message = "1 - Arata magazinele\n2 - Arata piesele unui magazin specific\n" +
                                    "3 - Comanda o piesa\n4 - Meniul principal\nCe doresti sa faci? ";
                                options = new int[] { 1, 2, 3, 4 };
                                option = getOption(message, optionError, options);
                                if (option == 4)
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
                                if (option == 4)
                                {
                                    session = null;
                                    Main();
                                    return;
                                }
                                moderatorWork(option);
                                break;
                            case 2:
                                message = "1 - Adauga magazin\n2 - Sterge magazin\n3 - Modifica magazin\n" +
                                    "4 - Privilegiaza utilizatorul\n5 - Sterge utilizatorul\n" +
                                    "6 - Exporteaza baza de date in fisiere CSV\n7 - Meniul principal\nCe doresti sa faci? ";
                                options = new int[] { 1, 2, 3, 4, 5, 6, 7};
                                option = getOption(message, optionError, options);
                                if (option == 7)
                                {
                                    session = null;
                                    Main();
                                    return;
                                }
                                adminWork(option);
                                break;
                        }
                    }
                case 3:
                    string[] tables = { "UsersType", "Users", "Piesa", "Magazin", "Comenzi" };
                    DatabaseHelper.importDatabase(tables);
                    break;
                case 4:
                    return;
            }
            Main();
        }
    }
}
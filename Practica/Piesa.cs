using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;

namespace Practica
{
    internal class Piesa
    {
        private string Nume;
        private double Pret;
        private uint Cantitate;
        private int idMagazin;
        private string NumeMagazin;
        public Piesa(bool create=true)
        {
            Console.Write("Numele piesei: ");
            Nume = Console.ReadLine();
            Console.Write("Pretul piesei: ");
            Pret = double.Parse(Console.ReadLine());
            Console.Write("Cantitatea: ");
            Cantitate = uint.Parse(Console.ReadLine());
            int[] shops = Magazin.getShops().ToArray();
            int[] options = Enumerable.Range(1, shops.Length).ToArray();
            string message = "Ce magazin alegi? ", errorMesg = "Nu exista asa magazin";
            int option = shops[Program.getOption(message, errorMesg, options)-1];
            idMagazin = option;
            DatabaseHelper db = new DatabaseHelper(Program.DATABASE);
            SQLiteDataReader reader = db.getReader("SELECT Nume FROM Magazin WHERE idMagazin=" + option);
            reader.Read();
            NumeMagazin = reader["Nume"].ToString();
            reader.Close();
            db.closeConnection();
            if(create)
            {
                db.openConnection();
                db.Query($"INSERT INTO Piesa (Nume, Pret, Cantitate, idMagazin) VALUES ('{Nume}', {Pret}, {Cantitate}, {option})");
                db.closeConnection();
                Program.printMessage("Piesa adaugata cu succes", ConsoleColor.Green);
            }
        }
        /*private void createPart()
        {
            DatabaseHelper db = new DatabaseHelper(Program.DATABASE);
            db.Query($"INSERT INTO Produs (Nume, Pret, Cantitate, idMagazin) VALUES ('{Nume}', '{Pret}', '{Cantitate}', '{idMagazin}')");
            db.closeConnection();
        }*/
        public static void deletePart()
        {
            int[] shops = Magazin.getShops().ToArray();
            string message = "Din ce magazin doresti sa stergi? ", errorMsg = "Nu exista asa magazin";
            int option = Program.getOption(message, errorMsg, shops);
            int[] parts = getParts(option).ToArray();
            if (parts.Length == 0)
            {
                Program.printMessage("Magazinul selectat nu are nici o piesa", ConsoleColor.Red);
                return;
            }
            message = "Ce piesa doresti sa stergi? "; errorMsg = "Piesa nu exista";
            option = Program.getOption(message, errorMsg, parts);
            DatabaseHelper db = new DatabaseHelper(Program.DATABASE);
            db.Query("DELETE FROM Piesa WHERE idPart=" + option);
            db.closeConnection();
            Program.printMessage("Piesa a fost stearsa cu succes", ConsoleColor.Green);
        }
        public static void updatePart()
        {
            int[] shops = Magazin.getShops().ToArray();
            string message = "Din ce magazin doresti sa stergi? ", errorMsg = "Nu exista asa magazin";
            int option = Program.getOption(message, errorMsg, shops);
            int[] parts = getParts(option).ToArray();
            if (parts.Length == 0)
            {
                Program.printMessage("Magazinul selectat nu are nici o piesa", ConsoleColor.Red);
                return;
            }
            message = "Ce piesa doresti sa modifici? "; errorMsg = "Piesa nu exista";
            option = Program.getOption(message, errorMsg, parts);
            Piesa part = new Piesa(false);
            DatabaseHelper db = new DatabaseHelper(Program.DATABASE);
            db.Query($"UPDATE Piesa SET Nume='{part.Nume}', Pret={part.Pret}, Cantitate={part.Cantitate}, idMagazin={part.idMagazin} WHERE idPart=" + option);
            db.closeConnection();
            Program.printMessage("Piesa a fost modificata cu succes", ConsoleColor.Green);
        }
        /*public static List<int> getParts()
        {
            DatabaseHelper db = new DatabaseHelper(Program.DATABASE);
            SQLiteDataReader reader = db.getReader("SELECT * FROM Piesa");
            List<int> ids = new List<int>();
            while (reader.Read())
            {
                Console.WriteLine($"{reader["idPart"]}) {reader["Nume"]} {reader["Pret"]} {reader["Cantitate"]}");
                ids.Add(Convert.ToInt32(reader["idPart"]));
            }
            reader.Close();
            db.closeConnection();
            return ids;
        }*/
        public static List<int> getParts(int idMagazin)
        {
            DatabaseHelper db = new DatabaseHelper(Program.DATABASE);
            SQLiteDataReader reader = db.getReader("SELECT * FROM Piesa WHERE idMagazin=" + idMagazin);
            List<int> ids = new List<int>();
            int count = 1;
            while (reader.Read())
            {
                Console.WriteLine($"{count}) {reader["Nume"]} {reader["Pret"]} {reader["Cantitate"]}");
                ids.Add(Convert.ToInt32(reader["idPart"]));
                count++;
            }
            reader.Close();
            db.closeConnection();
            return ids;
        }
        public override string ToString() => $"Nume: {Nume}\nPret: {Pret}\nCantitate: {Cantitate}\nMagazin: {NumeMagazin}";
    }
}
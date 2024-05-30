using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;

namespace Practica
{
    internal class Magazin
    {
        private string Nume;
        private long Telefon;
        private string Adresa;
        public Magazin(bool create=true)
        {
            Console.Write("Numele magazinului: ");
            Nume = Console.ReadLine();
            Console.Write("Nr de telefon al magazinului: ");
            try
            { Telefon = long.Parse(Console.ReadLine()); }
            catch(FormatException)
            { Program.printMessage("Nr de telefon introdus este invalid", ConsoleColor.Red); return; }
            Console.Write("Adresa magazinului: ");
            Adresa = Console.ReadLine();
            DatabaseHelper db = new DatabaseHelper(Program.DATABASE);
            int check = Convert.ToInt32(db.getScalar($"SELECT idMagazin FROM Magazin WHERE Nume='{Nume}' AND Telefon='{Telefon}' AND Adresa='{Adresa}'"));
            db.closeConnection();
            if (check != 0)
            {
                Program.printMessage("Un magazin cu aceleasi date deja exista", ConsoleColor.Red);
                return;
            }
            if(create)
                createMagazin();
        }
        private void createMagazin()
        {
            DatabaseHelper db = new DatabaseHelper(Program.DATABASE);
            db.Query($"INSERT INTO Magazin (Nume, Telefon, Adresa) VALUES ('{Nume}', '{Telefon}', '{Adresa}')");
            db.closeConnection();
            Program.printMessage("Magazin inregistrat", ConsoleColor.Green);
        }
        public static void updateMagazin()
        {
            int[] shops = getShops().ToArray();
            int[] options = Enumerable.Range(1, shops.Length).ToArray();
            string message = "Ce magazin doresti sa modifici? ", errorMsg = "Magazinul nu exista";
            int selMag = shops[Program.getOption(message, errorMsg, options)-1];
            Magazin mg = new Magazin(false);
            DatabaseHelper db = new DatabaseHelper(Program.DATABASE);
            db.Query($"UPDATE Magazin SET Nume='{mg.Nume}', Telefon={mg.Telefon}, Adresa='{mg.Adresa}' WHERE idMagazin=" + selMag);
            db.closeConnection();
            Program.printMessage("Magazinul a fost actualizat", ConsoleColor.Green);
        }
        public static List<int> getShops()
        {
            DatabaseHelper db = new DatabaseHelper(Program.DATABASE);
            SQLiteDataReader reader = db.getReader("SELECT * FROM Magazin");
            List<int> ids = new List<int>();
            int count = 1;
            while(reader.Read())
            {
                Console.WriteLine($"{count}) {reader["Nume"]} {reader["Telefon"]} {reader["Adresa"]}");
                ids.Add(Convert.ToInt32(reader["idMagazin"]));
                count++;
            }
            reader.Close();
            db.closeConnection();
            return ids;
        }
        public static void deleteShop()
        {
            List<int> ids = getShops();
            int[] options = Enumerable.Range(1, ids.Count).ToArray();
            string message = "Ce magazin doresti sa stergi? ", optionError = "Nu avem o astfel de optiune";
            int option = ids[Program.getOption(message, optionError, options)-1];
            DatabaseHelper db = new DatabaseHelper(Program.DATABASE);
            db.Query("DELETE FROM Magazin WHERE idMagazin = " + option);
            db.Query("DELETE FROM Piesa WHERE idMagazin="+option);
            db.closeConnection();
            Program.printMessage("Magazin sters cu succes", ConsoleColor.Green);
        }
    }
}
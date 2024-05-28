using System;

namespace Practica
{
    internal class Comanda
    {
        private string date;
        private uint Cantitate;
        private string Adresa;
        private int idPiesa;
        public Comanda(uint Cantitate, string Adresa, int idPiesa)
        {
            date = DateTime.Today.ToString("yyyy-MM-dd");
            this.Cantitate = Cantitate;
            this.Adresa = Adresa;
            this.idPiesa = idPiesa;
        }
        public Comanda(int idUser)
        {
            date = DateTime.Today.ToString("yyyy-MM-dd");
            idPiesa = getPart();
            DatabaseHelper db = new DatabaseHelper(Program.DATABASE);
            int cant = Convert.ToInt32(db.getScalar("SELECT Cantitate FROM Piesa WHERE idPart=" + idPiesa));
            if(cant == 0)
            {
                Program.printMessage("In stoc sunt 0 piesa", ConsoleColor.Red);
                return;
            }
            Console.WriteLine($"In stoc: {cant} piese");
            while (true)
            {
                Console.Write("Cantitatea: ");
                Cantitate = uint.Parse(Console.ReadLine());
                if (Cantitate <= cant) break;
                Program.printMessage("Prea mult", ConsoleColor.Red);
            }
            Console.Write("Adresa: ");
            Adresa = Console.ReadLine();
            db.Query($"INSERT INTO Comenzi (Data, Cantitate, Adresa, idPiesa, idUser) VALUES ('{date}', {Cantitate}, '{Adresa}', {idPiesa}, {idUser})");
            db.Query($"UPDATE Piesa SET Cantitate={cant-Cantitate} WHERE idPart="+idPiesa);
            db.closeConnection();
            Program.printMessage("Piesa a fost comandata cu succes", ConsoleColor.Green);
        }
        private int getPart()
        {
            int[] shops = Magazin.getShops().ToArray();
            string message = "Din ce magazin doresti sa comanzi? ", errorMsg = "Acest magazin nu exista";
            int selMag = Program.getOption(message, errorMsg, shops);
            int[] parts = Piesa.getParts(selMag).ToArray();
            message = "Ce piesa doresti sa comanzi? "; errorMsg = "Nu exista piesa selectata";
            int selPart = Program.getOption(message, errorMsg, parts);
            return selPart;
        }
    }
}
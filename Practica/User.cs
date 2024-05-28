using System;
using System.Data.SQLite;
using System.Security.Cryptography;
using System.Text;

namespace Practica
{
    internal class User
    {
        private string Username, Password, Adresa;
        private long Telefon;
        private int Type;
        public int type { get { return Type; } }
        public string password {  get { return Password; } }
        private DatabaseHelper db;
        public User(string username, string password, string adresa, long telefon, bool create, int type = 0)
        {
            Username = username;
            Adresa = adresa;
            Telefon = telefon;
            Type = type;
            byte[] hashPass = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(password));
            Password = Convert.ToBase64String(hashPass);
            if(create)
                if (createUser() != 0) Program.printMessage("Utilizator creat cu succes", ConsoleColor.Green);
                else Program.printMessage("Utilizatorul nu a putut fi creat", ConsoleColor.Red);
        }
        public User(bool create)
        {
            getData();
            if (create)
                if (createUser() != 0) Program.printMessage("Utilizator creat cu succes", ConsoleColor.Green);
                else Program.printMessage("Utilizatorul nu a putut fi creat", ConsoleColor.Red);
        }
        private void getData()
        {
            string username, plainPassword, adresa;
            Console.Write("Nume de utilizator: "); username = Console.ReadLine();
            Console.Write("Parola: "); plainPassword = Program.getPassword();
            Console.Write("Adresa: "); adresa = Console.ReadLine();
            Console.Write("Nr de telefon: ");
            try
            {
                Telefon = long.Parse(Console.ReadLine());
                Username = username;
                byte[] hashPass = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(plainPassword));
                Password = Convert.ToBase64String(hashPass);
                Adresa = adresa;
            }
            catch(FormatException)
            {
                Program.printMessage("Numar de telefon incorect\nProcedura se va repeta", ConsoleColor.Red);
                getData();
                return;
            }
        }
        private int createUser()
        {
            db = new DatabaseHelper(Program.DATABASE);
            int check = Convert.ToInt32(db.getScalar($"SELECT idUser FROM Users WHERE Username='{Username}'"));
            if(check != 0)
            {
                Program.printMessage("Utilizator deja exista", ConsoleColor.Red);
                return 0;
            }
            db.Query($"INSERT INTO Users (Username, Password, Telefon, Adresa, Type)" +
                $"VALUES ('{Username}', '{Password}', '{Telefon}', '{Adresa}', '{Type}')");
            check = Convert.ToInt32(db.getScalar($"SELECT idUser FROM Users WHERE Username='{Username}'"));
            db.closeConnection();
            return check;
        }
        public int getUser()
        {
            DatabaseHelper db = new DatabaseHelper(Program.DATABASE);
            int id = Convert.ToInt32(db.getScalar($"SELECT idUser FROM Users WHERE Username='{Username}'"));
            db.closeConnection();
            return id;
        }
        public static User checkPassword(string username, string password)
        {
            string plainPassword = password;
            password = Convert.ToBase64String(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(password)));
            DatabaseHelper db = new DatabaseHelper(Program.DATABASE);
            string sql = $"SELECT * FROM Users WHERE Username = '{username}'";
            SQLiteDataReader reader = db.getReader(sql);
            reader.Read();
            try
            {
                if (password != reader["Password"].ToString())
                {
                    Program.printMessage("\nAutentificarea nu a reusit", ConsoleColor.Red);
                    return null;
                }
                Program.printMessage("\nAutentificat cu succes", ConsoleColor.Green);
                User session = new User(username, plainPassword, reader["Adresa"].ToString(),
                    long.Parse(reader["Telefon"].ToString()), false, int.Parse(reader["Type"].ToString()));
                reader.Close();
                db.closeConnection();
                return session;
            }
            catch(Exception)
            {
                Program.printMessage("\nCont inexistent", ConsoleColor.Red);
                db.closeConnection();
                return null;
            }
        }
        /*public bool updatePassword(string oldPassword, string newPassword)
        {
            oldPassword = Convert.ToBase64String(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(oldPassword)));
            if (oldPassword != Password) return false;
            Password = Convert.ToBase64String(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(newPassword)));
            db = new DatabaseHelper(Program.DATABASE);
            db.Query($"UPDATE Users SET Password='{Password}' WHERE Username='{Username}'");
            db.closeConnection();
            return true;
        }*/
        public static void setprivilege()
        {
            int count = getUsers();
            string message = "Ce utilizator doresti sa privilegiezi? ", optionError = "Nu avem o astfel de optiune";
            int[] options = new int[count];
            for (int i = 1; i <= count; i++) options[i - 1] = i;
            int userSelect = Program.getOption(message, optionError, options);
            message = "0 - Utilizator\n1 - Moderator\n3 - Administrator\nCe privilegiu doresti sa setezi? ";
            options = new int[] { 0, 1, 2 };
            int setPriv = Program.getOption(message, optionError, options);
            DatabaseHelper db = new DatabaseHelper(Program.DATABASE);
            db.Query($"UPDATE Users SET Type={setPriv} WHERE idUser={userSelect}");
            db.closeConnection();
            Program.printMessage("Privilegiu setat", ConsoleColor.Green);
        }
        public static void deleteUser()
        {
            int count = getUsers();
            string message = "Ce utilizator doresti sa stergi? ", optionError = "Nu avem o astfel de optiune";
            int[] options = new int[count];
            for (int i = 1; i <= count; i++) options[i - 1] = i;
            int userSelect = Program.getOption(message, optionError, options);
            DatabaseHelper db = new DatabaseHelper(Program.DATABASE);
            db.Query("DELETE FROM Users WHERE idUser=" + userSelect);
            db.closeConnection();
            Program.printMessage("Utilizator sters cu succes", ConsoleColor.Green);
        }
        private static int getUsers()
        {
            DatabaseHelper db = new DatabaseHelper(Program.DATABASE);
            SQLiteDataReader reader = db.getReader("SELECT idUser, Users.Username, UsersType.description FROM Users JOIN UsersType WHERE Users.Type=UsersType.idType");
            int count = 0;
            while(reader.Read())
            {
                Console.WriteLine($"{reader["idUser"]}) {reader["Username"]} {reader["description"]}");
                count++;
            }
            reader.Close();
            db.closeConnection();
            return count;
        }
        public override string ToString() => $"Nume de utilizator: {Username}\nAdresa: {Adresa}\nTelefon: {Telefon}";
    }
}
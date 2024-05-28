using System;
using System.IO;
using System.Data.SQLite;

namespace Practica
{
    internal class DatabaseHelper
    {
        private string FILENAME = "";
        private SQLiteConnection _connection;
        public DatabaseHelper(string filename)
        {
            FILENAME = filename;
            getConnection();
        }
        private void getConnection()
        {
            if(!File.Exists(FILENAME))
                SQLiteConnection.CreateFile(FILENAME);
            _connection = new SQLiteConnection(@"Data Source=" + FILENAME);
            _connection.Open();
        }
        public void openConnection() => _connection.Open();
        public void closeConnection() => _connection.Close();
        public void Query(string sql)
        {
            SQLiteCommand cmd = new SQLiteCommand(sql, _connection);
            cmd.ExecuteNonQuery();
        }
        public SQLiteDataReader getReader(string sql)
        {
            SQLiteCommand cmd = new SQLiteCommand(sql, _connection);
            return cmd.ExecuteReader();
        }
        public object getScalar(string sql)
        {
            SQLiteCommand cmd = new SQLiteCommand(sql, _connection);
            return cmd.ExecuteScalar();
        }
        public static void exportDatabase()
        {
            string[] tables = { "UsersType", "Users", "Piesa", "Magazin", "Comenzi" };
            DatabaseHelper db = new DatabaseHelper(Program.DATABASE);
            SQLiteDataReader reader;
            foreach (string table in tables)
            {
                reader = db.getReader("SELECT * FROM " + table);
                string[] columns = new string[reader.FieldCount];
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    columns[i] = reader.GetName(i);
                    File.AppendAllText(table + ".csv", reader.GetName(i) + ";");
                }
                File.AppendAllText(table + ".csv", "\n");
                while (reader.Read())
                {
                    foreach (string col in columns)
                        File.AppendAllText(table + ".csv", reader[col].ToString() + ";");
                    File.AppendAllText(table + ".csv", "\n");
                }
                File.AppendAllText(table + ".csv", "\n");
                reader.Close();
            }
            db.closeConnection();
            Program.printMessage("Baza de date a fost exportata", ConsoleColor.Green);
        }
    }
}
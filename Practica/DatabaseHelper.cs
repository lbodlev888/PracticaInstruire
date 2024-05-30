using System;
using System.IO;
using System.Data.SQLite;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography;

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
        public static void exportDatabase(string[] tables)
        {
            //string[] tables = { "UsersType", "Users", "Piesa", "Magazin", "Comenzi" };
            DatabaseHelper db = new DatabaseHelper(Program.DATABASE);
            SQLiteDataReader reader;
            foreach (string table in tables)
            {
                reader = db.getReader("SELECT * FROM " + table);
                string[] columns = new string[reader.FieldCount];
                for (int i = 0; i < reader.FieldCount-1; i++)
                {
                    columns[i] = reader.GetName(i);
                    File.AppendAllText(table + ".csv", reader.GetName(i) + ";");
                }
                columns[columns.Length - 1] = reader.GetName(reader.FieldCount - 1);
                File.AppendAllText(table + ".csv", $"{reader.GetName(reader.FieldCount-1)}\n");
                while (reader.Read())
                {
                    for(int i = 0; i < columns.Length-1; i++)
                        File.AppendAllText(table + ".csv", reader[columns[i]].ToString() + ";");
                    File.AppendAllText(table + ".csv", reader[columns[columns.Length - 1]].ToString() + "\n");
                }
                reader.Close();
            }
            db.closeConnection();
            Program.printMessage("Baza de date a fost exportata", ConsoleColor.Green);
        }
        public static void importDatabase(string[] tables)
        {
            DatabaseHelper db = new DatabaseHelper(Program.DATABASE);
            foreach(string table in tables)
            {
                string filename = table + ".csv";
                string[] lines = File.ReadAllLines(filename);
                if (lines.Length == 1) continue;
                string[] headers = lines[0].Split(';');
                string sql = $"INSERT INTO {table} (";
                for (int i = 0; i < headers.Length - 1; i++) sql += $"{headers[i]},";
                sql += $"{headers[headers.Length-1]}) VALUES ";
                for (int i = 1; i < lines.Length; i++)
                {
                    string[] data = lines[i].Split(';');
                    for(int j = 0; j < data.Length-1; j++)
                    {
                        if (j % headers.Length == 0)
                            if (i == 1) sql += "(";
                            else sql += "), (";
                        if (isString(data[j])) sql += $"'{data[j]}', ";
                        else sql += $"{data[j]}, ";
                    }
                    if (isString(data[data.Length - 1])) sql += $"'{data[data.Length - 1]}'";
                    else sql += $"{data[data.Length - 1]}";
                }
                sql += ")";
                db.Query(sql);
            }
            db.closeConnection();
        }
        private static bool isString(string data)
        {
            if (long.TryParse(data, out long templong) || int.TryParse(data, out int tempint) || DateTime.TryParse(data, out DateTime tempdateTime)) return false;
            return true;
        }
    }
}
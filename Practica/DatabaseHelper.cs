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
        public static void exportDatabase(string[] tables)
        {
            if (!Directory.Exists(Program.DATABASE + " Export"))
                Directory.CreateDirectory(Program.DATABASE + " Export");
            DatabaseHelper db = new DatabaseHelper(Program.DATABASE);
            SQLiteDataReader reader;
            foreach (string table in tables)
            {
                File.WriteAllText($"{Program.DATABASE} Export/{table}.csv", string.Empty);
                reader = db.getReader("SELECT * FROM " + table);
                string[] columns = new string[reader.FieldCount];
                for (int i = 0; i < reader.FieldCount-1; i++)
                {
                    columns[i] = reader.GetName(i);
                    File.AppendAllText($"{Program.DATABASE} Export/{table}.csv", reader.GetName(i) + ";");
                }
                columns[columns.Length - 1] = reader.GetName(reader.FieldCount - 1);
                File.AppendAllText($"{Program.DATABASE} Export/{table}.csv", $"{reader.GetName(reader.FieldCount-1)}\n");
                while (reader.Read())
                {
                    for(int i = 0; i < columns.Length-1; i++)
                    {
                        if(DateTime.TryParse(reader[columns[i]].ToString(), out DateTime result))
                        {
                            File.AppendAllText($"{Program.DATABASE} Export/{table}.csv", result.ToString("yyyy-MM-dd") + ";");
                            continue;
                        }
                        File.AppendAllText($"{Program.DATABASE} Export/{table}.csv", reader[columns[i]].ToString() + ";");
                    }
                    if (DateTime.TryParse(reader[columns[columns.Length - 1]].ToString(), out DateTime result1))
                    {
                        File.AppendAllText($"{Program.DATABASE} Export/{table}.csv", result1.ToString("yyyy-MM-dd") + ";");
                        continue;
                    }
                    File.AppendAllText($"{Program.DATABASE} Export/{table}.csv", reader[columns[columns.Length - 1]].ToString() + "\n");
                }
                reader.Close();
            }
            db.closeConnection();
            Program.printMessage("Baza de date a fost exportata", ConsoleColor.Green);
        }
        public static void importDatabase(string[] tables)
        {
            if(!Directory.Exists(Program.DATABASE + " Export"))
            {
                Program.printMessage("Nu exista fisierele spre importare", ConsoleColor.Red);
                return;
            }
            DatabaseHelper db = new DatabaseHelper(Program.DATABASE);
            foreach(string table in tables)
            {
                string filename = $"{Program.DATABASE} Export/{table}.csv";
                string[] lines;
                try { lines = File.ReadAllLines(filename); }
                catch (FileNotFoundException) { continue; }
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
                        if (isString(data[j]))
                            sql += $"'{data[j]}', ";
                        else sql += $"{data[j]}, ";
                    }
                    if (isString(data[data.Length - 1])) sql += $"'{data[data.Length - 1]}'";
                    else sql += $"{data[data.Length - 1]}";
                }
                sql += ")";
                try { db.Query(sql); }
                catch (SQLiteException) { Program.printMessage("Baza de date nu a putut fi importata", ConsoleColor.Red); return; }
            }
            db.closeConnection();
            Program.printMessage("Baza de date importata cu succes", ConsoleColor.Green);
        }
        private static bool isString(string data) =>
            !(long.TryParse(data, out long templong) || int.TryParse(data, out int tempint));
    }
}
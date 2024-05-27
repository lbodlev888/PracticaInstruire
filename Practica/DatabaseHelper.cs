using System.Data.SQLite;
using System.IO;

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
    }
}

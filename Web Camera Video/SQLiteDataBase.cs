using System;
using System.Data;
using System.Data.SQLite;
using System.IO;

namespace Web_Camera_Video
{
    public class SQLiteDataBase
    {
        private String dbFileName;
        private SQLiteConnection Connection;
        private SQLiteCommand Command;

        public string ErrorMsg;

        public SQLiteDataBase(string FileName)
        {
            dbFileName = FileName;
            Connection = new SQLiteConnection();
            Command = new SQLiteCommand();
        }

        static public SQLiteDataBase Create(string FileName, string Query)
        {
            SQLiteDataBase NewBase = new SQLiteDataBase(FileName);

            if (NewBase.CreateDB(Query))
                return NewBase;
            else
                return null;
        }

        static public SQLiteDataBase Open(string FileName)
        {
            SQLiteDataBase NewBase = new SQLiteDataBase(FileName);

            if (NewBase.OpenDB())
                return NewBase;
            else
                return null;
        }

        private bool OpenDB()
        {
            if (!File.Exists(dbFileName))
            {
                ErrorMsg = "Database file not found";
                return false;
            }

            try
            {
                Connection = new SQLiteConnection("Data Source=" + dbFileName + ";Version=3;");
                Connection.Open();
                Command.Connection = Connection;
            }
            catch (SQLiteException ex)
            {
                ErrorMsg = ex.Message;
                return false;
            }
            return true;
        }

        private bool CreateDB(string Query)
        {
            if (!File.Exists(dbFileName))
                SQLiteConnection.CreateFile(dbFileName);

            try
            {
                Connection = new SQLiteConnection("Data Source=" + dbFileName + ";Version=3;");
                Connection.Open();
                Command.Connection = Connection;

                Command.CommandText = Query;
                Command.ExecuteNonQuery();
            }
            catch (SQLiteException ex)
            {
                ErrorMsg = ex.Message;
                return false;
            }
            return true;
        }

        public DataTable ReadTable(string Query)
        {
            DataTable dTable = new DataTable();

            if (Connection.State != ConnectionState.Open)
            {
                ErrorMsg = "Open Database";
                return null;
            }

            try
            {
                SQLiteDataAdapter adapter = new SQLiteDataAdapter(Query, Connection);
                adapter.Fill(dTable);
            }
            catch (SQLiteException ex)
            {
                ErrorMsg = ex.Message;
                return null;
            }

            return dTable;
        }

        public bool Execute(string Query)
        {
            if (Connection.State != ConnectionState.Open)
            {
                ErrorMsg = "Open connection with database";
                return false;
            }

            try
            {
                Command.CommandText = Query;
                Command.ExecuteNonQuery();
            }
            catch (SQLiteException ex)
            {
                ErrorMsg = ex.Message;
                return false;
            }
            return true;
        }

        public string GetConfigValue(string name)
        {
            DataTable Conf = ReadTable("SELECT `value` FROM `config` WHERE `name`='" + name + "' LIMIT 1");
            return Conf.Rows[0].ItemArray[0].ToString();
        }
    }
}

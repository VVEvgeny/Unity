using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SQLite;
using UnityEngine;

namespace Assets.Scripts.Database
{
    public static class Database
    {
        private static DatabaseHelper _databaseHelper;
        public static DatabaseHelper Get
        {
            get { return _databaseHelper ?? (_databaseHelper = new DatabaseHelper()); }
        }
        /*
        public static SQLiteConnection Connect
        {
            get
            {
                if (_databaseHelper == null) _databaseHelper = new DatabaseHelper();
                return _databaseHelper.connection;
            }
        }
        */
    }

    public class DatabaseHelper
    {
        private const int ROADS_VERSION = 1;
        private const string ROADS_NAME = "Roads";

        public SQLiteConnection connection;
        private const string DBName = "myseen.s3db";
        private string databaseFilePath;

        public DatabaseHelper()
        {
            //xamarin 
            //string folder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            //databaseFilePath = Path.Combine(folder, DBName);
            //unity
            //databaseFilePath = "URI=file:" + Application.dataPath + "/" + DBName;
            databaseFilePath = Application.dataPath + "/" + DBName;

            Debug.Log("databaseFilePath=" + databaseFilePath);
            //Log.Warn(LogTAG, "databaseFilePath=" + databaseFilePath);

            connection = new SQLiteConnection(databaseFilePath);

            if (!File.Exists(databaseFilePath)) CreateTables();
            else
            {
                //var tv = connection.Table<TablesVersion>().Where(t => t.TableName == ROADS_NAME).First();
                var tv = connection.Table<TablesVersion>().FirstOrDefault(t => t.TableName == ROADS_NAME);
                if (tv == null)
                {
                    throw new Exception("БД есть а таблицы нет...");
                }
                Debug.Log("table Films version=" + tv.Version + " current version=" + ROADS_VERSION);

                if (tv.Version < ROADS_VERSION)
                {
                    Debug.Log("table ROADS is OLD RECREAT");
                    connection.DropTable<Roads>();
                    connection.CreateTable<Roads>();
                    tv.Version = ROADS_VERSION;
                    connection.Update(tv);
                }
            }
        }

        private void CreateTables()
        {
            Debug.Log("CreateTables()");
            //Log.Warn(LogTAG, "CreateTables Create tables begin");
            connection.CreateTable<Roads>();
            connection.CreateTable<TablesVersion>();
            connection.Insert(new TablesVersion {TableName = ROADS_NAME, Version = ROADS_VERSION});
        }

        /*
        public int GetSerialsCount()
        {
            return connection.Table<Serials>().Count();
        }
        public void Add(Films film)
        {
            connection.Insert(film);
        }
        public void Update(Films film)
        {
            connection.Update(film);
        }
        public bool isFilmExist(string name)
        {
            return connection.Table<Films>().Where(f => f.Name == name).Count() != 0;
        }
        public bool isFilmExist(int? id)
        {
            if (id.GetValueOrDefault(-1) == -1) return false;
            return connection.Table<Films>().Where(f => f.Id == id.Value).Count() != 0;
        }
        public bool isFilmExistAndNotSame(string name, int id)
        {
            return connection.Table<Films>().Where(f => f.Name == name && f.Id != id).Count() != 0;
        }
        */
    }
}

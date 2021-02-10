using SQLite;
using System.Collections.Generic;
using System.IO;

namespace DataBase
{
    public class Data
    {
        public int ID { set; get; }
        public int ver { set; get; }
        public string name { set; get; }
        public string discription { set; get; }
        public string IDDate { set; get; }
        public string date { set; get; }
    }

    public class DataBase
    {
        private string Path = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "Data.db");
        private const string nameTable = "Doctors";

        public DataBase()
        {
            CreateTable();
        }

        public bool CreateTable()
        {
            try
            {
                if (!File.Exists(Path))
                {
                    File.Create(Path);
                }

                var db = new SQLiteConnection(Path);
                var cmd = new SQLiteCommand(db);

                cmd.CommandText = "CREATE TABLE " + nameTable + " (ID INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, name STRING NOT NULL, discription STRING NOT NULL);";
                cmd.ExecuteNonQuery();

                cmd.CommandText = "CREATE TABLE ver (ver INTEGER NOT NULL);";
                cmd.ExecuteNonQuery();
                
                cmd.CommandText = "CREATE TABLE note (ID INTEGER NOT NULL, number STRING NOT NULL);";
                cmd.ExecuteNonQuery();

                cmd.CommandText = "CREATE TABLE dates (ID INTEGER PRIMARY KEY AUTOINCREMENT UNIQUE NOT NULL, date STRING NOT NULL UNIQUE);";
                cmd.ExecuteNonQuery();

                cmd.CommandText = "CREATE TABLE [dos:date] (IDDoc INTEGER NOT NULL, IDDate INTEGER NOT NULL);";
                cmd.ExecuteNonQuery();

                cmd.CommandText = "INSERT INTO ver (ver) VALUES (0)";
                cmd.ExecuteNonQuery();
                
                db.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void createNote(int ID, string number) 
        {
            var db = new SQLiteConnection(Path);
            var cmd = new SQLiteCommand(db);

            cmd.CommandText = "INSERT INTO note (ID, number) VALUES (" + ID + ", '" + number + "')";
            cmd.ExecuteNonQuery();

            db.Close();
        }

        public int ver
        {
            set
            {
                var db = new SQLiteConnection(Path);
                var cmd = new SQLiteCommand(db);
                cmd.CommandText = "UPDATE ver SET ver=" + value;
                cmd.ExecuteNonQuery();
                db.Close();
            }

            get
            {
                var db = new SQLiteConnection(Path);
                var cmd = new SQLiteCommand(db);
                cmd.CommandText = "SELECT * FROM ver";
                List<Data> vs = cmd.ExecuteQuery<Data>();
                db.Close();
                return vs[0].ver;
            }
        }

        public void newOrder(int ID, string name, string phone, string birthday, string orderDay, string first) 
        {
            var db = new SQLiteConnection(Path);
            var cmd = new SQLiteCommand(db);

            cmd.CommandText = $"INSERT INTO note (ID, name, phone, birthday, day, first) VALUES ({ID}, '{name}', '{phone}', '{birthday}', '{orderDay}', '{first}')";
            cmd.ExecuteNonQuery();

            cmd.CommandText = $"SELECT ID FROM dates WHERE date='{orderDay}'";
            List<Data> data = cmd.ExecuteQuery<Data>();

            cmd.CommandText = $"DELETE FROM [dos:date] WHERE IDDate={data[0].ID}";
            cmd.ExecuteNonQuery();

            db.Close();
        }

        public List<string> orderDates(int IDdoc) 
        {
            var db = new SQLiteConnection(Path);
            var cmd = new SQLiteCommand(db);
            
            cmd.CommandText = "SELECT * FROM [dos:date] WHERE IDDoc = " + IDdoc;
            
            List<Data> list = cmd.ExecuteQuery<Data>();
            List<string> dates = new List<string>();
            
            for (int i = 0; i < list.Count; i++) 
            {
                cmd.CommandText = "SELECT * FROM dates WHERE ID = " + list[i].IDDate;
                List<Data> date = cmd.ExecuteQuery<Data>();
                dates.Add(date[0].date);
            }
            
            db.Close();
            return dates;
        }

        public void Insert(string name, string discription)
        {
            var db = new SQLiteConnection(Path);
            var cmd = new SQLiteCommand(db);
            cmd.CommandText = "INSERT INTO " + nameTable + " (name, discription) VALUES ('" + name + "', '" + discription + "')";
            cmd.ExecuteNonQuery();
            db.Close();
        }

        public List<Data> GetData()
        {
            var db = new SQLiteConnection(Path);
            var cmd = new SQLiteCommand(db);

            cmd.CommandText = "SELECT * FROM " + nameTable;
            List<Data> name = cmd.ExecuteQuery<Data>();

            db.Close();
            return name;
        }
    }
}
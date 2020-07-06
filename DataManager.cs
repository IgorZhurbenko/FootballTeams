using System;
using System.Collections.Generic;
using System.Text;
using FootballTeams.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using System.Text.Json;
using Microsoft.Data.Sqlite;

namespace FootballTeams.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder DbOptions)
        {
            string DBName = "DB";
            DbOptions.UseSqlite(connectionString: @"FileName=wwwroot\" + DBName + ".db3");

            
        }

        public DbSet<Player> Players { get; set; }
    }

    public static class DataManager
    {
        public static SQLiteConnection conn;
        //static string InstanceName = "TESTSQLINSTANCE";
        static string DbName = "Players";
        static DataManager()
        {
            if (File.Exists(@"wwwroot\dbsettings.txt"))
            { 
                DbName = File.ReadAllText(@"wwwroot\dbsettings.txt");
            }
            EnsureDatabaseExists();
            EnsureTableExists();
            conn = NewConnection();
            conn.Open();
        }
        private static SQLiteConnection NewConnection()
        {
            string connectionString = @$"Data Source=.\{DbName}.db3;Version=3;";
            return new SQLiteConnection(connectionString);
        }

        public static string[] PlayerMainFields = { "id", "FirstName", "LastName" };

        public static string[] PlayerShowableFields = { "id", "FirstName", "LastName", "TeamId", "Country", "Gender", "BirthDate"};

        public static void EnsureDatabaseExists()
        {
            if (!File.Exists($@"wwwroot\{DbName}.db3"))
            {
                SQLiteConnection.CreateFile($@"wwwroot\{DbName}.db3");
            }            
        }
        public static void AddBasicData()
        {
            
        }
        public static string ParameterStringToCreateVUE()
        {
            string res = "";
            bool First = true;
            foreach (string Field in PlayerShowableFields)
            {
                res = res + (First ? "" : ",\n") + Field + ": \' \'";
                First = false;
            }
            return res;

        }

        public static void EnlistNewPlayer(Dictionary<string, object> Data)
        {
            var FilteredData = Data.Where(elem => PlayerShowableFields.Contains(elem.Key) && elem.Key != "id");

            string QueryText = "insert into PlayersInfo (" +
                String.Join(", ", FilteredData.Select(elem => elem.Key))
                + ")" + "values (" +
                String.Join(", ", FilteredData.Select(elem => QuoteIfString(elem.Key, elem.Value))) + ")";
            //" where id = " + Data["id"].ToString();

            //var conn = NewConnection();
            //conn.Open();

            new SQLiteCommand(QueryText, conn).ExecuteNonQuery();

            //conn.Close();
        }

        public static void EnsureTableExists()
        {
            //var conn = NewConnection();
            var TableCreatingExpression = "Create table 'PlayersInfo' (INTEGER PRIMARY KEY, FirstName text, " +
                "LastName text, Gender int, TeamId int, Country text, BirthDate text)";
            try
            {
                var command = new SQLiteCommand(TableCreatingExpression, conn);
                //conn.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                var a = ex.Message;
            }
            finally
            { 
                //conn.Close();
            }

        }

        private static string QuoteIfString(string FieldName, object PossibleString)
        {
            if (!FieldName.ToLower().Contains("id") && ((JsonElement)PossibleString).ValueKind is JsonValueKind.String)
            {
                return "'" + PossibleString.ToString() + "'";
            }
            else { return PossibleString.ToString(); }
        }

        public static void UpdatePlayerInfo(Dictionary<string, object> Data)
        {
            var FilteredData = Data.Where(elem => PlayerShowableFields.Contains(elem.Key) && elem.Key != "id");

            string QueryText = "update PlayersInfo set " + String.Join(", ",
                FilteredData.Select(elem => elem.Key + " = " + QuoteIfString(elem.Key, elem.Value))) +
                " where id = " + Data["id"].ToString();

            //var conn = NewConnection();
            //conn.Open();

            new SQLiteCommand(QueryText, conn).ExecuteNonQuery();

            //conn.Close();
        }

        //public static void 

        public static IEnumerable<Dictionary<string, object>> AllPlayers()
        {
            //var conn = NewConnection();
            string GetterCommand = "select " + 
                //String.Join(", ", PlayerMainFields.Select(elem => $@"'{elem}'")) + 
                "*" +
                " from PlayersInfo";
            var command = new SQLiteCommand(GetterCommand, conn);
            //conn.Open();
            var Selection = command.ExecuteReader();
            return Selection.ToListOfDictionaries();
        }

        public static string AllPlayersForVue()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(AllPlayers());
        }
        public static object PlayerInfo(int id)
        {
            //var conn = NewConnection();
            string GetterQueryText = "select * from PlayersInfo where id = " + id.ToString();

            var command = new SQLiteCommand(GetterQueryText, conn);
            //conn.Open();
            var Selection = command.ExecuteReader();

            object res;

            if (Selection.Read())
            {
                res = Selection.ToDictionary();
            }
            else
            {
                res = null;
            }
            //conn.Close();
            return res;

        }

        public static void DeletePlayer(int id)
        {
            string QueryText = "Delete from PlayersInfo where id = " + id.ToString();
            //var conn = NewConnection();
            //conn.Open();
            new SQLiteCommand(QueryText, conn).ExecuteNonQuery();
            //conn.Close();
        }

        public static List<Dictionary<string, object>> GetAllTeams()
        {
            string GetterCommand = "select " +
                //String.Join(", ", PlayerMainFields.Select(elem => $@"'{elem}'")) + 
                "*" +
                " from Teams";
            var command = new SQLiteCommand(GetterCommand, conn);
            //conn.Open();
            var Selection = command.ExecuteReader();
            return Selection.ToListOfDictionaries();
        }

        internal static string EnlistNewTeam(Dictionary<string, object> Data)
        {
            //throw new NotImplementedException();
            var FilteredData = Data.Where(elem => elem.Key.ToLower() != "id");

            string QueryText = "insert into Teams (" +
                String.Join(", ", FilteredData.Select(elem => elem.Key))
                + ")" + "values (" +
                String.Join(", ", FilteredData.Select(elem => QuoteIfString(elem.Key, elem.Value))) + ")";
            //" where id = " + Data["id"].ToString();

            //var conn = NewConnection();
            //conn.Open();

            new SQLiteCommand(QueryText, conn).ExecuteNonQuery();

            return GetIdLastAddedTeamWithName(Data["Name"].ToString());
            //conn.Close();
        }

        private static string GetIdLastAddedTeamWithName(string Name)
        {
            string QueryText = $"select Id from Teams where Name = '{Name}' order by id desc limit 1";
            return new SQLiteCommand(QueryText, conn).ExecuteScalar().ToString();
        }
    }

    public static class SelectionExtension
    {
        public static Dictionary<string, object> ToDictionary(this SQLiteDataReader Selection)
        {
            var Result = new Dictionary<string, object>();
            for (int i = 0; i < Selection.VisibleFieldCount; i++)
            {
                Result.Add(Selection.GetName(i), Selection.GetValue(i));
            }
            return Result;
        }

        public static List<Dictionary<string, object>> ToListOfDictionaries(this SQLiteDataReader Selection)
        {
            List<Dictionary<string, object>> res = new List<Dictionary<string, object>>();
            while (Selection.Read())
            {
                res.Add(Selection.ToDictionary());
            }
            return res;
        }
    }

    
}

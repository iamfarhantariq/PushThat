using Notification.Model;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Notification.SQLite
{
    public class SQLiteDatabase
    {
        private static SQLiteAsyncConnection _sqlconnection;

        public SQLiteDatabase()
        {
            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Notification.db3");
            _sqlconnection = new SQLiteAsyncConnection(dbPath);
            _sqlconnection.CreateTableAsync<MessageClass>().Wait();
            _sqlconnection.CreateTableAsync<EditQuitHoursModel>().Wait();
        }

        public static Task Dispose()
        {
            return _sqlconnection.CloseAsync();
        }

        public static Task<int> InsertAsync<T>(T Model)
        {
            return _sqlconnection.InsertAsync(Model);
        }

        public static Task<int> InsertOrReplaceAsync<T>(T Model)
        {
            return _sqlconnection.InsertOrReplaceAsync(Model);
        }

        public static Task<int> InsertAllAsync<T>(List<T> Model)
        {
            return _sqlconnection.InsertAllAsync(Model);
        }

        public static Task<int> UpdateAsync<T>(T Model)
        {
            Task<int> res = _sqlconnection.UpdateAsync(Model);
            return res;
        }

        public static Task<int> DeleteAsync<T>(T Model)
        {
            return _sqlconnection.DeleteAsync(Model);
        }
        public static Task<int> DeleteAllAsync<T>()
        {
            return _sqlconnection.DeleteAllAsync<T>();
        }

        public static Task<List<T>> GetAllTableDataAsync<T>() where T : new()
        {
            return _sqlconnection.Table<T>().ToListAsync();
        }

        //public static Task<MessageClass> GetDistinct<MessageClass>() where T : new()
        //{
        //    Task<T> distinct = _sqlconnection.FindWithQueryAsync<T>("SELECT MessageFrom, Count(MessageFrom) as count FROM MessageClass Group by MessageFrom;");
        //    return distinct;
        //}


        //        SELECT Name, COUNT(name) as count
        //        FROM Name
        //        GROUP BY Name

        //public static Task<T> GetSingleDataByIdAsync<T>(int id) where T : new()
        //{
        //    return connection.Table<T>().Where(p => p.GetHashCode() == id).FirstOrDefaultAsync();
        //}

        //public static Task<T> GetSingleDataByIdAsync<T>(string tableName, string columnName, int id) where T : new()
        //{
        //    return connection.FindWithQueryAsync<T>("SELECT * FROM " + tableName + " Where " + columnName + " = " + id);
        //}

        //#region WapointsById

        //public static Task<List<WayPoint>> GetAllWaypointsByTripIdAsync(int id)
        //{
        //    return connection.Table<WayPoint>().Where(p => p.TripId == id).ToListAsync();
        //}


        //#endregion

        //public static Task<T> GetLastRecord<T>(string TableName) where T : new()
        //{
        //    Task<T> lastRecord = connection.FindWithQueryAsync<T>("SELECT * FROM " + TableName + " ORDER BY Id DESC LIMIT 1");
        //    return lastRecord;
        //}
        //public static Task<T> GetLastRecordByFkId<T>(string TableName, string FkPropertyName, int Fk) where T : new()
        //{
        //    Task<T> lastRecord = connection.FindWithQueryAsync<T>("SELECT * FROM " + TableName + " Where " + FkPropertyName + " = " + Fk + " ORDER BY Id DESC LIMIT 1");
        //    return lastRecord;
        //}

        //public static Task<T> CustomQuery<T>(string query) where T : new()
        //{
        //    return connection.FindWithQueryAsync<T>(query);
        //}
        //public static Task<List<T>> CustomQueryReturnList<T>(string query) where T : new()
        //{
        //    return connection.QueryAsync<T>(query);
        //}
        //public static Task<T> GetLastRecordWhereColumn<T>(string TableName, string FkPropertyName, int FkInt = 0, string FkString = null) where T : new()
        //{
        //    if (FkInt == 0)
        //    {
        //        Task<T> lastRecord = connection.FindWithQueryAsync<T>("SELECT * FROM " + TableName + " Where " + FkPropertyName + " = \"" + FkString + "\" ORDER BY Id DESC LIMIT 1");
        //        return lastRecord;
        //    }
        //    else
        //    {
        //        Task<T> lastRecord = connection.FindWithQueryAsync<T>("SELECT * FROM " + TableName + " Where " + FkPropertyName + " = " + FkInt + " ORDER BY Id DESC LIMIT 1");
        //        return lastRecord;
        //    }

        //}

        //public static Task<List<T>> GetAllRecordByFkId<T>(string TableName, string ColumnName, int id) where T : new()
        //{
        //    Task<List<T>> allRecord = connection.QueryAsync<T>("SELECT * FROM " + TableName + " Where " + ColumnName + " = " + id + " ORDER BY Id");
        //    return allRecord;
        //}

        //public static Task<List<T>> GetAllRecordByColumns<T>(string TableName, string FkTableName, int Fk, string colName, int val) where T : new()
        //{
        //    Task<List<T>> allRecord = connection.QueryAsync<T>("SELECT * FROM " + TableName + " Where " + FkTableName + " = " + Fk + " and " + colName + " = " + val + " ORDER BY Id");
        //    return allRecord;
        //}

        //public static void TruncateTable<T>() where T : new()
        //{
        //    connection.DropTableAsync<T>().Wait();
        //    connection.CreateTableAsync<T>().Wait();
        //}
        //#endregion
    }
}


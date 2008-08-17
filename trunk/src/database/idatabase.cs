using System;
namespace Drive_LFSS.Database_
{
    public interface IDatabase
    {
        void CancelCommand();
        void EndTransaction();
        int ExecuteNonQuery(string _command);
        System.Data.IDataReader ExecuteQuery(string _command);
        IAsyncResult NewExecuteNonQuery();
        int EndExecuteNonQuery(IAsyncResult _iaSyncResult);
        uint GetLastRowId(string tableName);
        bool IsExistColum(string tableName, string colName);
        bool IsExistTable(string tableName);
        void NewTransaction();
    }
}

using System;
namespace Drive_LFSS.Database_
{
    interface IDatabase
    {
        void CancelCommand();
        void EndTransaction();
        void NewTransaction();
        int ExecuteNonQuery(string _command);
        System.Data.IDataReader ExecuteQuery(string _command);
        uint GetLastRowId(string _tableName);
        bool IsExistColum(string tableName, string colName);
        bool IsExistTable(string tableName);
    }
}

using System;
namespace Drive_LFSS.Database_
{
    public interface IDatabase
    {
        void Lock();
        void Unlock();
        void CancelCommand();
        void EndTransaction();
        int ExecuteNonQuery(string _command);
        System.Data.IDataReader ExecuteQuery(string _command);
        //IAsyncResult NewExecuteNonQuery();
       // int EndExecuteNonQuery(IAsyncResult _iaSyncResult);
        void NewTransaction();
    }
}

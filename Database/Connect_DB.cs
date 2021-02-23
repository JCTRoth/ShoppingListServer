using System;
using System.Data.SqlClient;
using Dapper;

namespace ShoppingListServer.Database
{
    public class Connect_DB
    {
            // DB Entery
            // SyncID, UserID, AcessType
            // string,string,int


            /* Connect_DB()
            {
                var cs = @"Server=localhost\ShoppingAPI;Database=testdb;Trusted_Connection=True;";

                using var con = new SqlConnection(cs);
                con.Open();

                var version = con.ExecuteScalar<string>("SELECT @@VERSION");

                Console.WriteLine(version);
            } */
        }
}
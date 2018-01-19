using LOG.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Web;

namespace LOG
{
    public class LOGDAL
    {
        CommonModels common = new CommonModels();

        public void InsertUser(UserModel model)
        {
            var connectionString = common.GetConnectionString();

            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();
                OleDbCommand cmd = new OleDbCommand();
                
                if (!CheckTableExists(LogConstants.UserTable))
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "CREATE TABLE " + LogConstants.UserTable + " (UserId INT, UserName VARCHAR, ContactNo VARCHAR , EmailId VARCHAR , Subject VARCHAR , Message VARCHAR );";
                    cmd.ExecuteNonQuery();

                }

                int _id = GetPreviouId(LogConstants.UserTable);

                string commnadTxt = "INSERT INTO " + LogConstants.UserTable + "(UserId,UserName,ContactNo,EmailId,Subject,Message) VALUES(@UserId,@UserName,@ContactNo,@EmailId,@Subject,@Message);";

                cmd = new OleDbCommand(commnadTxt, conn);

                cmd.Parameters.AddWithValue("@UserId", _id + 1);
                cmd.Parameters.AddWithValue("@UserName", model.UserName);
                cmd.Parameters.AddWithValue("@ContactNo", model.ContactNo);
                cmd.Parameters.AddWithValue("@EmailId", model.EmailId);
                cmd.Parameters.AddWithValue("@Subject", model.Subject);
                cmd.Parameters.AddWithValue("@Message", model.Message);

                cmd.ExecuteNonQuery();

                conn.Close();
            }


        }

        public int GetPreviouId(string table)
        {
            var connectionString = common.GetConnectionString();

            DataSet ds = new DataSet();

            int maxId = 0;

            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();
                OleDbCommand cmd = new OleDbCommand();
                cmd.Connection = conn;

                cmd.CommandText = "SELECT Max(UserId) FROM [" + table + "]";

                if (!(cmd.ExecuteScalar() is DBNull))
                {
                    maxId = Convert.ToInt32(cmd.ExecuteScalar());
                }

                cmd = null;

            }

            return maxId;

        }

        public bool CheckTableExists(string table)
        {
            var connectionString = common.GetConnectionString();
            var exists = false;

            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();
                OleDbCommand cmd = new OleDbCommand();
                cmd.Connection = conn;

                exists = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, table, "TABLE" }).Rows.Count > 0;

                conn.Close();
            }

            return exists;
        }
    }
}
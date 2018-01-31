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
                    cmd.CommandText = "CREATE TABLE " + LogConstants.UserTable + " (UserId INT, UserName VARCHAR,[Password] VARCHAR, ContactNo VARCHAR , EmailId VARCHAR , Subject VARCHAR , Message VARCHAR , IsAdmin BIT);";
                    cmd.ExecuteNonQuery();

                }

                int _id = GetPreviouId(LogConstants.UserTable, "UserId");
                string commnadTxt = "INSERT INTO " + LogConstants.UserTable + "(UserId,UserName,[Password],ContactNo,EmailId,Subject,Message,IsAdmin) VALUES(@UserId,@UserName,@Password,@ContactNo,@EmailId,@Subject,@Message,@IsAdmin);";

                cmd = new OleDbCommand(commnadTxt, conn);

                cmd.Parameters.AddWithValue("@UserId", _id + 1);
                cmd.Parameters.AddWithValue("@UserName", model.UserName ?? string.Empty);
                cmd.Parameters.AddWithValue("@Password", model.Password ?? string.Empty);
                cmd.Parameters.AddWithValue("@ContactNo", model.ContactNo ?? string.Empty);
                cmd.Parameters.AddWithValue("@EmailId", model.EmailId ?? string.Empty);
                cmd.Parameters.AddWithValue("@Subject", model.Subject ?? string.Empty);
                cmd.Parameters.AddWithValue("@Message", model.Message ?? string.Empty);
                cmd.Parameters.AddWithValue("@IsAdmin", model.IsAdmin);

                cmd.ExecuteNonQuery();

                conn.Close();
            }


        }

        public int GetPreviouId(string table, string column)
        {
            var connectionString = common.GetConnectionString();

            DataSet ds = new DataSet();

            int maxId = 0;

            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();
                OleDbCommand cmd = new OleDbCommand();
                cmd.Connection = conn;

                cmd.CommandText = "SELECT Max(" + column + ") FROM [" + table + "]";

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

        public void InsertUploadItem(UploadModel model)
        {
            var connectionString = common.GetConnectionString();

            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();
                OleDbCommand cmd = new OleDbCommand();

                if (!CheckTableExists(LogConstants.UploadTable))
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "CREATE TABLE " + LogConstants.UploadTable + " (UploadId INT, UploadType VARCHAR, Title VARCHAR ,FilePath VARCHAR);";
                    cmd.ExecuteNonQuery();

                }

                int _id = GetPreviouId(LogConstants.UploadTable, "UploadId");
                string commnadTxt = "INSERT INTO " + LogConstants.UploadTable + "(UploadId,UploadType,Title,FilePath) VALUES(@UploadId,@UploadType,@Title,@FilePath);";

                cmd = new OleDbCommand(commnadTxt, conn);

                cmd.Parameters.AddWithValue("@UploadId", _id + 1);
                cmd.Parameters.AddWithValue("@UploadType", model.UploadType ?? string.Empty);
                cmd.Parameters.AddWithValue("@Title", model.Title ?? string.Empty);
                cmd.Parameters.AddWithValue("@FilePath", model.FilePath ?? string.Empty);


                cmd.ExecuteNonQuery();

                conn.Close();
            }
        }
    }
}
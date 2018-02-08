using LOG.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using Newtonsoft.Json.Linq;
using System.Web.Script.Serialization;
using System.IO;
using Newtonsoft.Json;


namespace LOG
{
    public class LOGDAL
    {
        CommonModels common = new CommonModels();

        public void InsertUser(UserModel model)
        {
            var usersList = new List<UserModel>();

            string jsonFilePath = LogConstants.APP_DATA_PATH + "" + LogConstants.UserTable + ".json";


            if (File.Exists(jsonFilePath))
            {


                usersList = JsonConvert.DeserializeObject<List<UserModel>>(File.ReadAllText(jsonFilePath));

                model.UserId = usersList.Count() > 0 ? usersList.Max(i => i.UserId) + 1 : 1;

            }

            else { model.UserId = 1; }

            usersList.Add(model);

            string jsondata = JsonConvert.SerializeObject(usersList, Formatting.Indented);

            System.IO.File.WriteAllText(LogConstants.APP_DATA_PATH + "" + LogConstants.UserTable + ".json", jsondata);

        }




        public int GetPreviouId(string table, string column)
        {

            int maxId = 0;

            string jsonFilePath = LogConstants.APP_DATA_PATH + "" + table + ".json";

            if (File.Exists(jsonFilePath))
            {
                var json = File.ReadAllText(jsonFilePath);

                var jsonObj = JObject.Parse(json);
            }

            return maxId + 1;

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

            var uploadItems = new List<UploadModel>();

            string jsonFilePath = LogConstants.APP_DATA_PATH + "" + LogConstants.UploadTable + ".json";


            if (File.Exists(jsonFilePath))
            {


                uploadItems = JsonConvert.DeserializeObject<List<UploadModel>>(File.ReadAllText(jsonFilePath));

                model.UploadId = uploadItems.Count() > 0 ? uploadItems.Max(i => i.UploadId) + 1 : 1;

            }
            else { model.UploadId = 1; }

            uploadItems.Add(model);

            string jsondata = JsonConvert.SerializeObject(uploadItems, Formatting.Indented);

            System.IO.File.WriteAllText(LogConstants.APP_DATA_PATH + "" + LogConstants.UploadTable + ".json", jsondata);

        }


        public List<UploadModel> GetUploadedItems()
        {
            var itemsList = new List<UploadModel>();

            string jsonFilePath = LogConstants.APP_DATA_PATH + "" + LogConstants.UploadTable + ".json";


            if (File.Exists(jsonFilePath))
            {

                itemsList = JsonConvert.DeserializeObject<List<UploadModel>>(File.ReadAllText(jsonFilePath));

            }

            return itemsList;
        }

        public List<UploadModel> GetMessages(short type)
        {

            var itemsList = new List<UploadModel>();

            string jsonFilePath = LogConstants.APP_DATA_PATH + "" + LogConstants.UploadTable + ".json";


            if (File.Exists(jsonFilePath))
            {

                itemsList = JsonConvert.DeserializeObject<List<UploadModel>>(File.ReadAllText(jsonFilePath)).Where(i => i.UploadType == type.ToString()).ToList();

            }

            return itemsList;
        }

        public bool IsAdmin(UserModel login)
        {
            var usersList = new List<UserModel>();

            bool isAdmin = false;

            string jsonFilePath = LogConstants.APP_DATA_PATH + "" + LogConstants.UserTable + ".json";


            if (File.Exists(jsonFilePath))
            {

                usersList = JsonConvert.DeserializeObject<List<UserModel>>(File.ReadAllText(jsonFilePath));

                if (usersList.Any(i => i.UserName == login.UserName && i.Password == login.Password && i.IsAdmin == true))
                {

                    isAdmin = true;
                }
            }

            return isAdmin;
        }

        public List<UserModel> GetAllVisitors()
        {
            var visitorsList = new List<UserModel>();

            string jsonFilePath = LogConstants.APP_DATA_PATH + "" + LogConstants.UserTable + ".json";


            if (File.Exists(jsonFilePath))
            {

                visitorsList = JsonConvert.DeserializeObject<List<UserModel>>(File.ReadAllText(jsonFilePath)).Where(i => i.IsAdmin == false).ToList();

            }

            return visitorsList;
        }

        public bool DeleteUploadedItem(int uploadId)
        {
            var isDeleted = true;

            var itemsList = new List<UploadModel>();

            string jsonFilePath = LogConstants.APP_DATA_PATH + "" + LogConstants.UploadTable + ".json";

            try
            {

                itemsList = JsonConvert.DeserializeObject<List<UploadModel>>(File.ReadAllText(jsonFilePath));

                var toBeDeleted = itemsList.Where(i => i.UploadId == uploadId).FirstOrDefault();

                itemsList.Remove(toBeDeleted);

                string jsondata = JsonConvert.SerializeObject(itemsList, Formatting.Indented);

                System.IO.File.WriteAllText(LogConstants.APP_DATA_PATH + "" + LogConstants.UploadTable + ".json", jsondata);

                if (File.Exists(toBeDeleted.FilePath))
                {
                    File.Delete(toBeDeleted.FilePath);

                }
            }
            catch
            {

                isDeleted = false;
            }


            return isDeleted;
        }

        public List<UploadModel> GetGalleryImages()
        {
            var itemsList = new List<UploadModel>();

            string jsonFilePath = LogConstants.APP_DATA_PATH + "" + LogConstants.UploadTable + ".json";


            if (File.Exists(jsonFilePath))
            {

                itemsList = JsonConvert.DeserializeObject<List<UploadModel>>(File.ReadAllText(jsonFilePath)).Where(i => i.UploadType == Upload.Gallery.ToString()).ToList();

            }

            return itemsList;
        }
    }
}
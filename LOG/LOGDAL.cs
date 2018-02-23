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
using System.Configuration;
using System.Text;
using System.Net.Mail;


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

        /// <summary>
        /// Send an email from [DELETED]
        /// </summary>
        /// <param name="to">Message to address</param>
        /// <param name="body">Text of message to send</param>
        /// <param name="subject">Subject line of message</param>
        /// <param name="fromAddress">Message from address</param>
        /// <param name="fromDisplay">Display name for "message from address"</param>
        /// <param name="credentialUser">User whose credentials are used for message send</param>
        /// <param name="credentialPassword">User password used for message send</param>
        /// <param name="attachments">Optional attachments for message</param>
        public static string Email(string to,
                                 string body,
                                 string subject,
                                 List<HttpPostedFileBase> attachments)
        {

            try
            {
                MailMessage mail = new MailMessage();
                mail.Body = body;
                mail.IsBodyHtml = true;
                mail.To.Add(new MailAddress(to));
                mail.From = new MailAddress(LogConstants.GMAILUserName, LogConstants.DiplayName, Encoding.UTF8);
                mail.Subject = subject;
                mail.SubjectEncoding = Encoding.UTF8;
                mail.Priority = MailPriority.Normal;

                if (attachments != null)
                {

                    foreach (var file in attachments)
                    {
                        var attachment = new Attachment(file.InputStream, file.FileName);
                        mail.Attachments.Add(attachment);
                        
                    }
                }

                SmtpClient smtp = new SmtpClient();
                smtp.EnableSsl = true;
                smtp.Credentials = new System.Net.NetworkCredential(LogConstants.GMAILUserName, LogConstants.GMAILPassword);
                smtp.Host = LogConstants.SMTPHost;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Port = 587;
                smtp.Send(mail);

                return "Success";
            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder(1024);
                sb.Append("\nTo:" + to);
                sb.Append("\nbody:" + body);
                sb.Append("\nsubject:" + subject);
                sb.Append("\nfromAddress:" + LogConstants.SMTPHost);
                sb.Append("\nfromDisplay:" + LogConstants.DiplayName);
                sb.Append("\ncredentialUser:" + LogConstants.GMAILUserName);
                sb.Append("\ncredentialPasswordto:" + LogConstants.GMAILPassword);
                sb.Append("\nHosting:" + LogConstants.SMTPHost);

                return "Failed" + sb + ex.StackTrace;
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LOG.Models
{
    public static class LogConstants
    {

        public static string UserTable = "LogUsers";

        public static string UploadTable = "UploadItems";

        public static string APP_DATA_PATH = System.Web.HttpContext.Current.Server.MapPath("~/App_Data/");

        public static string GMAILUserName = ConfigurationManager.AppSettings["UserName"];

        public static string GMAILPassword = ConfigurationManager.AppSettings["Password"];

        public static string SMTPHost = ConfigurationManager.AppSettings["SMTPHost"];

        public static string DiplayName = ConfigurationManager.AppSettings["DiplayName"];


    }

    public static class About
    {
        public static int Mission = 1;
        public static int WhoWeAre = 2;
        public static int Gallery = 3;

    }

    public static class Upload
    {
        public const short Tract = 1;
        public const short Audio = 2;
        public const short Vedio = 3;
        public const short Gallery = 4;


        public static List<SelectListItem> GetUploadFileTypes()
        {

            var list = new List<SelectListItem>();

            list.Add(new SelectListItem { Value = Upload.Tract.ToString(), Text = "Tract" });
            list.Add(new SelectListItem { Value = Upload.Audio.ToString(), Text = "Audio" });
            list.Add(new SelectListItem { Value = Upload.Vedio.ToString(), Text = "Vedio" });
            list.Add(new SelectListItem { Value = Upload.Gallery.ToString(), Text = "Gallery" });

            return list;

        }

        public static string GetUploadType(string type)
        {
            return GetUploadFileTypes().Where(i => i.Value == type.ToString()).FirstOrDefault().Text;
        }
    }

    public static class GalleryType
    {
        public const short Gospel = 1;
        public const short Baptism = 2;
        public const short Anniversary = 3;
        public const short ChristmasNewYear = 4;
        public const short SpecailEvents = 5;

        public static List<SelectListItem> GetGalleryTypes()
        {

            var list = new List<SelectListItem>();

            list.Add(new SelectListItem { Value = Upload.Tract.ToString(), Text = "Gospel" });
            list.Add(new SelectListItem { Value = Upload.Audio.ToString(), Text = "Baptism" });
            list.Add(new SelectListItem { Value = Upload.Vedio.ToString(), Text = "Anniversary" });
            list.Add(new SelectListItem { Value = Upload.Gallery.ToString(), Text = "ChristmasNewYear" });
            list.Add(new SelectListItem { Value = Upload.Gallery.ToString(), Text = "SpecailEvents" });

            return list;

        }

        public static string GetGalleryType(string type)
        {
            return GetGalleryTypes().Where(i => i.Value == type.ToString()).FirstOrDefault().Text;
        }
    }
}
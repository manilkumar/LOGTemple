using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LOG.Models
{
    public static class LogConstants
    {

        public static string UserTable = "LogUsers";
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

        public static List<SelectListItem> GetUploadFileTypes()
        {

            var list = new List<SelectListItem>();

            list.Add(new SelectListItem { Value = Upload.Tract.ToString(), Text = "Tract" });
            list.Add(new SelectListItem { Value = Upload.Audio.ToString(), Text = "Audio" });
            list.Add(new SelectListItem { Value = Upload.Vedio.ToString(), Text = "Vedio" });

            return list;

        }
    }
}
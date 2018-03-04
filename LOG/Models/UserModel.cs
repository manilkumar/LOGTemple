using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LOG.Models
{
    public class UserModel
    {
        public int UserId { get; set; }

        public string UserName { get; set; }
        
        public string Password { get; set; }

        public string ContactNo { get; set; }

        public string EmailId { get; set; }

        public string Subject { get; set; }

        public string Message { get; set; }

        public bool IsAdmin { get; set; }

        public bool IsQuery { get; set; }

        public bool IsPrayerRequest { get; set; }
    }

    public class LoginModel
    {

        public string UserName { get; set; }

        public string Password { get; set; }
    }

    public class UploadModel
    {

        public string UploadType { get; set; }

        public string Title { get; set; }

        public string FilePath { get; set; }

        public int UploadId { get; set; }

        public short GalleryType { get; set; }
    
    }


}
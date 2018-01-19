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

        public string ContactNo { get; set; }

        public string EmailId { get; set; }

        public string Subject { get; set; }

        public string Message { get; set; }
    }
}
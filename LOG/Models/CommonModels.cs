using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;


namespace LOG.Models
{
    public class CommonModels
    {
       

        public string GetConnectionString()
        {
            Dictionary<string, string> props = new Dictionary<string, string>();

         
            props["Provider"] = "Microsoft.ACE.OLEDB.12.0;";
            props["Extended Properties"] = "Excel 12.0 XML";
            props["Data Source"] = HttpContext.Current.Server.MapPath("~/LogDB.xls");

            StringBuilder sb = new StringBuilder();

            foreach (KeyValuePair<string, string> prop in props)
            {
                sb.Append(prop.Key);
                sb.Append('=');
                sb.Append(prop.Value);
                sb.Append(';');
            }

            return sb.ToString();
        }
    }
}
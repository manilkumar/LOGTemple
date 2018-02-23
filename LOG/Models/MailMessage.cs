using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mime;
using System.Net.Mail;
using System.IO;

namespace LOG.Models
{
    public class GMailMessage
    {

        private MemoryStream stream;
        private string filename;
        private string mediaType;

        #region Properties
        /// <summary>
        /// Gets the data stream for this attachment
        /// </summary>
        public Stream Data { get { return stream; } }
        /// <summary>
        /// Gets the original filename for this attachment
        /// </summary>
        public string Filename { get { return filename; } }
        /// <summary>
        /// Gets the attachment type: Bytes or String
        /// </summary>
        public string MediaType { get { return mediaType; } }
        /// <summary>
        /// Gets the file for this attachment (as a new attachment)
        /// </summary>
        public Attachment File { get { return new Attachment(Data, Filename, MediaType); } }
        #endregion
       
        public string TO { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

       
    }
}
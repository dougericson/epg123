﻿using System.Xml.Serialization;

namespace epg123.MxfXml
{
    public class MxfProvider
    {
        [XmlIgnore]
        public int index;

        /// <summary>
        /// Provides information and copyright about who provided the listing data.
        /// </summary>
        public MxfProvider() { }

        /// <summary>
        /// An ID that is unique to the document and defines this element.
        /// </summary>
        [XmlAttribute("id")]
        public string Id
        {
            get
            {
                return ("provider" + index.ToString());
            }
            set { }
        }

        /// <summary>
        /// The name of the supplier of the listings.
        /// The maximum length is 255 characters.
        /// </summary>
        [XmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>
        /// More information about the supplier of the listings.
        /// The maximum length is 255 characters.
        /// </summary>
        [XmlAttribute("displayName")]
        public string DisplayName { get; set; }

        /// <summary>
        /// The copyright notice from the supplier of the listings.
        /// The maximum length is 1024 characters.
        /// </summary>
        [XmlAttribute("copyright")]
        public string Copyright { get; set; }

        /// <summary>
        /// The status of the EPG123 MXF file creation.
        /// </summary>
        [XmlAttribute("status")]
        public string Status { get; set; }
    }
}

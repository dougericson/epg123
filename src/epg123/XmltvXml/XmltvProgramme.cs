﻿using System.Collections.Generic;
using System.Xml.Serialization;

namespace epg123.XmltvXml
{
    /// <summary>
    /// <!ELEMENT programme(title+, sub-title*, desc*, credits?, date?,
    ///                     category*, keyword*, language?, orig-language?,
    ///                     length?, icon*, url*, country*, episode-num*,
    ///                     video?, audio?, previously-shown?, premiere?,
    ///                     last-chance?, new?, subtitles*, rating*,
    ///                     star-rating*, review* )>
    /// <!ATTLIST programme start CDATA #REQUIRED
    ///                     stop CDATA #IMPLIED
    ///                     pdc-start CDATA #IMPLIED
    ///                     vps-start CDATA #IMPLIED
    ///                     showview CDATA #IMPLIED
    ///                     videoplus CDATA #IMPLIED
    ///                     channel CDATA #REQUIRED
    ///                     clumpidx CDATA "0/1" >
    /// </summary>
    public class XmltvProgramme
    {
        [XmlAttribute("start")]
        public string Start { get; set; }

        [XmlAttribute("stop")]
        public string Stop { get; set; }

        [XmlAttribute("pdc-start")]
        public string PdcStart { get; set; }

        [XmlAttribute("vps-start")]
        public string VpsStart { get; set; }

        [XmlAttribute("showview")]
        public string ShowView { get; set; }

        [XmlAttribute("videoplus")]
        public string VideoPlus { get; set; }

        [XmlAttribute("channel")]
        public string Channel { get; set; }

        [XmlAttribute("clumpidx")]
        public string ClumpIdx { get; set; }

        [XmlElement("title")]
        public List<XmltvText> Titles { get; set; }

        [XmlElement("sub-title")]
        public List<XmltvText> SubTitles { get; set; }

        [XmlElement("desc")]
        public List<XmltvText> Descriptions { get; set; }

        [XmlElement("credits")]
        public XmltvCredit Credits { get; set; }

        [XmlElement("date")]
        public string Date { get; set; }

        [XmlElement("category")]
        public List<XmltvText> Categories { get; set; }

        [XmlElement("keyword")]
        public List<XmltvText> Keywords { get; set; }

        [XmlElement("language")]
        public XmltvText Language { get; set; }

        [XmlElement("orig-language")]
        public XmltvText OrigLanguage { get; set; }

        [XmlElement("length")]
        public XmltvLength Length { get; set; }

        [XmlElement("icon")]
        public List<XmltvIcon> Icons { get; set; }

        [XmlElement("url")]
        public List<string> Urls { get; set; }

        [XmlElement("country")]
        public List<XmltvText> Countries { get; set; }

        [XmlElement("episode-num")]
        public List<XmltvEpisodeNum> EpisodeNums { get; set; }

        [XmlElement("video")]
        public XmltvVideo Video { get; set; }

        [XmlElement("audio")]
        public XmltvAudio Audio { get; set; }

        [XmlElement("previously-shown")]
        public XmltvPreviouslyShown PreviouslyShown { get; set; }

        [XmlElement("premiere")]
        public XmltvText Premiere { get; set; }

        [XmlElement("last-chance")]
        public XmltvText LastChance { get; set; }

        [XmlElement("new")]
        public string New { get; set; }

        [XmlElement("live")]
        public string Live { get; set; }

        [XmlElement("subtitles")]
        public List<XmltvSubtitles> Subtitles { get; set; }

        [XmlElement("rating")]
        public List<XmltvRating> Rating { get; set; }

        [XmlElement("star-rating")]
        public List<XmltvRating> StarRating { get; set; }

        [XmlElement("review")]
        public List<XmltvReview> Review { get; set; }
    }
}
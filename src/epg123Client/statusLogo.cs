﻿using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Win32;
using epg123Client;

namespace epg123
{
    public static class statusLogo
    {
        public enum EPG123STATUS
        {
            SUCCESS = 0,
            UPDATEAVAIL = 1,
            WARNING = 0xBAD1,
            ERROR = 0xDEAD,
            OTHERMXF = 0xFFFF
        }

        public static string mxfFile { get; set; }
        public static bool updateAvailable { get; set; }
        private static EPG123STATUS mxfFileStatus
        {
            get
            {
                // if mxf file doesn't exist, automatically an error
                if (string.IsNullOrEmpty(mxfFile) || !File.Exists(mxfFile))
                {
                    return EPG123STATUS.ERROR;
                }
                else
                {
                    // look at the status of the mxf file generated for warnings
                    XDocument xdoc = null;
                    using (XmlReader reader = XmlReader.Create(mxfFile))
                    {
                        reader.MoveToContent();
                        while (reader.Read())
                        {
                            if (reader.Name == "Providers")
                            {
                                xdoc = XDocument.Load(reader.ReadSubtree());
                                break;
                            }
                        }
                    }

                    if (xdoc != null)
                    {
                        var provider = xdoc.Descendants()
                            .Where(arg => arg.Name.LocalName == "Provider")
                            .Where(arg => arg.Attribute("name") != null)
                            .Where(arg => arg.Attribute("name").Value == "EPG123")
                            .SingleOrDefault();
                        if (provider != null)
                        {
                            // look at the age of the file to determine if there should be a warning
                            FileInfo fi = new FileInfo(mxfFile);
                            TimeSpan mxfFileAge = DateTime.UtcNow - fi.LastWriteTimeUtc;
                            if (DateTime.UtcNow - fi.LastWriteTimeUtc > TimeSpan.FromHours(24.0))
                            {
                                Logger.WriteError(string.Format("The MXF file imported is {0:N2} hours old.", mxfFileAge.TotalHours));
                                return EPG123STATUS.ERROR;
                            }

                            // determine the update available flag
                            if (provider.Attribute("displayName") != null)
                            {
                                updateAvailable = provider.Attribute("displayName").Value.Contains("Available");
                            }

                            // read the epg123 status
                            if (provider.Attribute("status") != null)
                            {
                                EPG123STATUS ret = (EPG123STATUS)(int.Parse(provider.Attribute("status").Value));
                                switch (ret)
                                {
                                    case EPG123STATUS.WARNING:
                                        Logger.WriteWarning("The imported MXF file contained a WARNING in its status field.");
                                        break;
                                    case EPG123STATUS.ERROR:
                                        Logger.WriteError("The imported MXF file contained an ERROR in its status field.");
                                        break;
                                    default:
                                        break;
                                }
                                return ret;
                            }
                            else
                            {
                                return EPG123STATUS.SUCCESS;
                            }
                        }
                        else
                        {
                            return EPG123STATUS.OTHERMXF;
                        }
                    }
                }
                return EPG123STATUS.OTHERMXF;
            }
        }

        public static void statusImage()
        {
            // don't update the status logo if the imported mxf was not from epg123
            int filestatus = (int)mxfFileStatus;
            if ((EPG123STATUS)filestatus == EPG123STATUS.OTHERMXF) return;

            // determine overall status
            EPG123STATUS status = (EPG123STATUS)(Math.Max(Logger.eventID, filestatus));

            // select base image based on status code
            double opacity = 1.0;
            string accent = "Light";
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Media Center\\Start Menu", false))
            {
                try
                {
                    accent = (string)key.GetValue("OEMLogoAccent", "Light");
                    opacity = (int)key.GetValue("OEMLogoOpacity", 100) / 100.0;
                }
                catch { }
            }

            // set up the base image; default for brandlogo is light
            Bitmap baseImage;
            switch (accent)
            {
                case "Light":
                    switch (status)
                    {
                        case EPG123STATUS.SUCCESS:
                        case EPG123STATUS.UPDATEAVAIL:
                            baseImage = resImages.EPG123OKLight;
                            break;
                        case EPG123STATUS.WARNING:
                            baseImage = resImages.EPG123WarningLight;
                            break;
                        case EPG123STATUS.ERROR:
                        default:
                            baseImage = resImages.EPG123ErrorLight;
                            break;
                    }
                    break;
                case "Dark":
                    switch (status)
                    {
                        case EPG123STATUS.SUCCESS:
                        case EPG123STATUS.UPDATEAVAIL:
                            baseImage = resImages.EPG123OKDark;
                            break;
                        case EPG123STATUS.WARNING:
                            baseImage = resImages.EPG123WarningDark;
                            break;
                        case EPG123STATUS.ERROR:
                        default:
                            baseImage = resImages.EPG123ErrorDark;
                            break;
                    }
                    break;
                case "None":
                default:
                    return;
            }

            // make text color match logo color
            Color textColor = baseImage.GetPixel(45, 55);

            // prep for update symbol
            Bitmap updateImage = new Bitmap(1, 1);
            if (updateAvailable)
            {
                updateImage = resImages.updateAvailable;
            }

            // determine width of date text to add to bottom of image
            SizeF textSize;
            string text = string.Format("{0:d}", DateTime.Now);
            Font font = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Bold, GraphicsUnit.Point);
            using (Image img = new Bitmap(1, 1))
            {
                using (Graphics g = Graphics.FromImage(img))
                {
                    textSize = g.MeasureString(text, font);

                    // adjust for screen dpi
                    float scaleFactor = (g.DpiX / 96f);
                    if (scaleFactor != 1.0)
                    {
                        textSize.Width /= scaleFactor;
                        textSize.Height /= scaleFactor;
                    }
                }
            }

            // create new image with base image and date text
            Bitmap image = new Bitmap(200, 75);
            image.SetResolution(baseImage.HorizontalResolution, baseImage.VerticalResolution);
            using (Graphics g = Graphics.FromImage(image))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

                int updateImageWidth = (updateImage.Width == 1) ? 0 : updateImage.Width;

                g.DrawImage(updateImage, image.Width - updateImageWidth, 0);
                g.DrawImage(baseImage, image.Width - updateImageWidth - baseImage.Width, image.Height - 75);

                using (Brush textbrush = new SolidBrush(textColor))
                {
                    g.DrawString(text, font, textbrush, image.Width - updateImageWidth - textSize.Width + 2, image.Height - textSize.Height + 2);
                }
            }

            // adjust alpha channel as needed
            if (opacity < 1.0)
            {
                adjustImageOpacity(image, opacity);
            }

            // save status image
            image.Save(Helper.Epg123StatusLogoPath);

            // ensure OEMLogoUri is pointed to the file
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Media Center\\Start Menu", true))
            {
                try
                {
                    key.SetValue("OEMLogoUri", "file://" + Helper.Epg123StatusLogoPath);
                }
                catch { }
            }

            return;
        }

        public static void adjustImageOpacity(Bitmap image, double opacity)
        {
            try
            {
                // lock bitmap and return bitmap data with 32-bits per pixel color (ARGB)
                BitmapData bitmapData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

                // create byte array to copy pixel values
                int step = 32 / 8;
                byte[] pixels = new byte[image.Width * image.Height * step];
                IntPtr ptr = bitmapData.Scan0;

                // copy data from pointer to array
                Marshal.Copy(ptr, pixels, 0, pixels.Length);

                // adjust alpha channel
                for (int counter = step - 1; counter < pixels.Length; counter += step)
                {
                    pixels[counter] = (byte)(pixels[counter] * opacity);
                }

                // copy array back into bitmap
                Marshal.Copy(pixels, 0, ptr, pixels.Length);

                // unlock bitmap
                image.UnlockBits(bitmapData);
            }
            catch { }
        }
    }
}

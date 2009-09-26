/*
 * 
 * Windows Live ID Membership Provider for SharePoint
 * originally developed for SharePointCommunity.se
 * http://www.sharepointcommunity.se/
 * ------------------------------------------
 * Copyright (c) 2009, Wictor Wilén
 * http://spwla.codeplex.com/
 * http://www.wictorwilen.se/
 * ------------------------------------------
 * Licensed under the Microsoft Public License (Ms-PL) 
 * http://www.opensource.org/licenses/ms-pl.html
 * 
 */

using System;
using System.Web;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint;
using System.Reflection;
using System.Globalization;

namespace SPCS.WindowsLiveAuth {

    public class LiveAuthImageHandler: IHttpHandler {

         const string gif = "R0lGODlhAQABAIAAAAAAAP///yH5BAEAAAEALAAAAAABAAEAAAIBTAA7";
        #region IHttpHandler Members

        public bool IsReusable {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            string uuid = context.Request["uuid"];
            if (string.IsNullOrEmpty(uuid))
            {
                emptyGif(context);
                return;
            }
            LiveCommunityUser lcu = LiveCommunityUser.GetUser(uuid);
            if (lcu == null)
            {
                emptyGif(context);
                return;
            }

            int size;
            if (!string.IsNullOrEmpty(context.Request.QueryString["s"]))
                size = int.Parse(context.Request.QueryString["s"], CultureInfo.CurrentCulture);
            else
                size = 100;

            using (MemoryStream stream = new MemoryStream())
            {
                lcu.GetImage(stream);

                if (stream == null || stream.Length == 0) {
                    Assembly asm = Assembly.GetExecutingAssembly();
                    using (Stream from = asm.GetManifestResourceStream("SPCS.WindowsLiveAuth.Resources.person.png")) {
                        int readCount;
                        byte[] buffer = new byte[1024];
                        while ((readCount = from.Read(buffer, 0, 1024)) != 0)
                            stream.Write(buffer, 0, readCount);
                    }
                }
                

                if (stream != null)
                {
                    if (stream.Length > 0)
                    {
                        context.Response.ContentType = "image/png";
                        context.Response.Cache.SetExpires(DateTime.Now.AddSeconds(60));
                        context.Response.Cache.SetCacheability(HttpCacheability.Public);

                        stream.Position = 0;
                        using (System.Drawing.Image image = System.Drawing.Image.FromStream(stream, true, true))
                        {
                            using (System.Drawing.Image thumbnail = new System.Drawing.Bitmap(size, size))
                            {
                                using (System.Drawing.Graphics gfx = System.Drawing.Graphics.FromImage(thumbnail))
                                {

                                    gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;
                                    gfx.SmoothingMode = SmoothingMode.HighQuality;
                                    gfx.PixelOffsetMode = PixelOffsetMode.HighQuality;
                                    gfx.CompositingQuality = CompositingQuality.HighQuality;

                                    gfx.DrawImage(image, 0, 0, size, size);
                                    using (MemoryStream ms = new MemoryStream()) {
                                        thumbnail.Save(ms, ImageFormat.Png);
                                        ms.WriteTo(context.Response.OutputStream);
                                        
                                    }
                                    


                                }
                            }
                        }
                        context.Response.End();
                        return;
                    }

                }
            }

        }

        private static void emptyGif(HttpContext context) {
            context.Response.ContentType = "image/gif";
            context.Response.BinaryWrite(Convert.FromBase64String(gif));
            context.Response.End();
        }
        #endregion
    }
}

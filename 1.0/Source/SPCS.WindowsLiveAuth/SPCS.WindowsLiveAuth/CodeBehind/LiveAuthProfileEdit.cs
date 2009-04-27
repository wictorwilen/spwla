﻿/*
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
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Web.UI.WebControls;
using System.Drawing.Drawing2D;
using System.IO;
using System.Drawing.Imaging;
using WindowsLive;

namespace SPCS.WindowsLiveAuth {
    public partial class LiveAuthProfileEdit: System.Web.UI.Page {

        protected string redir;

        protected InputFormTextBox tbDisplayName;
        protected InputFormTextBox tbEmail;
        protected InputFormTextBox tbAboutMe;
        protected InputFormTextBox tbDepartment;
        protected InputFormTextBox tbJobTitle;
        protected InputFormTextBox tbSipAddress;
        protected InputFormTextBox tbHomepage;
        protected InputFormTextBox tbTwitter;
        protected InputFormTextBox tbFeed;
        protected InputFormTextBox tbCity;
        protected InputFormTextBox tbCountry;
        protected FileUpload fileupload;
        protected Image userpicture;
        

        
        private void Page_Load(object sender, System.EventArgs e) {
            redir = Request.QueryString["Source"];

            if (string.IsNullOrEmpty(redir)) {
                redir = SPContext.Current.Web.Url;
            }

            if (User.Identity.IsAuthenticated) {
                if (!IsPostBack) {
                    string uuid = Request.QueryString["uuid"];
                    string id = Request.QueryString["id"];
                   
                    LiveCommunityUser lcu = LiveCommunityUser.GetUser(User.Identity.Name);
                                 
                    if (lcu != null) {
                        tbDisplayName.Text = lcu.DisplayName;
                        tbEmail.Text = lcu.Email;
                        tbAboutMe.Text = lcu.Description;
                        tbDepartment.Text = lcu.Company;
                        tbFeed.Text = lcu.FeedUrl;
                        tbHomepage.Text = lcu.HomePageUrl;
                        tbJobTitle.Text = lcu.Title;
                        //tbPicture.Text = lcu.ImageUrl;
                        if (string.IsNullOrEmpty(lcu.ImageUrl)) {
                            userpicture.Visible = false;
                        }
                        else {
                            userpicture.Visible = true;
                            userpicture.ImageUrl = string.Format("/_layouts/liveauth-image.ashx?uuid={0}&s=100", lcu.Id);
                        }
                        tbSipAddress.Text = lcu.SipAddress;
                        tbTwitter.Text = lcu.TwitterAccount;
                        tbCountry.Text = lcu.Country;
                        tbCity.Text = lcu.City;
                    }
                    else {
                        Response.Redirect(redir);
                    }
                }
            }
            else {
                Response.Redirect(redir);
            }
        }
        public const int size = 400;
        public void Submit_Click(Object sender, EventArgs e) {
            if (User.Identity.IsAuthenticated) {
                LiveCommunityUser lcu = LiveCommunityUser.GetUser(User.Identity.Name);
                // upload the file
                if (fileupload.HasFile) {

                    try {
                        // Convert it to 400x400 png image
                        using (System.Drawing.Image image = System.Drawing.Image.FromStream(fileupload.FileContent, true, true)) {
                            using (System.Drawing.Image thumbnail = new System.Drawing.Bitmap(size, size)) {
                                using (System.Drawing.Graphics gfx = System.Drawing.Graphics.FromImage(thumbnail)) {
                                
                                    gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;
                                    gfx.SmoothingMode = SmoothingMode.HighQuality;
                                    gfx.PixelOffsetMode = PixelOffsetMode.HighQuality;
                                    gfx.CompositingQuality = CompositingQuality.HighQuality;

                                    gfx.DrawImage(image, 0, 0, size,size);
                                    
                                    MemoryStream ms = new MemoryStream();
                                    
                                    thumbnail.Save(ms, ImageFormat.Png);
                                    
                                    lcu.SetImage(ms.GetBuffer());
                                
                                }
                            }
                        }
                    }
                    catch (Exception ex) {
                        
                    }
                }
               
                // update the user
                
                lcu.DisplayName = tbDisplayName.Text;
                lcu.Email = tbEmail.Text;
                lcu.Description = tbAboutMe.Text;
                lcu.Company = tbDepartment.Text;
                lcu.FeedUrl = tbFeed.Text;
                lcu.HomePageUrl = tbHomepage.Text;
                lcu.Title = tbJobTitle.Text;
                lcu.ImageUrl = string.Format("/_layouts/liveauth-image.ashx?uuid={0}&s=50", lcu.Id);
                //lcu.ImageUrl = tbPicture.Text;
                lcu.SipAddress = tbSipAddress.Text;
                lcu.TwitterAccount = tbTwitter.Text;
                lcu.City = tbCity.Text;
                lcu.Country = tbCountry.Text;
                lcu.Update();

                // Make sure that the user exists in SharePoint also
                // EventHandler will then update the user info after lcu.Update
                SPSecurity.RunWithElevatedPrivileges(delegate() {
                    using (SPSite site = new SPSite(SPContext.Current.Web.Url)) {
                        using (SPWeb web = site.RootWeb) {
                            web.AllowUnsafeUpdates = true;
                            SPUser user = web.EnsureUser(lcu.Id);
                            user.Name = tbDisplayName.Text;
                            user.Update();
                        }
                    }
                });

                Response.Redirect(redir);
            }
        }



        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e) {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.Load += new EventHandler(Page_Load);
        }
        #endregion
    }
}

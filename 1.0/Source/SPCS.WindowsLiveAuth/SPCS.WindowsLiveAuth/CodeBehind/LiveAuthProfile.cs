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
using System.Web.UI;
using WindowsLive;
using SPExLib.General;
using SPExLib.SharePoint;
using System.Globalization;

namespace SPCS.WindowsLiveAuth {
    public class LiveAuthProfile: System.Web.UI.Page {

        protected string redir;
        protected Label LabelTitle;
        protected Label lblDisplayName;
        protected HyperLink hlEmail;
        protected Image imImage;
        protected Label lbAbout;
        protected Label lbCompany;
        protected Label lbTitle;
        protected Label lbCountry;
        protected Label lbCity;
        protected HyperLink hlTwitter;
        protected HyperLink hlWebsite;
        protected HyperLink hlFeed;
        protected ToolBarButton tbbEdit;



        protected HyperLink delauth;
        protected Image messengerstatus;




        private void Page_Load(object sender, EventArgs e) {

            if (string.IsNullOrEmpty(redir))
                redir = SPContext.Current.Web.Url;
            else
                redir = Request.QueryString["Source"];

            LiveAuthConfiguration settings = LiveAuthConfiguration.GetSettings(SPContext.Current.Site.WebApplication);
            



            if (User.Identity.IsAuthenticated) {
                if (!IsPostBack) {
                    string uuid = Request.QueryString["uuid"];
                    string id = Request.QueryString["id"];
                    LiveCommunityUser lcu;
                    LiveCommunityUser currentUser = LiveCommunityUser.GetUser(User.Identity.Name);


                    if (!string.IsNullOrEmpty(uuid)) {
                        lcu = LiveCommunityUser.GetUser(uuid);
                    }
                    else if (!string.IsNullOrEmpty(id)) {
                        SPUser spUser = SPContext.Current.Web.AllUsers.GetByID(int.Parse(id, CultureInfo.CurrentCulture));
                        lcu = LiveCommunityUser.GetUser(spUser.GetFormsLoginName());
                    }
                    else {
                        lcu = currentUser;
                    }
                    if (lcu == null || (lcu != null && lcu.Id != currentUser.Id)) {
                        tbbEdit.Visible = false;
                    }
                    if (lcu != null) {
                        LabelTitle.Text = lcu.DisplayName;
                        lblDisplayName.Text = lcu.DisplayName;
                        hlEmail.NavigateUrl = "mailto:{0}".FormatWith(lcu.Email);
                        hlEmail.Text = lcu.Email;
                        lbAbout.Controls.Add(new LiteralControl(lcu.Description));
                        imImage.ImageUrl = "/_layouts/liveauth-image.ashx?uuid={0}&s=60".FormatWith(lcu.Id);
                        lbCompany.Text = lcu.Company;
                        lbTitle.Text = lcu.Title;
                        if (!string.IsNullOrEmpty(lcu.FeedUrl)) {
                            hlFeed.NavigateUrl = lcu.FeedUrl;
                            hlFeed.Text = lcu.FeedUrl;
                        }
                        if (!string.IsNullOrEmpty(lcu.TwitterAccount)) {
                            hlTwitter.NavigateUrl = "http://twitter.com/{0}".FormatWith( lcu.TwitterAccount);
                            hlTwitter.Text = "http://twitter.com/{0}".FormatWith(lcu.TwitterAccount);
                        }
                        if (!string.IsNullOrEmpty(lcu.HomePageUrl)) {
                            hlWebsite.NavigateUrl = lcu.HomePageUrl;
                            hlWebsite.Text = lcu.HomePageUrl;
                        }
                        lbCountry.Text = lcu.Country;
                        lbCity.Text = lcu.City;
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
        private const int size = 400;
        public void Close_Click(Object sender, EventArgs e) {
                Response.Redirect(redir);            
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

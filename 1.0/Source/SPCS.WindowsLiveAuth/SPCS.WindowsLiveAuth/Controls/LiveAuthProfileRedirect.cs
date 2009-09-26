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
using Microsoft.SharePoint.WebControls;
using System.Web;
using Microsoft.SharePoint;
using System;
using SPExLib.General;
using SPExLib.SharePoint;
using System.Globalization;

namespace SPCS.WindowsLiveAuth {
    public class LiveAuthProfileRedirect: SPControl {
        public LiveAuthProfileRedirect() {
            
        }
        protected override void Render(System.Web.UI.HtmlTextWriter writer) {
            // redirect if WLA installed and WLA account and if Forms auth is used on the site
            if (SPSecurity.AuthenticationMode == System.Web.Configuration.AuthenticationMode.Forms) {
                LiveAuthConfiguration settings = LiveAuthConfiguration.GetSettings(SPContext.Current.Site.WebApplication);
                if (settings != null) {
                    SPUser spUser = null;
                    if (string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["id"])) {
                        spUser = SPContext.Current.Web.CurrentUser;
                    }
                    else {
                        spUser = SPContext.Current.Web.AllUsers.GetByID(int.Parse(HttpContext.Current.Request.QueryString["id"], CultureInfo.CurrentCulture));    
                    }
                    
                    LiveCommunityUser lcu = LiveCommunityUser.GetUser(spUser.GetFormsLoginName());
                    if (lcu != null) {
                        HttpContext.Current.Response.Redirect(String.Format("{0}/_layouts/liveauth-profile.aspx?id={1}", SPContext.Current.Web.Url, HttpContext.Current.Request.QueryString["ID"]), true);
                    }
                }
            }        
        }
    }
}

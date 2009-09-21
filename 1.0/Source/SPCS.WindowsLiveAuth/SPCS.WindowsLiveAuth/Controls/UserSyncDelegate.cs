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

namespace SPCS.WindowsLiveAuth {
    public sealed class UserSyncDelegate: SPControl {
        /// <summary>
        /// Initializes a new instance of the UserSyncDelegate class.
        /// </summary>
        public UserSyncDelegate() {
        }

        /// <summary>
        /// Override the OnInit, so this executes the Welcome ctrl OnLoad
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e) {
            // only run it on the LiveID web app
            if (SPSecurity.AuthenticationMode == System.Web.Configuration.AuthenticationMode.Forms) {
                // ...and only when SPWLA is configured
                LiveAuthConfiguration settings = LiveAuthConfiguration.GetSettings(SPContext.Current.Site.WebApplication);
                if (settings != null) {
                    SPUser spUser = SPContext.Current.Web.CurrentUser;
                    // ... and only if the user is logged in
                    if (spUser != null) {
                        // ... and only if the user name is the same as the loginname
                        if (spUser.Name == spUser.GetFormsLoginName()) {
                            // ... make sure the user exists
                            SPContext.Current.Web.EnsureUser(spUser.LoginName);
                            LiveCommunityUser lcu = LiveCommunityUser.GetUser(spUser.GetFormsLoginName());
                            if (lcu != null) {
                                lcu.PushProfile();
                                // now we have to redirect the user, since the welcome ctrl gets the username from SPContext, sux!
                                HttpContext.Current.Response.Redirect(HttpContext.Current.Request.Url.ToString(), true);
                            }
                            else {
                                // user not found, redirect to register page
                                // TODO:
                            }                            
                        }    
                    }
                }
            }
        }
    }
}

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
using SPExLib.SharePoint;
using System.Security.Principal;

namespace SPCS.WindowsLiveAuth {
    public class UserSyncDelegate : SPControl {

        /// <summary>
        /// Override the OnInit, so this executes the Welcome ctrl OnLoad
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e) {
            try {

                // only run it on the LiveID web app
#if DEBUG
                status = 1;
                dbg = SPSecurity.AuthenticationMode.ToString();
#endif
                if (SPSecurity.AuthenticationMode == System.Web.Configuration.AuthenticationMode.Forms) {
#if DEBUG
                    SPSecurity.RunWithElevatedPrivileges(() => {
                        status = 2;
#endif                // ...and only when SPWLA is configured
                        LiveAuthConfiguration settings = LiveAuthConfiguration.GetSettings(SPContext.Current.Site.WebApplication);
                        if (settings != null) {
#if DEBUG
                            status = 3;
#endif
                            SPUser spUser = SPContext.Current.Web.CurrentUser;
                            if (spUser == null) {
                                IPrincipal principal = HttpContext.Current.User;
                                if (principal != null) {
                                    if (principal.Identity.IsAuthenticated) {
                                        status = 10;
                                        SPSecurity.RunWithElevatedPrivileges(() => {
                                            SPContext.Current.Web.EnsureUser("liveid:" + principal.Identity.Name);
                                            LiveCommunityUser lcu = LiveCommunityUser.GetUser(principal.Identity.Name);
                                            if (lcu != null) {
#if DEBUG
                                                status = 17;
#endif
                                                lcu.PushProfile();
                                                // now we have to redirect the user, since the welcome ctrl gets the username from SPContext, sux!
                                                HttpContext.Current.Response.Redirect(HttpContext.Current.Request.Url.ToString(), true);
                                            }
                                            else {
#if DEBUG
                                                status = 16;
#endif
                                                // user not found, redirect to register page
                                                // TODO:
                                            }
                                        });
                                    }
                                    else {
                                        status = 11;
                                    }
                                }
                            }
                            // ... and only if the user is logged in
                            if (spUser != null) {
#if DEBUG
                                status = 4;
#endif
                                // ... and only if the user name is the same as the loginname
                                if (spUser.Name == spUser.GetFormsLoginName()) {
#if DEBUG
                                    status = 5;
#endif
                                    // ... make sure the user exists
                                    SPContext.Current.Web.EnsureUser(spUser.LoginName);
                                    LiveCommunityUser lcu = LiveCommunityUser.GetUser(spUser.GetFormsLoginName());
                                    if (lcu != null) {
#if DEBUG
                                        status = 7;
#endif
                                        lcu.PushProfile();
                                        // now we have to redirect the user, since the welcome ctrl gets the username from SPContext, sux!
                                        HttpContext.Current.Response.Redirect(HttpContext.Current.Request.Url.ToString(), true);
                                    }
                                    else {
#if DEBUG
                                        status = 6;
#endif
                                        // user not found, redirect to register page
                                        // TODO:
                                    }
                                }
                            }
                        }
                    });
                }

            }
            catch (Exception ex) {
                dbg += ex.ToString();                
            }
        }
#if DEBUG
        int status;
        string dbg = string.Empty;
        protected override void Render(System.Web.UI.HtmlTextWriter writer) {
            writer.Write("<!-- UserSyncDelegate: {0}, {1} -->", status, dbg);
        }
#endif
    }
}

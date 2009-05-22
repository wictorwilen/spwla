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
using Microsoft.SharePoint;
using SPExLib.Extensions;

namespace SPCS.WindowsLiveAuth {
    class ProfileListEventHandler : SPItemEventReceiver {
        public override void ItemAdded(SPItemEventProperties properties) {

            string account = properties.ListItem["UUID"].ToString();
            SPWeb spWeb = properties.OpenWeb();
            LiveCommunityUser lcu = LiveCommunityUser.GetUser(account, spWeb);
            if (lcu != null) {
                // Create profile announcement
                string title = string.Format("New member - {0}", lcu.DisplayName);
                string body = string.Format("<b><a href='/_layouts/liveauth-profile.aspx?uuid={0}'>{1}</a></b> is now a member.", lcu.Id, lcu.DisplayName);
                string image = string.Format("/_layouts/liveauth-image.ashx?uuid={0}&s=40", lcu.Id);
                createAnnouncement(spWeb, title, body, image);
            }

        }

        public override void ItemUpdated(SPItemEventProperties properties) {
            string account = properties.ListItem["UUID"].ToString();
            SPWeb spWeb = properties.OpenWeb();
            LiveCommunityUser lcu = LiveCommunityUser.GetUser(account, spWeb);

            if (lcu != null) {
                lcu.PushProfile(properties.OpenWeb());


                DateTime? ll = DateTimeExtensions.ParseNull(properties.AfterProperties["LastLogin"]);
                if(ll != null && ll.Value.AddSeconds(5) < DateTime.Now) {                
                    // Create profile announcement, but not when the user logs in
                    return;
                }
                string title = string.Format("Profile updated for {0}", lcu.DisplayName);
                string body = string.Format("The profile for <b><a href='/_layouts/liveauth-profile.aspx?uuid={0}'>{1}</a></b> is updated", lcu.Id, lcu.DisplayName);
                string image = string.Format("/_layouts/liveauth-image.ashx?uuid={0}&s=40", lcu.Id);
                createAnnouncement(spWeb, title, body, image);
            }

            
        }
        static void createAnnouncement(SPWeb spWeb, string title, string body, string image) {
            LiveAuthConfiguration settings = LiveAuthConfiguration.GetSettings(spWeb.Site.WebApplication);
            if (settings != null) {
                using (SPSite site = new SPSite(settings.ProfileSiteUrl)) {
                    using (SPWeb web = site.OpenWeb()) {
                        if (web.ListExists(settings.SiteSyncListName)) {
                            SPList list = web.Lists[settings.SiteSyncListName];
                            foreach (SPListItem item in list.Items) {
                                using (SPSite site2 = new SPSite(item.GetListItemString("Url"))) {
                                    if (site2.ID == spWeb.Site.ID) { 
                                        if (item.GetListItemBool("EnableAnnouncements") && item.GetListItemBool("ProfileUpdateInfo")) {                                    
                                            using (SPWeb web2 = site2.OpenWeb()) {

                                                SPList announcements = web2.Lists[item.GetListItemString("AnnouncementList")];
                                                SPListItem announcement = announcements.Items.Add();
                                                announcement["Title"] = title;
                                                announcement["Body"] = body;
                                                if (announcement.Contains("Image")) {
                                                    announcement["Image"] = image;
                                                }
                                                announcement["Expires"] = DateTime.Now.AddDays(14);
                                                announcement.Update();
                                            }
                                        }
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    

}

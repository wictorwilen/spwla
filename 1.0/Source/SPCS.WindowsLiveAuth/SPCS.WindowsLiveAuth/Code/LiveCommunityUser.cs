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
using System.Collections.Generic;
using WindowsLive;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using System.IO;
using System.Web;

namespace SPCS.WindowsLiveAuth {
    public class LiveCommunityUser {
        private LiveCommunityUser(SPListItem listItem) {

            this.Id = listItem.GetListItemString("UUID");
            this.DisplayName = listItem.GetListItemString("DisplayName");
            this.Email = listItem.GetListItemString("Email");
            this.Description = listItem.GetListItemString("Description");
            this.Title = listItem.GetListItemString("JobTitle");
            this.Company = listItem.GetListItemString("Company");
            this.SipAddress = listItem.GetListItemString("SipAddress");
            this.Locked = listItem.GetListItemBool("Locked");
            this.Approved = listItem.GetListItemBool("Approved");
            this.HomePageUrl = listItem.GetListItemString("HomePageUrl");
            this.TwitterAccount = listItem.GetListItemString("TwitterAccount");
            this.FeedUrl = listItem.GetListItemString("FeedUrl");
            this.ImageUrl = listItem.GetListItemString("ImageUrl");
            this.Created = listItem.GetListItemDateTime("Created");
            this.LastLogin = listItem.GetListItemDateTime("LastLogin");
            this.LockedOn = listItem.GetListItemDateTime("LockedOn");
            this.City = listItem.GetListItemString("City");
            this.Country = listItem.GetListItemString("Country");
            this.ConsentToken = listItem.GetListItemString("ConsentToken");
            this.CID = listItem.GetListItemString("CID");
            // TODO


        }
        public static LiveCommunityUser GetUser(string id)      {
            return GetUser(id, null);

        }

        public static LiveCommunityUser GetUser(WindowsLiveLogin.User user) {
            return GetUser(user.Id);
        }
        public static LiveCommunityUser GetUser(string id, SPWeb sourceWeb) {
            if (string.IsNullOrEmpty(id)) {
                throw new ArgumentException("Invalid argument");
            }
            LiveCommunityUser lcu = null;

            SPWebApplication webApp = null;
            //if (sourceWeb == null) {
            //    webApp = SPContext.Current.Site.WebApplication;
            //}
            //else {
            //    webApp = sourceWeb.Site.WebApplication;
            //}

            //try {
            //    webApp = SPContext.Current.Site.WebApplication;
            //}
            //catch (InvalidOperationException) {
            //    using (SPSite site = new SPSite(HttpContext.Current.Request.Url.ToString())) {
            //        webApp = site.WebApplication;
            //    }
            //}
            if (sourceWeb == null) {
                SPSecurity.RunWithElevatedPrivileges(() => {
                    // get it using elev privs, if we dont do this then we
                    // get an infinite loop since it checks the roles of the current user
                    using (SPSite site = new SPSite(HttpContext.Current.Request.Url.ToString())) {
                        webApp = site.WebApplication;
                    }
                });

            }
            else {
                webApp = sourceWeb.Site.WebApplication;
            }
            LiveAuthConfiguration settings = LiveAuthConfiguration.GetSettings(webApp);

            SPSecurity.RunWithElevatedPrivileges(() => {
                using (SPSite site = new SPSite(settings.ProfileSiteUrl)) {
                    using (SPWeb web = site.OpenWeb()) {
                        SPList list = web.Lists[settings.ProfileListName];
                        SPQuery query = new SPQuery();
                        if (id.IndexOf("@") != -1)
                            query.Query = string.Format("<Where><Eq><FieldRef Name='Email'/><Value Type='Text'>{0}</Value></Eq></Where>", id);
                        else
                            query.Query = string.Format("<Where><Eq><FieldRef Name='UUID'/><Value Type='Text'>{0}</Value></Eq></Where>", id);
                        SPListItemCollection items = list.GetItems(query);
                        if (items.Count != 0)
                            lcu = new LiveCommunityUser(items[0]);
                    }
                }
            });
            return lcu;
        }
        public static bool CreateUser(string id, string email) {
            if (string.IsNullOrEmpty(id)) {
                throw new ArgumentException("Invalid id argument");
            }

            // Check if user exists
            LiveCommunityUser lcu = LiveCommunityUser.GetUser(id);
            if (lcu != null) {
                return false;
            }
            bool retVal = false;
            LiveAuthConfiguration settings = LiveAuthConfiguration.GetSettings(SPContext.Current.Site.WebApplication);
            SPSecurity.RunWithElevatedPrivileges(() => {
                using (SPSite site = new SPSite(settings.ProfileSiteUrl)) {
                    using (SPWeb web = site.OpenWeb()) {
                        web.AllowUnsafeUpdates = true;
                        SPList list = web.Lists[settings.ProfileListName];
                        if (checkUniqueEmail(list, email, id)) {
                            SPListItem item = list.Items.Add();
                            item["Email"] = email;
                            item["Title"] = email;
                            item["UUID"] = id;
                            item["Approved"] = settings.AutoApprove;
                            item.Update();
                            retVal = true;
                        }
                    }
                }
            });
            return retVal;

        }
        public static void CreateUser(WindowsLiveLogin.User user) {
            // Temporary create the user
            CreateUser(user.Id, string.Empty);

        }
        public static List<LiveCommunityUser> GetAllUsers() {
            List<LiveCommunityUser> users = new List<LiveCommunityUser>();
            LiveAuthConfiguration settings = LiveAuthConfiguration.GetSettings(SPContext.Current.Site.WebApplication);

            SPSecurity.RunWithElevatedPrivileges(() => {
                using (SPSite site = new SPSite(settings.ProfileSiteUrl)) {
                    using (SPWeb web = site.OpenWeb()) {
                        SPList list = web.Lists[settings.ProfileListName];
                        foreach (SPListItem item in list.Items)
                            users.Add(new LiveCommunityUser(item));
                    }
                }
            });
            return users;
        }
        public static bool DeleteUser(string id) {
            if (string.IsNullOrEmpty(id)) {
                throw new ArgumentException("Invalid username");
            }
            LiveAuthConfiguration settings = LiveAuthConfiguration.GetSettings(SPContext.Current.Site.WebApplication);

            SPSecurity.RunWithElevatedPrivileges(() => {
                using (SPSite site = new SPSite(settings.ProfileSiteUrl)) {
                    using (SPWeb web = site.OpenWeb()) {
                        SPList list = web.Lists[settings.ProfileListName];
                        SPQuery query = new SPQuery { Query = string.Format("<Where><Eq><FieldRef Name='UUID'/><Value Type='Text'>{0}</Value></Eq></Where>", id) };
                        SPListItemCollection items = list.GetItems(query);
                        if (items.Count != 0)
                            items[0].Delete();
                    }
                }
            });
            return true;

        }
        public static List<LiveCommunityUser> FindUsers(string field, string value) {

            List<LiveCommunityUser> users = new List<LiveCommunityUser>();

            LiveAuthConfiguration settings = LiveAuthConfiguration.GetSettings(SPContext.Current.Site.WebApplication);

            SPSecurity.RunWithElevatedPrivileges(() => {
                using (SPSite site = new SPSite(settings.ProfileSiteUrl)) {
                    using (SPWeb web = site.OpenWeb()) {
                        SPList list = web.Lists[settings.ProfileListName];
                        SPQuery query = new SPQuery { Query = string.Format("<Where><BeginsWith><FieldRef Name='{0}'/><Value Type='Text'>{1}</Value></BeginsWith></Where>", field, value) };
                        SPListItemCollection items = list.GetItems(query);
                        foreach (SPListItem item in items)
                            users.Add(new LiveCommunityUser(item));
                    }
                }
            });
            return users;
        }
        public static bool Unlock(string id) {
            if (string.IsNullOrEmpty(id)) {
                throw new ArgumentException("Invalid username");
            }

            LiveAuthConfiguration settings = LiveAuthConfiguration.GetSettings(SPContext.Current.Site.WebApplication);

            SPSecurity.RunWithElevatedPrivileges(() => {
                using (SPSite site = new SPSite(settings.ProfileSiteUrl)) {
                    using (SPWeb web = site.OpenWeb()) {
                        SPList list = web.Lists[settings.ProfileListName];
                        SPQuery query = new SPQuery { Query = string.Format("<Where><Eq><FieldRef Name='UUID'/><Value Type='Text'>{0}</Value></Eq></Where>", id) };
                        SPListItemCollection items = list.GetItems(query);
                        if (items.Count != 0) {
                            items[0]["Locked"] = false;
                            items[0].Update();
                        }
                    }
                }
            });
            return true;
        }
        public static bool Lock(string id) {
            if (string.IsNullOrEmpty(id)) {
                throw new ArgumentException("Invalid username");
            }

            LiveAuthConfiguration settings = LiveAuthConfiguration.GetSettings(SPContext.Current.Site.WebApplication);

            SPSecurity.RunWithElevatedPrivileges(() => {
                using (SPSite site = new SPSite(settings.ProfileSiteUrl)) {
                    using (SPWeb web = site.OpenWeb()) {
                        SPList list = web.Lists[settings.ProfileListName];
                        SPQuery query = new SPQuery { Query = string.Format("<Where><Eq><FieldRef Name='UUID'/><Value Type='Text'>{0}</Value></Eq></Where>", id) };
                        SPListItemCollection items = list.GetItems(query);
                        if (items.Count != 0) {
                            items[0]["Locked"] = true;
                            items[0].Update();
                        }
                    }
                }
            });
            return true;
        }

        public void LoggedIn() {
            LiveAuthConfiguration settings = LiveAuthConfiguration.GetSettings(SPContext.Current.Site.WebApplication);

            SPSecurity.RunWithElevatedPrivileges(() => {
                using (SPSite site = new SPSite(settings.ProfileSiteUrl)) {
                    using (SPWeb web = site.OpenWeb()) {
                        SPList list = web.Lists[settings.ProfileListName];
                        web.AllowUnsafeUpdates = true;
                        site.AllowUnsafeUpdates = true;
                        SPQuery query = new SPQuery { Query = string.Format("<Where><Eq><FieldRef Name='UUID'/><Value Type='Text'>{0}</Value></Eq></Where>", this.Id) };
                        SPListItemCollection items = list.GetItems(query);
                        if (items.Count != 0) {
                            SPListItem item = items[0];
                            item["LastLogin"] = DateTime.Now;
                            item.SystemUpdate();
                        }
                    }
                }
            });
        }
        public static void UpdateUser(LiveCommunityUser user) {
            if (user == null) {
                throw new ArgumentNullException("user");
            }

            LiveAuthConfiguration settings = LiveAuthConfiguration.GetSettings(SPContext.Current.Site.WebApplication);

            SPSecurity.RunWithElevatedPrivileges(() => {
                using (SPSite site = new SPSite(settings.ProfileSiteUrl)) {
                    using (SPWeb web = site.OpenWeb()) {
                        SPList list = web.Lists[settings.ProfileListName];
                        if (checkUniqueEmail(list, user.Email, user.Id)) {
                            web.AllowUnsafeUpdates = true;
                            site.AllowUnsafeUpdates = true;
                            SPQuery query = new SPQuery { Query = string.Format("<Where><Eq><FieldRef Name='UUID'/><Value Type='Text'>{0}</Value></Eq></Where>", user.Id) };
                            SPListItemCollection items = list.GetItems(query);
                            if (items.Count != 0) {
                                SPListItem item = items[0];
                                item["Title"] = user.Email;
                                item["DisplayName"] = user.DisplayName;
                                item["Email"] = user.Email;
                                item["Description"] = user.Description;
                                item["ImageUrl"] = user.ImageUrl;
                                item["Company"] = user.Company;
                                item["JobTitle"] = user.Title;
                                item["SipAddress"] = user.SipAddress;
                                item["Locked"] = user.Locked;
                                item["LockedOn"] = user.LockedOn;
                                item["Approved"] = user.Approved;
                                item["HomePageUrl"] = user.HomePageUrl;
                                item["TwitterAccount"] = user.TwitterAccount;
                                item["LastLogin"] = user.LastLogin;
                                item["LockedOn"] = user.LockedOn;
                                item["FeedUrl"] = user.FeedUrl;
                                item["Country"] = user.Country;
                                item["City"] = user.City;
                                item["CID"] = user.CID;
                                item.Update();
                            }
                        }
                        else
                            throw new ApplicationException("The e-mail entered is not valid");
                    }
                }
            });
        }

        public static void PushProfile(LiveCommunityUser user, SPWeb web) {

            LiveAuthConfiguration settings;
            if (web != null) {
                settings = LiveAuthConfiguration.GetSettings(web.Site.WebApplication);
            }
            else {
                settings = LiveAuthConfiguration.GetSettings(SPContext.Current.Site.WebApplication);
            }
            SPSecurity.RunWithElevatedPrivileges(() => {
                using (SPSite elevSite = new SPSite(settings.ProfileSiteUrl)) {
                    using (SPWeb elevWeb = elevSite.OpenWeb()) {
                        foreach (SPListItem item in elevWeb.Lists[settings.SiteSyncListName].Items)
                            using (SPSite site = new SPSite(item["Url"].ToString())) {
                                using (SPWeb sweb = site.OpenWeb()) {
                                    site.AllowUnsafeUpdates = true;
                                    sweb.AllowUnsafeUpdates = true;
                                    SPList list = sweb.Lists["User Information List"];
                                    SPQuery query = new SPQuery { Query = string.Format("<Where><Contains><FieldRef Name='Name'/><Value Type='Text'>{0}</Value></Contains></Where>", user.Id) };
                                    SPListItemCollection items = list.GetItems(query);
                                    if (items.Count > 0) {
                                        SPListItem uItem = items[0];
                                        uItem["Title"] = user.DisplayName;
                                        uItem["EMail"] = user.Email;
                                        uItem["Notes"] = user.Description;
                                        uItem["Picture"] = settings.Domain.TrimEnd(new char[] {'/'}) + user.ImageUrl;
                                        uItem["Department"] = user.Company;
                                        uItem["JobTitle"] = user.Title;
                                        uItem["SipAddress"] = user.SipAddress;
                                        uItem.Update();
                                    }
                                }
                            }
                    }
                }
            });

        }
        public void PushProfile() {
            PushProfile(this, null);
        }
        public void PushProfile(SPWeb web) {
            PushProfile(this, web);
        }

        public void Update() {
            UpdateUser(this);
        }

        private static bool checkUniqueEmail(SPList list, string email, string id) {
            // TODO
            return true;
        }
        public bool Locked {
            get;
            set;
        }
        public bool Approved {
            get;
            set;
        }
        public string Email {
            get;
            set;
        }
        public string DisplayName {
            get;
            set;
        }
        public string Id {
            get;
            private set;
        }
        public string Description {
            get;
            set;
        }
        public string HomePageUrl {
            get;
            set;
        }
        public string FeedUrl {
            get;
            set;
        }
        public string TwitterAccount {
            get;
            set;
        }
        public string ImageUrl {
            get;
            set;
        }
        public string Title {
            get;
            set;
        }
        public DateTime Created {
            get;
            private set;
        }
        public DateTime LastLogin {
            get;
            set;
        }


        public DateTime LockedOn {
            get;
            set;
        }
        public string SipAddress {
            get;
            set;
        }
        public string Company {
            get;
            set;
        }
        public string Country {
            get;
            set;
        }
        public string City {
            get;
            set;
        }

        internal string ConsentToken {
            get;
            set;
        }
        public string CID {
            get;
            internal set;
        }
        
        internal void UpdateConsentToken(string token) {
            LiveAuthConfiguration settings = LiveAuthConfiguration.GetSettings(SPContext.Current.Site.WebApplication);

            SPSecurity.RunWithElevatedPrivileges(() => {
                using (SPSite site = new SPSite(settings.ProfileSiteUrl)) {
                    using (SPWeb web = site.OpenWeb()) {
                        SPList list = web.Lists[settings.ProfileListName];
                        web.AllowUnsafeUpdates = true;
                        site.AllowUnsafeUpdates = true;
                        SPQuery query = new SPQuery { Query = string.Format("<Where><Eq><FieldRef Name='UUID'/><Value Type='Text'>{0}</Value></Eq></Where>", this.Id) };
                        SPListItemCollection items = list.GetItems(query);
                        if (items.Count != 0) {
                            SPListItem item = items[0];
                            this.ConsentToken = token;
                            item["ConsentToken"] = this.ConsentToken;
                            item.Update();
                        }
                    }
                }
            });
        }

        internal void SetImage(byte[] bytes) {
            LiveAuthConfiguration settings = LiveAuthConfiguration.GetSettings(SPContext.Current.Site.WebApplication);
            SPSecurity.RunWithElevatedPrivileges(() => {
                using (SPSite site = new SPSite(settings.ProfileSiteUrl)) {
                    using (SPWeb web = site.OpenWeb()) {
                        SPList list = web.Lists[settings.ProfileListName];
                        site.AllowUnsafeUpdates = true;
                        web.AllowUnsafeUpdates = true;
                        SPQuery query = new SPQuery { Query = string.Format("<Where><Eq><FieldRef Name='UUID'/><Value Type='Text'>{0}</Value></Eq></Where>", this.Id) };
                        SPListItemCollection items = list.GetItems(query);
                        if (items.Count > 0) {
                            SPListItem uItem = items[0];
                            try {
                                uItem.Attachments.DeleteNow("profileImage.png");
                            } catch (ArgumentOutOfRangeException) {
                            }
                            uItem.Attachments.AddNow("profileImage.png", bytes);
                        }
                    }
                }
            });

        }
        internal void GetImage(Stream stream) {
            LiveAuthConfiguration settings = LiveAuthConfiguration.GetSettings(SPContext.Current.Site.WebApplication);
            SPSecurity.RunWithElevatedPrivileges(() => {
                using (SPSite site = new SPSite(settings.ProfileSiteUrl)) {
                    using (SPWeb web = site.OpenWeb()) {
                        SPList list = web.Lists[settings.ProfileListName];
                        SPQuery query = new SPQuery { Query = string.Format("<Where><Eq><FieldRef Name='UUID'/><Value Type='Text'>{0}</Value></Eq></Where>", this.Id) };
                        SPListItemCollection items = list.GetItems(query);
                        if (items.Count > 0) {
                            SPListItem uItem = items[0];
                            foreach (string fileName in uItem.Attachments)
                                if (fileName == "profileImage.png") {
                                    SPFile file = uItem.ParentList.ParentWeb.GetFile(uItem.Attachments.UrlPrefix + fileName);
                                    using (BinaryReader from = new BinaryReader(file.OpenBinaryStream())) {
                                        int readCount;
                                        byte[] buffer = new byte[1024];
                                        while ((readCount = from.Read(buffer, 0, 1024)) != 0)
                                            stream.Write(buffer, 0, readCount);
                                    }
                                    break;
                                }
                        }
                    }
                }
            });
        }
    }
}

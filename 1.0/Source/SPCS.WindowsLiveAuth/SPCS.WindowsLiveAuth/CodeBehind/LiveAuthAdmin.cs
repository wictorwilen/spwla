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
using System.Web.UI.WebControls;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using SPExLib.General;
using SPExLib.SharePoint;

namespace SPCS.WindowsLiveAuth {
    public class LiveAuthAdmin: System.Web.UI.Page {

        protected CheckBox cbEnable;
        protected CheckBox cbNewProfiles;
        protected CheckBox cbProfileUpdates;
        protected ValidationSummary ValSummary;
        protected Label LabelErrorMessage;
        protected DropDownList ddAnnouncementList;

        private void Page_Load(object sender, System.EventArgs e) {
            if (!SPContext.Current.Web.UserIsSiteAdmin) {
                SPUtility.HandleAccessDenied(new UnauthorizedAccessException(SPResource.GetString("AdminVs401Message", new object[0])));
            }

            LiveAuthConfiguration settings = LiveAuthConfiguration.GetSettings(SPContext.Current.Site.WebApplication);
            if (settings == null) {
                LabelErrorMessage.Text = "Windows Live ID not configured for this Web Application";
                return;
            }
            foreach (SPList list in SPContext.Current.Web.Lists) {
                if (list.Fields.ContainsField("Body") && list.Fields.ContainsField("Expires")) {
                    ddAnnouncementList.Items.Add(new ListItem(list.Title));
                }
            }
            if (!this.IsPostBack) {
                SPSecurity.RunWithElevatedPrivileges(() => {
                    using (SPSite site = new SPSite(settings.ProfileSiteUrl)) {
                        using (SPWeb web = site.OpenWeb()) {
                            if (!web.ListExists(settings.SiteSyncListName))
                                LabelErrorMessage.Text = "Could not locate profile sync list, reconfigure your Windows Live ID settings";
                            else {
                                SPList list = web.Lists[settings.SiteSyncListName];
                                foreach (SPListItem item in list.Items)
                                    if (item.Contains("Url"))
                                        using (SPSite syncedSite = new SPSite(item["Url"].ToString())) {
                                            if (syncedSite.ID == SPContext.Current.Site.ID) {
                                                cbEnable.Checked = item.GetListItemBool("EnableAnnouncements");
                                                ddAnnouncementList.SelectedValue = item.GetListItemString("AnnouncementList");
                                                cbNewProfiles.Checked = item.GetListItemBool("NewProfileInfo");
                                                cbProfileUpdates.Checked = item.GetListItemBool("ProfileUpdateInfo");
                                                break;
                                            }
                                        }
                            }
                        }
                    }
                });
            }        
        }


        public void Submit_Click(Object sender, EventArgs e) {
            LiveAuthConfiguration settings = LiveAuthConfiguration.GetSettings(SPContext.Current.Site.WebApplication);
            if (settings == null) {                    
                LabelErrorMessage.Text = "Windows Live ID not configured for this Web Application";
                return;
            }

            SPSecurity.RunWithElevatedPrivileges(() => {
                using (SPSite site = new SPSite(settings.ProfileSiteUrl)) {
                    using (SPWeb web = site.OpenWeb()) {
                        if (!web.ListExists(settings.SiteSyncListName))
                            LabelErrorMessage.Text = "Could not locate profile sync list, reconfigure your Windows Live ID settings";
                        else {
                            web.AllowUnsafeUpdates = true;
                            SPList list = web.Lists[settings.SiteSyncListName];
                            foreach (SPListItem item in list.Items)
                                using (SPSite syncedSite = new SPSite(item["Url"].ToString())) {
                                    if (syncedSite.ID == SPContext.Current.Site.ID) {
                                        item["EnableAnnouncements"] = cbEnable.Checked;
                                        item["AnnouncementList"] = ddAnnouncementList.SelectedValue;
                                        item["NewProfileInfo"] = cbNewProfiles.Checked;
                                        item["ProfileUpdateInfo"] = cbProfileUpdates.Checked;
                                        item.Update();
                                        break;
                                    }
                                }
                        }
                    }
                }
            });
        
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

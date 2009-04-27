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
using System.Web.Configuration;
using System.Web.UI.WebControls;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.WebControls;
using System.Text.RegularExpressions;
using System.IO;
using System.Xml;

namespace SPCS.WindowsLiveAuth {
    public partial class LiveAuthSettings: System.Web.UI.Page {

        protected const string assemblyName = "SPCS.WindowsLiveAuth, Version=1.0.0.0, Culture=neutral, PublicKeyToken=4b37e1a71dac8e81";
        protected const string className = "SPCS.WindowsLiveAuth.ProfileListEventHandler";
        protected const string MemberResource = @"<add name=""{0}"" type=""SPCS.WindowsLiveAuth.LiveMembershipProvider, SPCS.WindowsLiveAuth, Version=1.0.0.0, Culture=neutral, PublicKeyToken=4b37e1a71dac8e81"" />";
        protected const string RoleResource = @"<add name=""{0}"" type=""SPCS.WindowsLiveAuth.LiveRoleProvider, SPCS.WindowsLiveAuth, Version=1.0.0.0, Culture=neutral, PublicKeyToken=4b37e1a71dac8e81"" />";
        protected const string FormsResource = @"/_layouts/liveauth-handler.ashx?action=login";
        protected static Regex rulesMatch = new Regex("\"(?<node>[^\"]*)\"", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        protected WebApplicationSelector Selector;
        protected DropDownList ddZones;
        protected TextBox tbApplicationId;
        protected TextBox tbApplicationKey;
        protected CheckBox cbCreateLists;
        protected TextBox tbProfileSiteUrl;
        protected TextBox tbProfileListName;
        protected TextBox tbSyncListName;
        protected TextBox tbLockedUrl;
        protected CheckBox cbHttps;
        protected CheckBox cbApprove;
        protected ValidationSummary ValSummary;


        private void Page_Load(object sender, System.EventArgs e) {
            if (!this.Context.Request.RawUrl.ToLowerInvariant().StartsWith("/_admin/")) {
                SPUtility.HandleAccessDenied(new UnauthorizedAccessException(SPResource.GetString("AdminVs401Message", new object[0])));
            }
            
           


        }

        public void Delete_Click(Object sender, EventArgs e) {
            if (Selector.CurrentItem != null) {
                SPWebApplication webApp = Selector.CurrentItem;
                LiveAuthConfiguration settings = LiveAuthConfiguration.GetSettings(webApp);
                if (settings != null) {
                    settings.Delete();
                }
            }
            tbApplicationId.Text = string.Empty;
            tbApplicationKey.Text = string.Empty;
            cbHttps.Checked = false;
            tbLockedUrl.Text = string.Empty;
            tbProfileListName.Text = string.Empty;
            tbProfileSiteUrl.Text = string.Empty;
            tbSyncListName.Text = string.Empty;
            cbApprove.Checked = true;

            // delete providers
            removeProviders(Selector.CurrentItem);
        }

        public void Submit_Click(Object sender, EventArgs e) {
            Validate("valgroup1");
            if (this.IsValid) {
                LiveAuthConfiguration settings = LiveAuthConfiguration.GetSettings(Selector.CurrentItem);
                if (settings == null) {                    
                    settings = LiveAuthConfiguration.CreateNew(Selector.CurrentItem);
                    settings.Update();

                }
                settings.ApplicationId = tbApplicationId.Text;
                settings.ApplicationKey = tbApplicationKey.Text;
                settings.ApplicationMode = cbHttps.Checked ? LiveAuthConfiguration.LiveAuthApplicationMode.https : LiveAuthConfiguration.LiveAuthApplicationMode.http;
                settings.LockedUrl = tbLockedUrl.Text;
                settings.ProfileListName = tbProfileListName.Text;
                settings.ProfileSiteUrl = tbProfileSiteUrl.Text;
                settings.SiteSyncListName = tbSyncListName.Text;
                settings.AutoApprove = cbApprove.Checked;
                settings.Update();

                
                if (cbCreateLists.Checked) {
                    // create lists
                    createProfileList(tbProfileSiteUrl.Text, tbProfileListName.Text);
                    createSyncList(tbProfileSiteUrl.Text, tbSyncListName.Text);
                }


                SPWebApplication webApp = Selector.CurrentItem;

                // configure all zones
                configureProviders(webApp);

                if (!string.IsNullOrEmpty(ddZones.SelectedValue)) {
                    // configure the zone
                    configureFormsAuthForZone(webApp, (SPUrlZone)Enum.Parse(typeof(SPUrlZone), ddZones.SelectedValue));
                }
                // configure the other zones
                foreach (SPUrlZone zone in webApp.IisSettings.Keys) {
                    if (zone != (SPUrlZone)Enum.Parse(typeof(SPUrlZone), ddZones.SelectedValue)) {
                        configureProvidersForZone(webApp, zone);
                    }                    
                }
                //applyWebConfigModifications(webApp);

                

            }
        }

        private static void applyWebConfigModifications(SPWebApplication webApp) {
            try {
                webApp.WebService.ApplyWebConfigModifications();
            }
            catch (Exception ex) {
                // Check the error messages and report accordingly
                MatchCollection matches = rulesMatch.Matches(ex.Message);

                if (matches.Count > 0) {
                    // Forms login node only exists on servers using FBA not on the others so squelch the error
                    if (matches[0].Groups["node"].Value == "configuration/system.web/authentication/forms") {
                        webApp.WebConfigModifications.RemoveAt(webApp.WebConfigModifications.Count - 1);
                        applyWebConfigModifications(webApp);
                    }
                    else {
                        throw new ApplicationException(string.Format("Error applying web.config modification. Failed to find node \"{0}\".", matches[0].Groups["node"].Value));
                    }
                }
                else {
                    throw new ApplicationException(string.Format("ApplyWebConfigChanges() error: {0}", ex.Message));
                }
            }
        }
        protected void OnContextChange(object sender, EventArgs e) {
            ddZones.Items.Add(new ListItem("(none)", string.Empty));
            if (Selector.CurrentItem != null) {
                foreach (SPUrlZone zone in Selector.CurrentItem.IisSettings.Keys) {
                    SPIisSettings iis = Selector.CurrentItem.IisSettings[zone];
                    if (iis.AuthenticationMode == AuthenticationMode.Forms) {
                        ddZones.Items.Add(new ListItem(zone.ToString() + " (" + iis.MembershipProvider + ")", zone.ToString()));
                    }

                }
                LiveAuthConfiguration settings = Selector.CurrentItem.GetChild<LiveAuthConfiguration>("LiveAuthConfiguration");
                if (settings != null) {
                    tbApplicationId.Text = settings.ApplicationId;
                    tbApplicationKey.Text = settings.ApplicationKey;
                    cbHttps.Checked = settings.ApplicationMode == LiveAuthConfiguration.LiveAuthApplicationMode.https;
                    tbLockedUrl.Text = settings.LockedUrl;
                    tbProfileListName.Text = settings.ProfileListName;
                    tbProfileSiteUrl.Text = settings.ProfileSiteUrl;
                    tbSyncListName.Text = settings.SiteSyncListName;
                    cbApprove.Checked = settings.AutoApprove;
                }
                else {
                    tbApplicationId.Text = string.Empty;
                    tbApplicationKey.Text = string.Empty;
                    cbHttps.Checked = false;
                    tbLockedUrl.Text = string.Empty;
                    tbProfileListName.Text = string.Empty;
                    tbProfileSiteUrl.Text = string.Empty;
                    tbSyncListName.Text = string.Empty;
                    cbApprove.Checked = true;
                }
            }   
        }


        private static void configureProviders(SPWebApplication webApp) {
            // add membership provider
            SPWebConfigModification membershipmod = createWebConfigModification(80008, "membership", "configuration/system.web", "<membership><providers></providers></membership>", SPWebConfigModification.SPWebConfigModificationType.EnsureChildNode);
            // TODO: make LiveID configurable
            SPWebConfigModification membermod = createWebConfigModification(80009, "add[@name='LiveID']", "configuration/system.web/membership/providers", string.Format(MemberResource, "LiveID"), SPWebConfigModification.SPWebConfigModificationType.EnsureChildNode);
            SPWebConfigModification rolemanmod = createWebConfigModification(80010, "roleManager", "configuration/system.web", "<roleManager><providers></providers></roleManager>", SPWebConfigModification.SPWebConfigModificationType.EnsureChildNode);
            // TODO: make LiveRoles configurable
            SPWebConfigModification rolemod = createWebConfigModification(80011, "add[@name='LiveRoles']", "configuration/system.web/roleManager/providers", string.Format(RoleResource, "LiveRoles"), SPWebConfigModification.SPWebConfigModificationType.EnsureChildNode);

            webApp.WebConfigModifications.Add(membershipmod);
            webApp.WebConfigModifications.Add(membermod);
            webApp.WebConfigModifications.Add(rolemanmod);
            webApp.WebConfigModifications.Add(rolemod);

            webApp.WebService.ApplyWebConfigModifications();
        }

        private static SPWebConfigModification createWebConfigModification(uint sequence, string name, string path, string value, SPWebConfigModification.SPWebConfigModificationType wmtype) {
            SPWebConfigModification ret = new SPWebConfigModification(name, path);
            ret.Owner = "WindowsLiveAuthProvider";
            ret.Sequence = sequence;
            ret.Type = wmtype;
            ret.Value = value;

            return ret;
        }
        private void removeProviders(SPWebApplication webApp) {
            throw new NotImplementedException();
        }
        private void configureFormsAuthForZone(SPWebApplication webApp, SPUrlZone urlZone) {
            // make a manual change to the web.config, since the SPWebConfigModification does not support
            // zone specific changes. backside, this will not propagate to new front-ends (but just reconfigure WLA)
            SPIisSettings iisSettings;
            if (webApp.IisSettings.TryGetValue(urlZone, out iisSettings)) {
                FileInfo info = new FileInfo(iisSettings.Path.ToString() + @"\web.config");
                
                XmlDocument document = new XmlDocument();
                document.Load(info.ToString());

                // set the login form
                XmlNode node = document.SelectNodes("configuration/system.web/authentication/forms")[0];
                if ((node == null) || (node.NodeType != XmlNodeType.Element)) {
                    throw new ApplicationException("Selected zone is not configured for Forms authentication");
                }

                XmlElement elm = node as XmlElement;
                if (elm != null) {
                    elm.SetAttribute("loginUrl", FormsResource);
                }

                // add default providers
                node = document.SelectNodes("configuration/system.web/membership")[0];
                if ((node == null) || (node.NodeType != XmlNodeType.Element)) {
                    throw new ApplicationException("Selected zone is not configured for Forms authentication (membership element is missing)");
                }
                elm = node as XmlElement;
                if (elm != null) {
                    elm.SetAttribute("defaultProvider", "LiveID");
                }
                node = document.SelectNodes("configuration/system.web/roleManager")[0];
                if ((node == null) || (node.NodeType != XmlNodeType.Element)) {
                    throw new ApplicationException("Selected zone is not configured for Forms authentication (roleManager element is missing)");
                }
                elm = node as XmlElement;
                if (elm != null) {
                    elm.SetAttribute("enabled", "true");
                    elm.SetAttribute("defaultProvider", "LiveRoles");
                }
                document.Save(info.ToString());

            }

        }

        private void configureProvidersForZone(SPWebApplication webApp, SPUrlZone urlZone)
        {
            // make a manual change to the web.config, since the SPWebConfigModification does not support
            // zone specific changes. backside, this will not propagate to new front-ends (but just reconfigure WLA)
            SPIisSettings iisSettings;
            if (webApp.IisSettings.TryGetValue(urlZone, out iisSettings)) {
                FileInfo info = new FileInfo(iisSettings.Path.ToString() + @"\web.config");

                XmlDocument document = new XmlDocument();
                document.Load(info.ToString());

                XmlNode node = document.SelectNodes("configuration/system.web/roleManager")[0];
                if ((node == null) || (node.NodeType != XmlNodeType.Element)) {
                    return; // no roleManager
                }
                XmlElement elm = node as XmlElement;
                if (elm != null) {
                    elm.SetAttribute("enabled", "true");
                }
                document.Save(info.ToString());

            }

        }
        private void createSyncList(string siteUrl, string listName) {
            using (SPSite site = new SPSite(siteUrl)) {
                using (SPWeb web = site.OpenWeb()) {
                    SPList list;
                    if (web.ListExists(listName)) {
                        // it already exists
                        // verify the fields
                        list = web.Lists[listName];
                        createProfileSyncListFields(list);
                        list.Update();
                        return;
                    }

                    SPListTemplate template = web.ListTemplates["Custom List"];
                    Guid g = web.Lists.Add(listName, "Windows Live ID Profile Sync and settings", template);
                    web.Update();

                    list = web.Lists[g];
                    list.OnQuickLaunch = true;
                    list.EnableAttachments = false;
                    list.EnableVersioning = true;
                    
                    createProfileSyncListFields(list);
                                       
                    list.Update();
                }
            }   
        }
        private static void createProfileList(string siteUrl, string listName) {
            using (SPSite site = new SPSite(siteUrl)) {
                using (SPWeb web = site.OpenWeb()) {
                    SPList list;
                    if (web.ListExists(listName)) {
                        // it already exists
                        // verify the fields
                        list = web.Lists[listName];
                        createProfileListFields(list);
                       
                        list.Update();
                        return;
                    }

                    SPListTemplate template = web.ListTemplates["Custom List"];
                    Guid g = web.Lists.Add(listName, "Windows Live ID Profile Storage", template);
                    web.Update();
                    list = web.Lists[g];
                    list.OnQuickLaunch = true;
                    list.EnableAttachments = true;
                    list.EnableVersioning = true;
                    list.Hidden = false;

                    createProfileListFields(list);

                    list.EventReceivers.Add(SPEventReceiverType.ItemAdded, assemblyName, className);
                    list.EventReceivers.Add(SPEventReceiverType.ItemUpdated, assemblyName, className);
                    
                    list.AllowEveryoneViewItems = false;
                    
                    list.Update();
                    web.Update();

                }
            }
        }
        private static void createProfileSyncListFields(SPList list) {
            // Url to site coll           
            if (!list.Fields.ContainsField("Url")) {
                list.Fields.Add("Url", SPFieldType.Text, false);
            }
            // fields for announcement stuff
            if (!list.Fields.ContainsField("EnableAnnouncements")) {
                list.Fields.Add("EnableAnnouncements", SPFieldType.Boolean, false);
            }
            if (!list.Fields.ContainsField("AnnouncementList")) {
                list.Fields.Add("AnnouncementList", SPFieldType.Text, false);
            }
            if (!list.Fields.ContainsField("NewProfileInfo")) {
                list.Fields.Add("NewProfileInfo", SPFieldType.Boolean, false);
            }
            if (!list.Fields.ContainsField("ProfileUpdateInfo")) {
                list.Fields.Add("ProfileUpdateInfo", SPFieldType.Boolean, false);
            }
        }
        private static void createProfileListFields(SPList list) {
            if (!list.Fields.ContainsField("DisplayName")) {
                list.Fields.Add("DisplayName", SPFieldType.Text, false);
            }
            if (!list.Fields.ContainsField("UUID")) {
                list.Fields.Add("UUID", SPFieldType.Text, false);
            }
            if (!list.Fields.ContainsField("Email")) {
                list.Fields.Add("Email", SPFieldType.Text, false);
            }
            if (!list.Fields.ContainsField("Description")) {
                list.Fields.Add("Description", SPFieldType.Text, false);
            }
            if (!list.Fields.ContainsField("JobTitle")) {
                list.Fields.Add("JobTitle", SPFieldType.Text, false);
            }
            if (!list.Fields.ContainsField("Company")) {
                list.Fields.Add("Company", SPFieldType.Text, false);
            }
            if (!list.Fields.ContainsField("SipAddress")) {
                list.Fields.Add("SipAddress", SPFieldType.Text, false);
            }
            if (!list.Fields.ContainsField("Locked")) {
                list.Fields.Add("Locked", SPFieldType.Boolean, false);
            }
            if (!list.Fields.ContainsField("Approved")) {
                list.Fields.Add("Approved", SPFieldType.Boolean, false);
            }
            if (!list.Fields.ContainsField("HomePageUrl")) {
                list.Fields.Add("HomePageUrl", SPFieldType.Text, false);
            }
            if (!list.Fields.ContainsField("FeedUrl")) {
                list.Fields.Add("FeedUrl", SPFieldType.Text, false);
            }
            if (!list.Fields.ContainsField("TwitterAccount")) {
                list.Fields.Add("TwitterAccount", SPFieldType.Text, false);
            }
            if (!list.Fields.ContainsField("ImageUrl")) {
                list.Fields.Add("ImageUrl", SPFieldType.Text, false);
            }
            if (!list.Fields.ContainsField("LastLogin")) {
                list.Fields.Add("LastLogin", SPFieldType.DateTime, false);
            }
            if (!list.Fields.ContainsField("LockedOn")) {
                list.Fields.Add("LockedOn", SPFieldType.DateTime, false);
            }
            if (!list.Fields.ContainsField("Country")) {
                list.Fields.Add("Country", SPFieldType.Text, false);
            }
            if (!list.Fields.ContainsField("City")) {
                list.Fields.Add("City", SPFieldType.Text, false);
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

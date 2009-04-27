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
using Microsoft.SharePoint;

namespace SPCS.WindowsLiveAuth {
    public class LiveAuthFeatureReceiver : SPFeatureReceiver {
        public override void FeatureActivated(SPFeatureReceiverProperties properties) {
            SPSite site = properties.Feature.Parent as SPSite;

            LiveAuthConfiguration settings = site.WebApplication.GetChild<LiveAuthConfiguration>("LiveAuthConfiguration");

            if (settings != null && !string.IsNullOrEmpty(settings.ProfileSiteUrl)) {

                using (SPSite lSite = new SPSite(settings.ProfileSiteUrl)) {
                    using (SPWeb lWeb = lSite.OpenWeb()) {
                        SPList lList = lWeb.Lists[settings.SiteSyncListName];
                        SPListItem item = lList.Items.Add();
                        item["Url"] = site.Url;
                        item["Title"] = site.Url;
                        item.Update();
                    }
                }
            }
        }

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties) {
            SPSite site = properties.Feature.Parent as SPSite;
            LiveAuthConfiguration settings = site.WebApplication.GetChild<LiveAuthConfiguration>("LiveAuthConfiguration");

            if (settings != null && !string.IsNullOrEmpty(settings.ProfileSiteUrl)) {

                using (SPSite lSite = new SPSite(settings.ProfileSiteUrl)) {
                    using (SPWeb lWeb = lSite.OpenWeb()) {
                        SPList lList = lWeb.Lists[settings.SiteSyncListName];
                        SPQuery query = new SPQuery();
                        query.Query = string.Format("<Where><Eq><FieldRef Name='Url'/><Value Type='Text'>{0}</Value></Eq></Where>", site.Url);
                        SPListItemCollection items = lList.GetItems(query);

                        if (items.Count > 0) {
                            items[0].Delete();
                        }

                        lList.Update();
                    }
                }
            }
        }

        public override void FeatureInstalled(SPFeatureReceiverProperties properties) {
        }

        public override void FeatureUninstalling(SPFeatureReceiverProperties properties) {
        }
    }

}

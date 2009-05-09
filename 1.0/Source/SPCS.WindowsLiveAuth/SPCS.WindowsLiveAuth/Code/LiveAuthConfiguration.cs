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
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint;

namespace SPCS.WindowsLiveAuth {
    public class LiveAuthConfiguration : SPPersistedObject {
        public LiveAuthConfiguration(string name, SPPersistedObject parent, Guid id) : base(name, parent, id) { }
        public LiveAuthConfiguration() { }

        [Persisted]
        public string ProfileSiteUrl;

        [Persisted]
        public string ApplicationId;
        
        [Persisted]
        public string ApplicationKey;


        [Persisted]
        public string Domain;

        [Persisted]
        public LiveAuthApplicationMode ApplicationMode;
        
        [Persisted]
        public string LockedUrl;
        
        [Persisted]
        public string ProfileListName;

        [Persisted]
        public string SiteSyncListName = "ProfileSync";

        [Persisted]
        public bool AutoApprove = true;

        [Persisted]
        public bool UseDelegatedAuth = false;

        [Persisted]
        public string PolicyPage = string.Empty;
        
        public readonly string ApplicationAlgorithm = "wsignin1.0";

        public enum LiveAuthApplicationMode {
            http,
            https
        }


        public static LiveAuthConfiguration GetSettings(SPWebApplication webApplication) {
            LiveAuthConfiguration lac = null;
            SPSecurity.RunWithElevatedPrivileges(() => 
                lac = webApplication.GetChild<LiveAuthConfiguration>("LiveAuthConfiguration")
                );
            return lac;
        }
        public static LiveAuthConfiguration CreateNew(SPWebApplication webApplication) {
            return new LiveAuthConfiguration("LiveAuthConfiguration",  webApplication, Guid.NewGuid());
        }

    }
}

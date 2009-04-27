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
using System.Collections.Generic;
using System.Linq;
using System.Web.Security;
using System.Configuration.Provider;

namespace SPCS.WindowsLiveAuth {
    public class LiveRoleProvider: RoleProvider {
        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config) {
            if (name == null || name.Length == 0) {
                name = "LiveRoleProvider";
            }
            this.ApplicationName = name;
            base.Initialize(name, config);

        }
        public override void AddUsersToRoles(string[] usernames, string[] roleNames) {
        }
        public override string ApplicationName {
            get;
            set;
        }

        public override void CreateRole(string roleName) {
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole) {
            return true;
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch) {
            string[] users;
            if (!RoleExists(roleName)) {
                throw new ProviderException("Role note found");
            }
            List<LiveCommunityUser> lcus = LiveCommunityUser.FindUsers("UUID", usernameToMatch);
            switch (roleName.ToLower()) {
                case "authenticated live users":
                    users = (from lcu in lcus
                             orderby lcu.DisplayName
                             select lcu.Id).ToArray<string>();
                    break;
                case "locked live users":
                    users = (from lcu in lcus
                             where lcu.Locked == true
                             orderby lcu.DisplayName
                             select lcu.Id).ToArray<string>();
                    break;
                case "unapproved live users":
                    users = (from lcu in lcus
                             where lcu.Approved == false
                             orderby lcu.DisplayName
                             select lcu.Id).ToArray<string>();
                    break;
                case "live users":
                    users = (from lcu in lcus
                             where lcu.Approved == true && lcu.Locked == false
                             orderby lcu.DisplayName
                             select lcu.Id).ToArray<string>();
                    break;
                default:
                    users = new string[1] { string.Empty };
                    break;
            }
            return users;
        }

        public override string[] GetAllRoles() {
            string[] roles = new string[4];
            roles[0] = "Authenticated Live Users";  // all users 
            roles[1] = "Locked Live Users";         // locked
            roles[2] = "Unapproved Live Users";     // unapproved
            roles[3] = "Live Users";                // approved && !locked
            return roles;

        }

        public override string[] GetRolesForUser(string username) {
            LiveCommunityUser lcu = LiveCommunityUser.GetUser(username);
            List<string> roles = new List<string>();
            roles.Add("Authenticated Live Users");
            if (lcu.Locked) {
                roles.Add("Locked Live Users");
            }
            if (!lcu.Approved) {
                roles.Add("Unapproved Live Users");
            }
            if (lcu.Approved && !lcu.Locked) {
                roles.Add("Live Users");
            }

            return roles.ToArray();
        }

        public override string[] GetUsersInRole(string roleName) {
            string[] users;
            
            if (!RoleExists(roleName)) {
                throw new ProviderException("Role note found");
            }
            
            List<LiveCommunityUser> lcus = LiveCommunityUser.GetAllUsers();
            switch (roleName.ToLower()) {
                case "authenticated live users":
                    users = (from lcu in lcus
                             where lcu.Approved == true
                             select lcu.Id).ToArray<string>();
                    break;
                case "locked live users":
                    users = (from lcu in lcus
                             where lcu.Locked == true
                             select lcu.Id).ToArray<string>();
                    break;
                case "unapproved live users":
                    users = (from lcu in lcus
                             where lcu.Approved == false
                             select lcu.Id).ToArray<string>();
                    break;
                case "live users":
                    users = (from lcu in lcus
                             select lcu.Id).ToArray<string>();
                    break;
                default:
                    users = new string[1] { string.Empty };
                    break;
            }
            return users;

        }

        public override bool IsUserInRole(string username, string roleName) {

            LiveCommunityUser lcu = LiveCommunityUser.GetUser(username);
            switch (roleName.ToLower()) {
                case "authenticated live users":
                    return lcu.Approved;
                case "locked live users":
                    return lcu.Locked;
                case "unapproved live users":
                    return !lcu.Approved;
                case "live users":
                    return true;

            }

            return false;

        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames) {
        }

        public override bool RoleExists(string roleName) {
            foreach (string s in this.GetAllRoles()) {
                if (s.ToLower() == roleName.ToLower()) {
                    return true;
                }
            }
            return false;
        }
    }
}

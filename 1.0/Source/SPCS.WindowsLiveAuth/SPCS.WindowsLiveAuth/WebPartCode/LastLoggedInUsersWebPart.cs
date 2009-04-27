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
using System.Runtime.InteropServices;

namespace SPCS.WindowsLiveAuth {
    [Guid("179c3248-7e59-4c35-8bbe-d4d473378ea3")]
    public class LastLoggedInUsersWebPart : LiveUserQueryBaseWebPart {
        public override string Query {
            get {
                return  "<OrderBy><FieldRef Name='LastLogin' Ascending='False' /></OrderBy>";
            }
        }
    }
}

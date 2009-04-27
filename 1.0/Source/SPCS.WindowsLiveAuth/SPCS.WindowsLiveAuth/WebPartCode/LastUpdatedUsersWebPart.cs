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
    [Guid("87155FC0-A9E0-4c3f-AF92-A85FB04B8B5A")]
    public class LastUpdatedUsersWebPart : LiveUserQueryBaseWebPart {
        public override string Query {
            get {
                return  "<OrderBy><FieldRef Name='Modified' Ascending='False' /></OrderBy>";
            }
        }
    }
}

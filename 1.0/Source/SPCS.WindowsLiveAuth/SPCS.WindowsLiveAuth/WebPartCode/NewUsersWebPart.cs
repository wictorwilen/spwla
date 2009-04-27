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
    [Guid("a6698c0d-236d-452a-b9ad-b6372ce5fcdb")]
    public class NewUsersWebPart: LiveUserQueryBaseWebPart {
        public override string Query {
            get {
                return "<OrderBy><FieldRef Name='Created' Ascending='False' /></OrderBy>";
            }
        }
    }
}

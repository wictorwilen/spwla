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
using System.Linq;
using System.Text;
using Microsoft.SharePoint;

namespace SPCS.WindowsLiveAuth {
    public static class SPWebExtensions {
        public static bool ListExists(this SPWeb web, string name) {
            try {
                // -- Check if list exists.
                SPList list = web.Lists[name];
            }
            catch (ArgumentException) {
                return false;
            }
            return true;
        }
    }
}

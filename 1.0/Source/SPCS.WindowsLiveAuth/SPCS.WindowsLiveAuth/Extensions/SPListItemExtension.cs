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
using Microsoft.SharePoint.Utilities;

namespace SPCS.WindowsLiveAuth {
    public static class SPListItemExtension {
        public static string GetListItemString(this SPListItem item, string column) {
            string ret = string.Empty;
            object data = item.GetListItemData(column);

            if (data != null) {
                ret = (string)data;
            }

            return ret;
        }
        public static DateTime GetListItemDateTime(this SPListItem item, string column) {
            DateTime ret = new DateTime(1900, 1, 1);            
            object data = item.GetListItemData(column);

            if (data != null) {
                ret = (DateTime)data;
            }

            return ret;
        }

        public static bool GetListItemBool(this SPListItem item, string column) {
            bool ret = false;
            object data =item.GetListItemData(column);

            if (data != null) {
                ret = (bool)data;
            }

            return ret;
        }

        public static object GetListItemData(this SPListItem item, string column) {
            object ret = null;
            if (item.Contains(column)) {
                ret = item[column];
            }

            return ret;
        }

        public static bool Contains(this SPListItem item, string fieldName) {
            return item.Fields.ContainsField(fieldName);
        }
    }

}

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
using Microsoft.SharePoint;

namespace SPCS.WindowsLiveAuth {
    public static class SPListItemExtension {
        public static string GetListItemString(this SPListItem item, string column) {
            object data = item.GetListItemData(column);

            if (data != null)
                return (string)data;
            else
                return string.Empty;
        }
        public static DateTime GetListItemDateTime(this SPListItem item, string column) {
            object data = item.GetListItemData(column);

            if (data != null)
                return (DateTime)data;
            else
                return new DateTime(1900, 1, 1);
        }

        public static bool GetListItemBool(this SPListItem item, string column) {
            object data =item.GetListItemData(column);

            if (data != null)
                return (bool)data;
            else
                return false;
        }

        public static object GetListItemData(this SPListItem item, string column) {
            if (item.Contains(column))
                return item[column];
            else
                return null;
        }

        public static bool Contains(this SPListItem item, string fieldName) {
            return item.Fields.ContainsField(fieldName);
        }
    }

}

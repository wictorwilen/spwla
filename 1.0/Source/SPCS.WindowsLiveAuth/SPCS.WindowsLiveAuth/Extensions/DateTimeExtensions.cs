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

namespace SPCS.WindowsLiveAuth
{
    public static class DateTimeExtensions {

        public static DateTime? ParseNull(object o) {
            if (o == null) {
                return null;
            }
            return DateTime.Parse(o.ToString());
        }
    }
}

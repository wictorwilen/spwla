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
using System.Linq;

namespace SPCS.WindowsLiveAuth
{
    public static class SPUserExtension {
        public static string GetFormsLoginName(this SPUser spUser)  {
            return spUser.LoginName.Contains(':') ? spUser.LoginName.Split(':')[1] : spUser.LoginName;

        }
    }
}

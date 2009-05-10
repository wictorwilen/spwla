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
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using System;

namespace SPCS.WindowsLiveAuth {
    [ToolboxData("<{0}:LiveCommunityUserControl runat=server></{0}:LiveCommunityUserControl>")]
    public class LiveCommunityUserControl : WebControl {
        public LiveCommunityUserControl() {
            
        }
        public LiveCommunityUserControl(HtmlTextWriterTag tag)
            : base(tag) {
            
        }
        protected LiveCommunityUserControl(string tag)
            : base(tag) {
            
        }
   
        
        [Bindable(true)]
        [Category("Data")]
        public LiveCommunityUser LiveCommunityUser {
            get;
            set;
        }
        [Bindable(true)]
        [Category("Layout")]
        [DefaultValue(50)]
        public int ImageSize{
            get;
            set;
        }

        [Bindable(true)]
        [Category("Layout")]
        [DefaultValue(true)]
        public bool ShowPresence {
            get;
            set;
        }
        [Bindable(true)]
        [Category("Layout")]
        [DefaultValue(false)]
        public bool ShowPresenceForAnonymous {
            get;
            set;
        }

        

        protected override void RenderContents(HtmlTextWriter output) {
            
            output.Write("<table cellPadding='0' cellSpacing='0' border='0' width='100%' class='vCard'><tr>");
            output.Write("<td style='width:{0}px;padding: 0 3 3 0'>", this.ImageSize);
            output.Write("<img src='/_layouts/liveauth-image.ashx?uuid={0}&s={1}' alt='{2}' class='photo'/>", this.LiveCommunityUser.Id, this.ImageSize, this.LiveCommunityUser.DisplayName);
            output.Write("</td>");
            output.Write("<td style='padding: 0 3 3 0' valign='top'>");
            if (this.Page.User.Identity.IsAuthenticated) {
                output.Write("<a href='/_layouts/liveauth-profile.aspx?uuid={0}' style='font-weight:bold' class='fn url'>{1}</a>", this.LiveCommunityUser.Id, this.LiveCommunityUser.DisplayName);
                if (this.ShowPresence && !string.IsNullOrEmpty(LiveCommunityUser.CID)) {
                    output.Write(String.Format("<img src='http://messenger.services.live.com/users/{0}@apps.messenger.live.com/presenceimage/' align='left'/>", LiveCommunityUser.CID));
                }
            }
            else {
                output.Write("<span style='font-weight:bold' class='fn'>{0}</span>", this.LiveCommunityUser.DisplayName);
                if (this.ShowPresenceForAnonymous && !string.IsNullOrEmpty(LiveCommunityUser.CID)) {
                    output.Write(String.Format("<img src='http://messenger.services.live.com/users/{0}@apps.messenger.live.com/presenceimage/' align='left'/>", LiveCommunityUser.CID));
                }
            }
            
            
            output.Write("<br/><nobr>");
            if (!string.IsNullOrEmpty(this.LiveCommunityUser.Title)) {
                output.Write(String.Format("<span class='title'>{0}</span>", this.LiveCommunityUser.Title));
            }
            if (!string.IsNullOrEmpty(this.LiveCommunityUser.Company)) {
                if (!string.IsNullOrEmpty(this.LiveCommunityUser.Title)) {
                    output.Write(", ");
                }
                output.Write(String.Format("<span class='org'>{0}</span>", this.LiveCommunityUser.Company));
            }
            output.Write("</nobr><br/><nobr>");
            if (!string.IsNullOrEmpty(this.LiveCommunityUser.City)) {
                output.Write(String.Format("<span class='adr locality'>{0}</span>", this.LiveCommunityUser.City));
            }
            if (!string.IsNullOrEmpty(this.LiveCommunityUser.Country)) {
                if (!string.IsNullOrEmpty(this.LiveCommunityUser.City)) {
                    output.Write(", ");
                }
                output.Write(String.Format("<span class='adr country-name'>{0}</span>", this.LiveCommunityUser.Country));
            }
            output.Write("</nobr></td>");
            output.Write("</tr></table>");
        }
    }
}

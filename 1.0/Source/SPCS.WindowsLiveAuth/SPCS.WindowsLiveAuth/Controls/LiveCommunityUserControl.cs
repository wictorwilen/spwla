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

namespace SPCS.WindowsLiveAuth {
    [ToolboxData("<{0}:LiveCommunityUserControl runat=server></{0}:LiveCommunityUserControl>")]
    public class LiveCommunityUserControl : WebControl {
        
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

        

        protected override void RenderContents(HtmlTextWriter output) {
            
            output.Write("<table cellPadding='0' cellSpacing='0' border='0' width='100%' class='vCard'><tr>");
            output.Write("<td style='width:{0}px;padding: 0 3 3 0'>", this.ImageSize);
            output.Write("<img src='/_layouts/liveauth-image.ashx?uuid={0}&s={1}' alt='{2}' class='photo'/>", this.LiveCommunityUser.Id, this.ImageSize, this.LiveCommunityUser.DisplayName);
            output.Write("</td>");
            output.Write("<td style='padding: 0 3 3 0' valign='top'>");
            if (this.Page.User.Identity.IsAuthenticated) {
                output.Write("<a href='/_layouts/liveauth-profile.aspx?uuid={0}' style='font-weight:bold' class='fn url'>{1}</a>", this.LiveCommunityUser.Id, this.LiveCommunityUser.DisplayName);
            }
            else {
                output.Write("<span style='font-weight:bold' class='fn'>{0}</span>", this.LiveCommunityUser.DisplayName);
            }
            output.Write("<br/><nobr>");
            if (!string.IsNullOrEmpty(this.LiveCommunityUser.Title)) {
                output.Write("<span class='title'>" + this.LiveCommunityUser.Title + "</span>");
            }
            if (!string.IsNullOrEmpty(this.LiveCommunityUser.Company)) {
                if (!string.IsNullOrEmpty(this.LiveCommunityUser.Title)) {
                    output.Write(", ");
                }
                output.Write("<span class='org'>" + this.LiveCommunityUser.Company + "</span>");
            }
            output.Write("</nobr><br/><nobr>");
            if (!string.IsNullOrEmpty(this.LiveCommunityUser.City)) {
                output.Write("<span class='adr locality'>" + this.LiveCommunityUser.City + "</span>");
            }
            if (!string.IsNullOrEmpty(this.LiveCommunityUser.Country)) {
                if (!string.IsNullOrEmpty(this.LiveCommunityUser.City)) {
                    output.Write(", ");
                }
                output.Write("<span class='adr country-name'>" + this.LiveCommunityUser.Country + "</span>");
            }
            output.Write("</nobr></td>");
            output.Write("</tr></table>");
        }
    }
}

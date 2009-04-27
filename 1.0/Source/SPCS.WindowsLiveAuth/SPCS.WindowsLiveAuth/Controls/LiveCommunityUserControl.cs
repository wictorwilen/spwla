using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SPCS.WindowsLiveAuth {
    [DefaultProperty("Text")]
    [ToolboxData("<{0}:LiveCommunityUserControl runat=server></{0}:LiveCommunityUserControl>")]
    public class LiveCommunityUserControl : WebControl {
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]
        public string Text {
            get {
                String s = (String)ViewState["Text"];
                return ((s == null) ? String.Empty : s);
            }

            set {
                ViewState["Text"] = value;
            }
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

        

        protected override void RenderContents(HtmlTextWriter output) {
            
            output.Write("<table cellPadding='0' cellSpacing='0' border='0' width='100%'><tr>");
            output.Write("<td style='width:{0}px;padding: 0 3 3 0'>", this.ImageSize);
            output.Write("<img src='/_layouts/liveauth-image.ashx?uuid={0}&s={1}' alt='{2}'/>", this.LiveCommunityUser.Id, this.ImageSize, this.LiveCommunityUser.DisplayName);
            output.Write("</td>");
            output.Write("<td style='padding: 0 3 3 0' valign='top'>");
            if (this.Page.User.Identity.IsAuthenticated) {
                output.Write("<a href='/_layouts/liveauth-profile.aspx?uuid={0}' style='font-weight:bold'>{1}</a>", this.LiveCommunityUser.Id, this.LiveCommunityUser.DisplayName);
            }
            else {
                output.Write("<span style='font-weight:bold'>{0}</span>", this.LiveCommunityUser.DisplayName);
            }
            output.Write("<br/><nobr>");
            if (!string.IsNullOrEmpty(this.LiveCommunityUser.Title)) {
                output.Write(this.LiveCommunityUser.Title);
            }
            if (!string.IsNullOrEmpty(this.LiveCommunityUser.Company)) {
                if (!string.IsNullOrEmpty(this.LiveCommunityUser.Title)) {
                    output.Write(", ");
                }
                output.Write(this.LiveCommunityUser.Company);
            }
            output.Write("</nobr><br/><nobr>");
            if (!string.IsNullOrEmpty(this.LiveCommunityUser.City)) {
                output.Write(this.LiveCommunityUser.City);
            }
            if (!string.IsNullOrEmpty(this.LiveCommunityUser.Country)) {
                if (!string.IsNullOrEmpty(this.LiveCommunityUser.City)) {
                    output.Write(", ");
                }
                output.Write(this.LiveCommunityUser.Country);
            }
            output.Write("</nobr></td>");
            output.Write("</tr></table>");
        }
    }
}

using System;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.ComponentModel;
using WindowsLive;
using Microsoft.SharePoint;
using SPExLib.Extensions;

namespace SPCS.WindowsLiveAuth {
    [ParseChildren(false)]
    [ControlBuilder(typeof(HyperLinkControlBuilder))]
    [Designer("System.Web.UI.Design.WebControls.HyperLinkDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
    [ToolboxData(@"<{0}:LivePresenceConsentLink runat=""server"" />")]
    [DefaultProperty("Text")]
    public class LivePresenceConsentLink: HyperLink {
        /// <summary>
        /// Initializes a new instance of the LivePresenceConsentControl class.
        /// </summary>
        public LivePresenceConsentLink() {
        }

        protected override void OnPreRender(EventArgs e) {
            base.OnPreRender(e);
            

            if (this.Page.User.Identity.IsAuthenticated) {
                SPUser spUser = SPContext.Current.Web.CurrentUser;
                LiveCommunityUser lcu = LiveCommunityUser.GetUser(spUser.GetFormsLoginName());
                LiveAuthConfiguration settings = LiveAuthConfiguration.GetSettings(SPContext.Current.Site.WebApplication);
                //only show this link if the user has sign up for this.
                if (lcu != null && settings !=  null) {
                    base.Text = "Online Presence Settings";

                    base.NavigateUrl = WindowsLiveLogin.GetOnlinePresenceInvitationUrl(
                            string.Format(@"{0}/_layouts/liveauth-handler.ashx", this.Page.Request.Url.GetLeftPart(UriPartial.Authority)),
                            settings.PolicyPage);
                }
                else {
                    this.Visible = false;
                }
            }
            else {
                this.Visible = false;
            }
        }


    }
}

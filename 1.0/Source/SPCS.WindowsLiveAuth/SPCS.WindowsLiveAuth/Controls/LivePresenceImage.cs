using System;
using System.Web.UI;
using System.ComponentModel;
using System.Web.UI.WebControls;
using Microsoft.SharePoint;

namespace SPCS.WindowsLiveAuth {
    [ParseChildren(false)]
    [ControlBuilder(typeof(Image))]
    [Designer("System.Web.UI.Design.WebControls.ImageDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
    [ToolboxData(@"<{0}:LivePresenceConsentLink runat=""server"" />")]
    [DefaultProperty("UserId")]
    public class LivePresenceImage: Image {
        /// <summary>
        /// Initializes a new instance of the LivePresenceImage class.
        /// </summary>
        public LivePresenceImage() {
        }

        [Category("Data")]
        public int UserId {
            get;
            set;
        }

        protected override void OnPreRender(EventArgs e) {
            base.OnPreRender(e);
            // only show it to the authenticated users
            if (this.Page.User.Identity.IsAuthenticated) {
                SPUser spUser = SPContext.Current.Web.AllUsers.GetByID(this.UserId);
                LiveCommunityUser lcu = LiveCommunityUser.GetUser(spUser.GetFormsLoginName());
    
                //only show this link if the user has sign up for this.
                if (lcu != null) {
                    if (string.IsNullOrEmpty(lcu.CID)) {
                        this.ImageUrl = "http://www.wlmessenger.net/static/img/presence/Offline.gif";
                    }
                    else {
                        this.ImageUrl = String.Format("http://messenger.services.live.com/users/{0}@apps.messenger.live.com/presenceimage/", lcu.CID);
                    }
          
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

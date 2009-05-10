using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.SharePoint;

namespace SPCS.WindowsLiveAuth {
    [ParseChildren(false)]
    [ToolboxData("<{0}:CurrentUserLivePresenceControl runat=server></{0}:CurrentUserLivePresenceControl>")]
    [DefaultProperty("ShowLink")]
    public class CurrentUserLivePresenceControl : PlaceHolder {


        public CurrentUserLivePresenceControl() {
            
        }
                            
        protected override void CreateChildControls() {

            if (this.Page.User.Identity.IsAuthenticated && SPContext.Current.Web.CurrentUser != null) {               
                    LivePresenceImage presenceImage = new LivePresenceImage { UserId = SPContext.Current.Web.CurrentUser.ID };
                    this.Controls.Add(presenceImage);
                    
                    if (ShowLink) {
                        this.Controls.Add(new LiteralControl("&nbsp;"));
                        LivePresenceConsentLink link = new LivePresenceConsentLink();
                        this.Controls.Add(link);
                    }                                   
            }
            else {
                this.Visible = false;
            }
            
            
            base.CreateChildControls();
        }

        [DefaultValue(true)]
        [Category("Layout")]
        public bool ShowLink {
            get;
            set;
        }
    }
}

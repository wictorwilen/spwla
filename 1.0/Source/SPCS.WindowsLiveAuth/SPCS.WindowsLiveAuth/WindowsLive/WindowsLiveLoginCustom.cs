using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsLive {
    public partial class WindowsLiveLogin {
        public static string GetOnlinePresenceInvitationUrl(string returnurl, string privacyurl) {
            return string.Format("http://settings.messenger.live.com/applications/websignup.aspx?returnurl={0}&privacyurl={1}", returnurl, privacyurl);
        }

    }
}

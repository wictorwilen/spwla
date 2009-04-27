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
using WindowsLive;
using System.Web.Security;
using System.Web;
using Microsoft.SharePoint;

namespace SPCS.WindowsLiveAuth {
    public class LiveAuthHandler : IHttpHandler {

        protected WindowsLiveLogin.User user;
        protected LiveCommunityUser communityUser;
        static WindowsLiveLogin wll = null;

        public void ProcessRequest(HttpContext context) {
            LiveAuthConfiguration settings = LiveAuthConfiguration.GetSettings(SPContext.Current.Site.WebApplication);

            if(wll == null){
                lock(this){
                    if(wll == null){
                        // initialize the WLA instance
                        wll = new WindowsLiveLogin(settings.ApplicationId, settings.ApplicationKey, settings.ApplicationAlgorithm, true, "http://www.expressen.se", "http://ibpcww7:81");
                    }
                }
            }
            user = wll.ProcessLogin(context.Request.Form);
            if (user != null) {
                communityUser = LiveCommunityUser.GetUser(user);
            }
            else {
                FormsAuthentication.SignOut();
            }

            string redirectUrl = "/Default.aspx";
            
            string liveLogin = string.Format("{0}://login.live.com/wlogin.srf?appid={1}&alg={2}&appctx=",
                settings.ApplicationMode.ToString(),
                settings.ApplicationId,
                settings.ApplicationAlgorithm);

            switch (context.Request.QueryString["action"]) {
                case "logout":
                    FormsAuthentication.SignOut();
                    break;

                case "clearcookie":
                    context.Response.Clear();
                    context.Response.Cookies.Remove(FormsAuthentication.FormsCookieName);
                    context.Response.ClearHeaders();
                    
                    byte[] content;
                    string type;
                    wll.GetClearCookieResponse(out type, out content);
                    context.Response.ContentType = type;
                    context.Response.OutputStream.Write(content,0,content.Length);
                    context.Response.End();
                    return;

                case "login":
                    if (string.IsNullOrEmpty(context.Request.QueryString["ReturnUrl"])) {
                        redirectUrl = liveLogin + context.Request.QueryString["ReturnUrl"];
                    }
                    else {
                        redirectUrl = liveLogin + context.Request.UrlReferrer;
                    }
                    break;
                default:
                    if (communityUser == null) {
                        // new 
                        LiveCommunityUser.CreateUser(user);
                        communityUser = LiveCommunityUser.GetUser(user);
                    }
                    
                    communityUser.LoggedIn();
                    if (!communityUser.Locked) {
                        FormsAuthentication.SetAuthCookie(communityUser.Id, user.UsePersistentCookie);
                        communityUser.PushProfile();
                        if (string.IsNullOrEmpty(communityUser.Email)) {
                            // first time
                            redirectUrl = string.Format("/_layouts/liveauth-editprofile.aspx?Source={0}", user.Context);
                        }
                        else {                            
                            if (user != null && !string.IsNullOrEmpty(user.Context)) {
                                redirectUrl = user.Context;
                            }
                        }
                    }
                    else {
                        // user is locked out
                        FormsAuthentication.SignOut();
                        redirectUrl = settings.LockedUrl;
                    }
                    break;
            }
            context.Response.Redirect(redirectUrl);
        }

        public bool IsReusable {
            get {
                return false;
            }
        }

    }
}

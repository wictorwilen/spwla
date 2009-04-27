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
using System.Collections.Generic;
using System.Web.Security;

namespace SPCS.WindowsLiveAuth {
    public class LiveMembershipProvider: MembershipProvider {
        private string _ApplicationName = "LiveID";
        public override string ApplicationName {
            get {return _ApplicationName;}
            set {}
        }

        public override bool ChangePassword(string username, string oldPassword, string newPassword) {
            // INFO: cannot change password here. Must be done on live.com
            return false;
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer) {
            // INFO: cannot change password here. Must be done on live.com
            return false;
        }

        public override MembershipUser CreateUser(string username, 
            string password, 
            string email, 
            string passwordQuestion, 
            string passwordAnswer, 
            bool isApproved, 
            object providerUserKey, 
            out MembershipCreateStatus status) {

            if (string.IsNullOrEmpty(username)) {
                status = MembershipCreateStatus.InvalidUserName;
                return null;
            }
            if (string.IsNullOrEmpty(email)) {
                status = MembershipCreateStatus.InvalidEmail;
                return null;
            }
            
            if (LiveCommunityUser.CreateUser(username, email)) {
                // return the the user (not logged in)
                MembershipUser user = GetUser(username, false);
                status = MembershipCreateStatus.Success;
                return user;
                
            }
            else {
                // cannot create user
                status = MembershipCreateStatus.ProviderError;
            }
            return null;

        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData) {
            return LiveCommunityUser.DeleteUser(username);

        }

        public override bool EnablePasswordReset {
            get { return false; }
        }

        public override bool EnablePasswordRetrieval {
            get { return false; }
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords) {
            return findUsers("Email", emailToMatch, pageIndex, pageSize, out totalRecords);
        }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords) {
            return findUsers("UUID", usernameToMatch, pageIndex, pageSize, out totalRecords);
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords) {
            MembershipUserCollection userCollection = new MembershipUserCollection();

            //Find the users and convert the to MembershipUsers
            List<MembershipUser> users = LiveCommunityUser.GetAllUsers().ConvertAll<MembershipUser>(
                user => ConvertFromLiveCommunityUser(user));

            totalRecords = users.Count;

            // Get the correct range
            users = users.GetRange(pageIndex * pageSize, pageSize);

            // add em to the collection
            users.ForEach(user => userCollection.Add(user));

            // return em
            return userCollection;
        }

        public override int GetNumberOfUsersOnline() {
            return 0;
        }

        public override string GetPassword(string username, string answer) {
            throw new NotImplementedException();
        }

        public override MembershipUser GetUser(string username, bool userIsOnline) {
            return ConvertFromLiveCommunityUser(LiveCommunityUser.GetUser(username));
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline) {
            return ConvertFromLiveCommunityUser(LiveCommunityUser.GetUser(providerUserKey.ToString()));
        }

        public override string GetUserNameByEmail(string email) {
            //Find the users and convert the to MembershipUsers
            List<LiveCommunityUser> users = LiveCommunityUser.FindUsers("Email", email);

            if (users.Count == 0) {
                return string.Empty;
            }

            return users[0].Email;
        }

        public override int MaxInvalidPasswordAttempts {
            get { return 4; }
        }

        public override int MinRequiredNonAlphanumericCharacters {
            get { return 1; }
        }

        public override int MinRequiredPasswordLength {
            get { return 1; }
        }

        public override int PasswordAttemptWindow {
            get { return  5; }
        }

        public override MembershipPasswordFormat PasswordFormat {
            get { return MembershipPasswordFormat.Clear; }
        }

        public override string PasswordStrengthRegularExpression {
            get { return string.Empty; }
        }

        public override bool RequiresQuestionAndAnswer {
            get { return false; }
        }

        public override bool RequiresUniqueEmail {
            get { return true; }
        }

        public override string ResetPassword(string username, string answer) {
            throw new NotImplementedException();
        }

        public override bool UnlockUser(string userName) {
            return LiveCommunityUser.Unlock(userName);
        }

        public override void UpdateUser(MembershipUser user) {
            LiveCommunityUser liveUser = LiveCommunityUser.GetUser(user.UserName);
            liveUser.Description = user.Comment;
            liveUser.LastLogin = user.LastLoginDate;
            liveUser.Locked = user.IsLockedOut;
            LiveCommunityUser.UpdateUser(liveUser);
        }

        public override bool ValidateUser(string username, string password) {
            LiveCommunityUser liveUser = LiveCommunityUser.GetUser(username);
            return liveUser != null;
        }
        #region Privates
        private MembershipUser ConvertFromLiveCommunityUser(LiveCommunityUser user) {
            if (user == null) {
                return null;
            }
            return new MembershipUser(this.ApplicationName,
                        user.Id,
                        user.Id,
                        user.Email,
                        null,
                        user.Description,
                        user.Approved,
                        user.Locked,
                        user.Created,
                        user.LastLogin,
                        user.LastLogin,
                        DateTime.Now,
                        user.LockedOn);
        }

        private MembershipUserCollection findUsers(string field, string match, int pageIndex, int pageSize, out int totalRecords) {
            MembershipUserCollection userCollection = new MembershipUserCollection();

            //Find the users and convert the to MembershipUsers
            List<MembershipUser> users = LiveCommunityUser.FindUsers(field, match).ConvertAll<MembershipUser>(
                user => ConvertFromLiveCommunityUser(user));

            totalRecords = users.Count;
            if (totalRecords > 0) {
                // Get the correct range
                users = users.GetRange(pageIndex * pageSize, pageSize);

                // add em to the collection
                users.ForEach(user => userCollection.Add(user));
            }
            // return em
            return userCollection;
        }
        #endregion

    }
}

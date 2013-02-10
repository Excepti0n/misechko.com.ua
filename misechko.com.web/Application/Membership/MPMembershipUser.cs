using System;
using System.Collections.Generic;
using System.Web.Security;

namespace misechko.com.Application.Membership
{
    public class MPMembershipUser: MembershipUser
    {
        private string _displayName;

        public string DisplayName { get { return _displayName; } }
        public List<string> Roles { get; set; } 

        public MPMembershipUser(string providername,
                                  string username,
                                  object providerUserKey,
                                  string email,
                                  string passwordQuestion,
                                  string comment,
                                  bool isApproved,
                                  bool isLockedOut,
                                  DateTime creationDate,
                                  DateTime lastLoginDate,
                                  DateTime lastActivityDate,
                                  DateTime lastPasswordChangedDate,
                                  DateTime lastLockedOutDate,
                                  string displayName) :
                                  base(providername,
                                       username,
                                       providerUserKey,
                                       email,
                                       passwordQuestion,
                                       comment,
                                       isApproved,
                                       isLockedOut,
                                       creationDate,
                                       lastLoginDate,
                                       lastActivityDate,
                                       lastPasswordChangedDate,
                                       lastLockedOutDate)
        {
            _displayName = displayName;
        }

        public void ChangeDisplayName(string newDisplayName)
        {
            _displayName = newDisplayName;
        }
    }
}
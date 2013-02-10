using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace misechko.com.data.Entities
{
    public class SiteUser: IdableEntity
    {
        [Required]
        public string UserName { get; set; }

        public string DisplayName { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }


        public string Email { get; set; }

        public int PasswordFailuresSinceLastSuccess { get; set; }

        public DateTime? LastPasswordFailureDate { get; set; }
        public DateTime? LastActivityDate { get; set; }
        public DateTime? LastLockoutDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public String ConfirmationToken { get; set; }
        public DateTime? CreateDate { get; set; }
        public Boolean IsLockedOut { get; set; }
        public DateTime? LastPasswordChangedDate { get; set; }
        public String PasswordVerificationToken { get; set; }
        public DateTime? PasswordVerificationTokenExpirationDate { get; set; }

        public virtual IList<UserRole> UserRoles { get; set; }
    }
}

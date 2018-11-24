using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CloudApiVietnam.Models
{
    public class UserInfo
    {
        public  string Email { get; set; }

        public  ICollection<IdentityUserRole> Roles { get; set; }
 
        public  string Id { get; set; }
  
        public  string UserName { get; set; }
    }
}
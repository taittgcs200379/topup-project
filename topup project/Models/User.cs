using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace topup_project.Models
{
    public class User
    {
        public User(ApplicationUser user) 
        {
            Id = user.Id;
            Name = user.UserName;
            
            
        }
        public string Id { get; set; }
        public string Name { get; set; }
        
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace muscshop.Models
{
    public class User
    {
       
        public int UserId { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }

        public bool Active { get; set; } = false;

        public Guid Confirmation { get; set; }

        public Guid PassRecovery { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace EcoTrend_Industry_Project.ViewModels
{
    public class ContactEmailVM
    {
        [DisplayName("Full Name")]
        public string FullName { get; set; }
        [DisplayName("Email Address")]
        public string EmailAddress { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DailyAww.Models
{
    public class OnDemandAwwViewModel
    {
        [EmailAddress]
        public string CustomEmail { get; set; }

        public List<OnDemandListItem> ModelList { get; set; }
    }

    public class OnDemandListItem
    {
        public bool Selected { get; set; }
        public int PersonId { get; set; }
        public string PersonName { get; set; }
    }
}
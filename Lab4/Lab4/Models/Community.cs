using Assignment2.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Lab4.Models
{
    public class Community
    {
        [Required]
        
        [Display(Name = "Registration Number")]
        public string Id { get; set; }


        [Required]
        [StringLength(maximumLength: 50, MinimumLength = 3)]
        public string Title
        {
            get;
            set;
        }

        [DataType(DataType.Currency)]
        [Column(TypeName = "money")]

        public float Budget
        {
            get;
            set;
        }

        public IEnumerable<CommunityMembership> Membership
        {
            get;
            set;
        }

        public IEnumerable<CommunityAdvertisement> Advertisements
        {
            get;
            set;
        }




    }
}

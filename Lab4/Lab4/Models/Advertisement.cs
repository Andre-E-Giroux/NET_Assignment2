using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Lab4.Models;

namespace Assignment2.Models
{
    public class Advertisement
    {
        public int AdvertisementId
        {
            get; set;
        }

        [Required]
        [StringLength(50)]
        [Column("FileName")]
        [Display(Name = "File Name")]
        public string FileName
        {
            get; set;
        }
        [Required]
        [DataType(DataType.Url)]
        public string Url
        {
            get; set;
        }

        public CommunityAdvertisement CommunityAdvertisment
        {
            get;
            set;
        }


    }
}

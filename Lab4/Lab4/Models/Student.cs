using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Lab4.Models
{
    public class Student
    {
        public int Id
        {
            get;
            set;
        }

        [Required]
        [StringLength(maximumLength: 50)]
        [Display(Name = "LastName")]
        public string LastName
        {
            get;
            set;
        }

        [Required]
        [StringLength(maximumLength: 50)]
        [Display(Name = "FirstName")]
        public string FirstName
        {
            get;
            set;
        }


        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Enrollment Date")]
        public DateTime EnrollmentDate
        {
            get;
            set;
        }


        private string FullName;

        public string GetFullName()
        {
            if(FullName ==null)
            {
                SetFullName();
            }

            return FullName;
        }

        public void SetFullName()
        {
            FullName = LastName + ", " + FirstName;
        }


        public IEnumerable<CommunityMembership> Membership
        {
            get;
            set;
        }


        /*
        public CommunityMembership Membership
        {
            get;
            set;
        }*/
    }
}

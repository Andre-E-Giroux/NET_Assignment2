using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Assignment2.Models.ViewModels
{
    public class FileInputViewModel
    {
        public string CommunityId { get; set; }
        public string CommunityTitle { get; set; }
        public IFormFile File { get; set; }
    }
}

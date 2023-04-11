using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;

namespace topup_project.Models
{
    

    public class Topic
    {
       public int Id { get; set; }
       public string Name { get; set; }
        [Display(Name="First Deadline")]
       public DateTime? FirstDeadLine { get; set; }
        [Display(Name = "Second Deadline")]
        public DateTime? LastDeadLine { get; set; }
    }
}
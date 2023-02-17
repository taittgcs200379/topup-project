using System;
using System.Data.Entity;
using System.Linq;

namespace topup_project.Models
{
    

    public class Topic
    {
       public int Id { get; set; }
       public string Name { get; set; }
       public DateTime? FirstDeadLine { get; set; }
       public DateTime? LastDeadLine { get; set; }
    }
}
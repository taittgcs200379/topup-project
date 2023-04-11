using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace topup_project.Models
{
    public class React
    {
        public int Id { get; set; }

        public int Reaction { get; set; }
        public string UserId { get; set; }
        public int IdeaId   { get; set; }
    }
}
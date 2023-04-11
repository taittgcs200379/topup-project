using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace topup_project.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime? Date { get; set; }
        public string UserId { get; set; }
        public int IdeaId { get; set; }
    }
}
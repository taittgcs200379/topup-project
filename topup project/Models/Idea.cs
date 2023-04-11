using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace topup_project.Models
{
    public class Idea
    {
        public int Id { get; set; }
        public string Text { get; set; }
        [DataType(DataType.Upload)]
        [Display(Name="File")]
        
        public string  FilePath { get; set; }
        [Display(Name ="Date")]
        public DateTime? DateTime { get; set; }
        [Display(Name ="User")]
        public string UserId { get; set; }
        [Display(Name ="Categroy")]
        [Required(ErrorMessage ="Please Choose Category!")]
        public int CategoryId { get; set; }
        public int TopicId { get; set; }
        [CheckBoxRequired(ErrorMessage ="Please Check Again!")]
        public bool IsAccepted { get; set; }
       

    }
    public class CheckBoxRequired : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var check = (Idea)validationContext.ObjectInstance;
            if ( check.IsAccepted == false)
            {
                return new ValidationResult(ErrorMessage == null ? "Please Check Again!" : ErrorMessage);
            }
            
               return ValidationResult.Success;
            
            
        }
    }


}
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace UniTrainingSystem.VIewModels
{
    public class EditStudentViewModel
    {
        [Display(Name = "Email the New Student")]
        [EmailAddress]
        public string? Email { get; set; }
        [Display(Name = "Student Password")]
        public string? Password { get; set; }
        [Display(Name = "Name of Student")]
        public string? FullName { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace UniTrainingSystem.VIewModels
{
    public class AddNewStudentViewModel
    {
        [Display(Name ="Email the New Student")]
        [EmailAddress]
        public string Email { get; set; }
        [Display(Name = "Student Password")]
        public string Password { get; set; }
        [Display(Name = "Name of Student")]
        public string FullName { get; set; }

    }
}

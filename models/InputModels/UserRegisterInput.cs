using System.ComponentModel.DataAnnotations;

namespace datecounter.Models{

    public class UserRegisterInput{
        [Required]
        public string Email {get;set;}
        [Required]
        public string Password{get;set;}

    }
}
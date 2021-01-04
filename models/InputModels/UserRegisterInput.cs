using System.ComponentModel.DataAnnotations;

namespace datecounter.Models{

    public class UserRegisterInput{
        [Required]
        [EmailAddress]
        public string email {get;set;}
        [Required]
        [MinLength(6)]
        public string password{get;set;}

    }
}
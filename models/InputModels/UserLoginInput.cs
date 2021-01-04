using System.ComponentModel.DataAnnotations;

namespace datecounter.Models{


    public class UserLoginInput{
        [Required]
        public string email{get;set;}
        [Required]
        public string password{get;set;}
    }
}
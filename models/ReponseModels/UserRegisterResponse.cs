using System.ComponentModel.DataAnnotations;

namespace datecounter.Models{
    public class UserRegisterRepsonse{
        [Required]
        public string token{get;set;}
    }
}
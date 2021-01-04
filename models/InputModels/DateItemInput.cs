using System.ComponentModel.DataAnnotations;
using System;

namespace datecounter.Models{

    public class DateItemInput{
        [Required]
        public string title{get;set;}
        [DataType(DataType.Date)]
        public DateTime date{get;set;}
    }
}
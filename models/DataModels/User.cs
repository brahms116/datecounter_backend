

namespace datecounter.Models{

    public class User{
        public int id {get;set;}
        public string email {get;set;}
        public string password {get;set;}
        public bool is_email_confirmed {get;set;}
    }
}
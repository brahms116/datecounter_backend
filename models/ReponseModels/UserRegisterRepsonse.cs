namespace datecounter.Models{
    public class UserRegisterResponseData{
        public User user{get;set;}
        public string token{get;set;}

    }

    public class UserRegisterResponse:ApiResponse{
        public UserRegisterResponseData payload{get;set;}
    }
}
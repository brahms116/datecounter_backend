

namespace datecounter.Models{

    public class UserLoginResponseData{
        public string token{get;set;}
    }

    public class UserLoginResponse:ApiResponse{
        public UserLoginResponseData payload{get;set;}
    }
}
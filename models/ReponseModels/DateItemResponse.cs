

namespace datecounter.Models{
    public class DateItemResponseData{
        public DateItem item{get;set;}
    }

    public class DateItemResponse:ApiResponse{
        public DateItemResponseData payload{get;set;}
    }
}
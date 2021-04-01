using System.Collections.Generic;

namespace datecounter.Models{
    public class GetItemsResponseData{
        public DateItem coverItem{get;set;}
        public IList<DateItem> items{get;set;}
    }

    public class GetItemsResponse:ApiResponse{
        public GetItemsResponseData payload{get;set;}
    }


}
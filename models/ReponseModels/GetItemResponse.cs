using System.Collections.Generic;

namespace datecounter.Models{
    public class GetItemResponse{
        public DateItem coverItem{get;set;}
        public IList<DateItem> items{get;set;}
    }
}
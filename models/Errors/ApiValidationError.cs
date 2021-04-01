using System.Collections.Generic;

namespace datecounter.Models{
    public class ApiValidationError:ApiError{
        public IList<ApiValidationErrorItem> errors {get;set;}
    }


    public class ApiValidationErrorItem{
        public string field{get;set;}
        public string msg {get;set;}
    }
}
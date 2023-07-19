using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Models.Errors
{
    public class ErrorDto
    {
        public string PropertyName { get; set; }
        public List<string> ErrorMessages { get; set; }
        public ErrorDto(string propertyName, List<string> errorMessages)
        {
            PropertyName=propertyName;
            
            ErrorMessages=errorMessages;
        }
    }
}
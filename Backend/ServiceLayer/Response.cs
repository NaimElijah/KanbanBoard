using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.ServiceLayer
{
    public class Response
    {

        public string? ErrorMessage { get; set; }
        public object? ReturnValue { get; set; }

        public Response(string errorMessage, object returnValue)
        {
            ErrorMessage = errorMessage;
            ReturnValue = returnValue;
        }

        public Response()
        {
            // default constructor
        }

        public string GetSerializedVersion()
        {
            return JsonSerializer.Serialize(this);
        }


        public bool ErrorOccured { get => ErrorMessage != null; }

    }
}

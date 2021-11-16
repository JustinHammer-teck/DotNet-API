using System.Collections.Generic;

namespace DotNet_API.Contracts.V1.Responses
{
    public class AuthFailResponse
    {
        public IEnumerable<string> Errors { get; set; }
    }
}
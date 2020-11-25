using System;
using System.Net;

namespace DogApi
{
    public class DogException : Exception
    {
        public HttpStatusCode StatusCode { get; set; }

        public string Content { get; set; }

        public DogException(HttpStatusCode statusCode, string content) : base($"Received a {statusCode} error from the API.")
        {
            this.StatusCode = statusCode;
            this.Content = content;
        }
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Serilog.RequestResponse.Extensions.Models
{
    public class CustomException : Exception
    {
        public int StatusCode { get; protected set; }
        public object Dados { get; set; }

        public CustomException(int statusCode = StatusCodes.Status500InternalServerError)
        {
            StatusCode = statusCode;
            Dados = new { Mensagem = Message };
        }

        public CustomException(object dados, int statusCode = StatusCodes.Status500InternalServerError)
        {
            StatusCode = statusCode;
            Dados = dados;
        }

        public CustomException(string mensagem, int statusCode = StatusCodes.Status500InternalServerError)
        {
            StatusCode = statusCode;
            Dados = new { Mensagem = mensagem };
        }
    }
}

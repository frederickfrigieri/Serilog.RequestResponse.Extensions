using Microsoft.AspNetCore.Http;
using Serilog.RequestResponse.Extensions.Models;
using System;

namespace Serilog.RequestResponse.Extensions.Exceptions
{
    public class DomainException : CustomException
    {
        public DomainException(int statusCode = StatusCodes.Status400BadRequest) : base(statusCode)
        {
            Dados = new { Mensagem = "Erro de negócio." };
        }

        public DomainException(object dados, int statusCode = StatusCodes.Status400BadRequest) : base(dados, statusCode)
        {
        }

        public DomainException(string mensagem, int statusCode = StatusCodes.Status400BadRequest) : base(mensagem, statusCode)
        {
        }
    }
}

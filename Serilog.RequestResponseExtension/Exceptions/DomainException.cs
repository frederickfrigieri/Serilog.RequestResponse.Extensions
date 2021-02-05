using Microsoft.AspNetCore.Http;
using Serilog.RequestResponse.Extensions.Models;
using System;

namespace Serilog.RequestResponse.Extensions.Exceptions
{
    public sealed class DomainException : CustomException
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

        public DomainException(object dados, Exception innerException, int statusCode = 500) : base(dados, innerException, statusCode)
        {
        }

        public DomainException(string mensagem, Exception innerException, int statusCode = 500) : base(mensagem, innerException, statusCode)
        {
        }
    }
}

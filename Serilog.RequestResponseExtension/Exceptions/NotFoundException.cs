using Microsoft.AspNetCore.Http;
using Serilog.RequestResponse.Extensions.Models;
using System;

namespace Serilog.RequestResponse.Extensions.Exceptions
{
    public sealed class NotFoundException : CustomException
    {
        public NotFoundException(int statusCode = StatusCodes.Status404NotFound) : base(statusCode)
        {
            Dados = new { Mensagem = "Recurso solicitado não encontrado." };
        }

        public NotFoundException(object dados, int statusCode = StatusCodes.Status404NotFound) : base(dados, statusCode)
        {
        }

        public NotFoundException(string mensagem, int statusCode = StatusCodes.Status404NotFound) : base(mensagem, statusCode)
        {
        }

        public NotFoundException(object dados, Exception innerException, int statusCode = 500) : base(dados, innerException, statusCode)
        {
        }

        public NotFoundException(string mensagem, Exception innerException, int statusCode = 500) : base(mensagem, innerException, statusCode)
        {
        }
    }
}

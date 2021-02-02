using Microsoft.AspNetCore.Http;
using Serilog.RequestResponse.Extensions.Models;

namespace Serilog.RequestResponse.Extensions.Exceptions
{
    public class NotAuthorizedException : CustomException
    {
        public NotAuthorizedException(int statusCode = StatusCodes.Status401Unauthorized) : base(statusCode)
        {
            Dados = new { Mensagem = "Dados de acesso inválidos." };
        }

        public NotAuthorizedException(object dados, int statusCode = 500) : base(dados, statusCode)
        {
        }

        public NotAuthorizedException(string mensagem, int statusCode = 500) : base(mensagem, statusCode)
        {
        }
    }
}

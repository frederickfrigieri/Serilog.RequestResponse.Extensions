using Serilog.RequestResponse.Extensions.Models;
using System;
using System.Linq;
using System.Reflection;

namespace Serilog.RequestResponse.Extensions
{
    public static class CustomExceptionExtension
    {
        public static CustomException ConstroiCustomExceptionDetails(this Exception exceptionCurrent)
        {
            var innerException = exceptionCurrent.InnerException;

            while (innerException != null)
            {
                exceptionCurrent = innerException;
                innerException = innerException.InnerException;
            }

            Type t = exceptionCurrent.GetType();

            if (t.Name.Equals("CustomException") || t.BaseType.Name.Equals("CustomException"))
            {
                PropertyInfo[] props = t.GetProperties();
                var statusCodeAux = props.Where(x => x.Name.Equals("StatusCode")).SingleOrDefault();
                var dadosAux = props.Where(x => x.Name.Equals("Dados")).SingleOrDefault();
                int statusCode = int.Parse(statusCodeAux.GetValue(exceptionCurrent).ToString());
                object dados = dadosAux.GetValue(exceptionCurrent);

                return new CustomException(dados, exceptionCurrent, statusCode);
            }

            return new CustomException(exceptionCurrent.Message, exceptionCurrent);
        }
    }
}

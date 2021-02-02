using Serilog.RequestResponse.Extensions.Models;
using System;
using System.Linq;
using System.Reflection;

namespace Serilog.RequestResponse.Extensions
{
    public static class CustomExceptionExtension
    {
        public static CustomException ConstroiCustomExceptionDetails(this Exception excpetionCurrent)
        {
            Type t = excpetionCurrent.GetType();

            if (t.BaseType.Name.Equals("CustomException"))
            {
                PropertyInfo[] props = t.GetProperties();
                var statusCodeAux = props.Where(x => x.Name.Equals("StatusCode")).SingleOrDefault();
                var dadosAux = props.Where(x => x.Name.Equals("Dados")).SingleOrDefault();

                if (statusCodeAux != null)
                {
                    int statusCode = int.Parse(statusCodeAux.GetValue(excpetionCurrent).ToString());
                    object dados = dadosAux.GetValue(excpetionCurrent);

                    return new CustomException(dados, statusCode);
                }
            }

            return new CustomException(excpetionCurrent.Message);

        }
    }
}

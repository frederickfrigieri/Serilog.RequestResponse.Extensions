﻿using Serilog.RequestResponse.Extensions.Models;
using System;
using System.Linq;
using System.Reflection;

namespace Serilog.RequestResponse.Extensions
{
    public static class CustomExceptionExtension
    {
        public static CustomException ConstroiCustomExceptionDetails(this Exception exceptionCurrent)
        {
            Type t = exceptionCurrent.GetType();
            var innerException = exceptionCurrent.InnerException;

            if (t.Name.Equals("CustomException") || t.BaseType.Name.Equals("CustomException"))
            {
                PropertyInfo[] props = t.GetProperties();
                var statusCodeAux = props.Where(x => x.Name.Equals("StatusCode")).SingleOrDefault();
                var dadosAux = props.Where(x => x.Name.Equals("Dados")).SingleOrDefault();

                if (statusCodeAux != null && !statusCodeAux.Equals(0))
                {
                    int statusCode = int.Parse(statusCodeAux.GetValue(exceptionCurrent).ToString());
                    object dados = dadosAux.GetValue(exceptionCurrent);

                    return new CustomException(dados, innerException, statusCode);
                }
            }

            return new CustomException(exceptionCurrent.Message, innerException);

        }
    }
}

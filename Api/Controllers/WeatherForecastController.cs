using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Serilog.RequestResponse.Extensions.Exceptions;
using Serilog.RequestResponse.Extensions.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/WeatherForecast")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        public WeatherForecastController()
        {
        }

        [HttpGet]
        [Route("ExceptionComTryCatch")]
        public IActionResult ExceptionComTryCatch()
        {
            (new ErroService()).GerarErro1();

            return Ok();
            //try
            //{
            //    throw new Exception("Exception no try");
            //}
            //catch (Exception e)
            //{
            //    throw new Exception("Exception no catch", e);
            //}
        }

        [HttpGet]
        [Route("ExceptionComObjetoEInnerException")]
        public IActionResult ExceptionComObjetoEInnerException()
        {
            throw new DomainException(new { Codigo = 1234567 }, new Exception("Inner Exception"), 200);
        }

        [HttpGet]
        [Route("ExceptionComObjeto")]
        public IActionResult ExceptionComObjeto()
        {
            throw new DomainException(new { Codigo = 1234567 }, 200);
        }

        [HttpGet]
        [Route("ExceptionComMensagemEInnerException")]
        public IActionResult ExceptionComMensagemEInnerException()
        {
            throw new DomainException("Mensagem do Throw", new Exception("Com Inner Exception"));
        }

        [HttpGet]
        [Route("ExceptionComMensagem")]
        public IActionResult ExceptionComMensagem()
        {
            throw new DomainException("Mensagem do Throw");
        }

        [HttpGet]
        [Route("ExceptionSemParametro")]
        public IActionResult ExceptionSemParametro()
        {
            throw new DomainException();
        }


        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}

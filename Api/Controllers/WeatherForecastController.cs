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
        public IEnumerable<WeatherForecast> Get()
        {
            //throw new DomainException();
            //throw new DomainException(new { Codigo = 1234567 }, 200);
            //throw new DomainException("Mensagem do Throw");

            throw new Exception("Teste exception", new DomainException());

            //try
            //{
            //    throw new CustomException("Lançando uma exception (linha 32)");
            //}
            //catch (Exception e)
            //{
            //    throw new Exception("Exception nova", e);
            //}

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

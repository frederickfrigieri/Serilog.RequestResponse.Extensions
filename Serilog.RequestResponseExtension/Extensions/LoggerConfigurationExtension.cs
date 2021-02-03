﻿using Serilog.Events;
using Serilog.Exceptions;
using Serilog.RequestResponseExtension.Models;
using Serilog.Sinks.Elasticsearch;
using Serilog.Sinks.MSSqlServer;
using System;
using System.Reflection;

namespace Serilog.RequestResponseExtension.Extensions
{
    public static class LoggerConfigurationExtension
    {
        private static MSSqlServerSinkOptions MsSqlConfig(string tabela, string schema, bool autoCreateTable) => new MSSqlServerSinkOptions
        {
            TableName = tabela,
            AutoCreateSqlTable = autoCreateTable,
            SchemaName = string.IsNullOrEmpty(schema) ? "dbo" : schema
        };

        public static LoggerConfiguration WithSqlServer(this LoggerConfiguration loggerConfiguration, SerilogSqlServerConfig serilogMSSqlModel)
        {
            var skinOptions = MsSqlConfig(serilogMSSqlModel.Tabela, serilogMSSqlModel.Schema, serilogMSSqlModel.CriaTabelaQuandoNaoTiver);

            return loggerConfiguration.WriteTo.MSSqlServer(serilogMSSqlModel.Connectionstring, skinOptions);
        }

        public static LoggerConfiguration WithES(this LoggerConfiguration loggerConfiguration, SerilogElasticsearchConfig configuration)
        {
            var skinOptions = new ElasticsearchSinkOptions(new Uri(configuration.Url))
            {
                AutoRegisterTemplate = true,
                IndexFormat = configuration.Index,
                ModifyConnectionSettings = (c) => string.IsNullOrEmpty(configuration.Username) ? c.BasicAuthentication(configuration.Username, configuration.Password)
                .DisableAutomaticProxyDetection(configuration.DisableProxy) : null
            };

            if (configuration.UseDatetimeInIndexName)
                skinOptions.IndexFormat += $"-{DateTime.UtcNow:yyyy-MM-dd}";

            return loggerConfiguration.WriteTo.Elasticsearch(skinOptions);
        }

        public static LoggerConfiguration CreateDefaultInstance(this LoggerConfiguration loggerConfiguration, string projectName)
        {
            loggerConfiguration.MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Fatal)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Model.Validation", LogEventLevel.Fatal)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Infrastructure", LogEventLevel.Fatal)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Connection", LogEventLevel.Fatal)
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithExceptionDetails()
                .Enrich.WithAssemblyName()
                .Enrich.WithAssemblyVersion()
                .Enrich.WithProperty("ProjectName", projectName);


            return loggerConfiguration;
        }
    }
}

# Introdução 
Este repositório contém pacotes usados em projetos internos. 

# Pacotes
Lista de pacotes disponíveis
- Serilog.RequestResponse.Extensions


# Como utilizar o pacote Serilog.RequestResponse.Extensions

# Passo 1 - Instalando o Pacote
Abra o projeto que for utilizar (ex: Api) e instale o pacote

Install-Package Serilog.RequestResponse.Extensions -Version 1.0.0

# Passo 2 - Configurando projeto 

- Adicione o código abaixo no appsetting

```
  "Serilog": {
    "SqlServer": {
      "ConnectionString": "Data Source=localdb;Initial Catalog=Temporaria;User ID=sa;Password=p@ssword",
      "Tabela": "LogRequestResponse",
      "Schema": "dbo",
      "CriaTabelaQuandoNaoTiver": true
    },
    "Elasticsearch": {
      "Url": "http://localhost:9200",
      "Index": "log-requisicoes",
      "Username": "",
      "Password": "",
      "DisableProxy": true,
      "UseDatetimeInIndexName": true
    }
  }

  ** Caso UseDatetimeInIndexName for true ele irá concatenar a data yyy-MM-dd no nome do Index (log-requisicoes-2021-01-10)
```

** Caso não for inserir no banco ou elastic pode ignorar a configuração do SqlServe e vice-versa.

- Na classe Startup.cs adicione o using do pacote adicionado

```
    using Serilog;
    using Serilog.RequestResponseExtension.Extensions;
    using Serilog.RequestResponseExtension.Models;
```

- Na classe Startup.cs altere o construtor

```
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            var sqlServerConfig = configuration.GetSection("Serilog:SqlServer").Get<SerilogSqlServerConfig>();
            var esConfig = configuration.GetSection("Serilog:Elasticsearch").Get<SerilogElasticsearchConfig>();
            Log.Logger = new LoggerConfiguration()
                .CreateDefaultInstance("Nome do projeto")
                .WithSqlServer(sqlServerConfig)
                .WithES(esConfig)
                .CreateLogger();
        }
```

** Caso não for inserir no banco ou elastic pode ignorar a configuração do SqlServe e vice-versa.

- Na classe Startup.cs configure o serviço do Serilog

```
        public void ConfigureServices(IServiceCollection services)
        {
            services.RegisterLogRequestResponseService();
        }
```

- Na classe Startup.cs configure o middleware

```
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.RegisterLogRequestResponseMiddleware();
        }
```

# Passo 3 - Verificando 

- Rode o projeto
- Consuma um endpoint
- Realize uma pesquisa no Elastic ou SQL Server no endereço que foi configurado no appsettings e verifique o registro que foi gravado

Qualquer dúvida, problema ou sugestão pode procurar o Fred, Eduardo ou Felipe

THE END

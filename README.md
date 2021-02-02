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
using Serilog.RequestResponse.Extensions;
using Serilog.RequestResponse.Extensions.Models;
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
            services.RegisterFilterException();
            services.RegisterLogRequestResponseService();
        }
```

- Na classe Startup.cs configure o middleware

```
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.RegisterLogRequestResponseMiddleware(new SerilogOptions { UseFilterException = false });
        }
```

** UseFilterException = False => Vai gravar um log quando a requisição chegar e outra quando enviar a resposta ao cliente ou seja registros (request e response). Caso aconteça um erro entre a entrada e a saída o request está sendo logado e o erro também menos a saída devido a exceção

** UseFilterException = True => Vai gravar apenas um log contendo a entrada, saída e a exceção caso ocorra, para isso registre o serviço do filtro de exceção.


# Passo 3 - Verificando 

- Rode o projeto
- Consuma um endpoint
- Realize uma pesquisa no Elastic ou SQL Server no endereço que foi configurado no appsettings e verifique o registro que foi gravado


# Customizando as Exceções

É possível customizar erros e respostas para o cliente para isso é necessário herdar a classe CustomException de Serilog.RequestResponse.Extensions.Models e implementar seus construtores;

```
    public class DomainException : CustomException
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
    }
```
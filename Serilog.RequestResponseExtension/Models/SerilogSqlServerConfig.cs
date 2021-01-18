namespace Serilog.RequestResponseExtension.Models
{
    public class SerilogSqlServerConfig
    {
        public string Connectionstring { get; set; }
        public string Tabela { get; set; } = "LogRequestResponse";
        public string Schema { get; set; } = "dbo";
        public bool CriaTabelaQuandoNaoTiver { get; set; } = true;
    }
}

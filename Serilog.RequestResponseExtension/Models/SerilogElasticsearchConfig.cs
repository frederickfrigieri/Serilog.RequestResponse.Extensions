namespace Serilog.RequestResponseExtension.Models
{
    public class SerilogElasticsearchConfig
    {
        public string Url { get; set; }
        public string Index { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool DisableProxy { get; set; } = true;
        public bool UseDatetimeInIndexName { get; set; } = true;
    }
}

namespace EmailSenderService.SmtpConfiguration
{
    public class SmtpServer
    {
        public string Server { get; set; }
        public int Port { get; set; }
        public bool IsHttps { get; set; }
    }
}

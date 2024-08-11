namespace TheBugTracker.Models
{
    public class MailSettings
    {
        //This will allow us to easily manipulate the
        //configurations that we're going to read from the appSettings.json
        public string Mail { get; set; }
        public string DisplayName { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
    }
}

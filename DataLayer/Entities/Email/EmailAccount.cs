using System.Diagnostics.CodeAnalysis;

namespace DataLayer.Entities.Email
{
    public class EmailAccount
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int SmtpPortIncoming { get; set; }
        public int SmtpPortOutgoing { get; set; }
        public string SmtpServerIncoming { get; set; }
        public string SmtpServerOutgoing { get; set; }
        [AllowNull]
        public bool? IsConnectionSuccessfully { get; set; }
    }
}

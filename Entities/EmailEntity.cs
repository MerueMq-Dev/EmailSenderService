
namespace EmailSenderService.Entities
{

    /// <summary>
    /// A class that describes an email table in a database
    /// </summary>
    public class EmailEntity
    {
        public EmailEntity(string subject, string body,string errorMessage, string result)
        {
            Subject = subject;
            Body = body;
            ErrorMessage = errorMessage;
            Result = result;
        }

        public EmailEntity()
        {
                
        }

        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public long Id { get; set; }

        /// <summary>
        /// Subject email
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Email message
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// List of email addresses to which the email will be sent
        /// </summary>
        public List<EmailAdressEntity> Recipients { get; set; } = new List<EmailAdressEntity>();

        /// <summary>
        /// Date the email was created
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        /// <summary>
        /// The result of sending an email. Maybe (values ​​OK, Failed)
        /// </summary>
        public string Result { get; set; }

        /// <summary>
        /// Error message if an error occurred while sending an email
        /// </summary>
        public string ErrorMessage { get; set; }

    }
}

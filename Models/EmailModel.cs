using System.ComponentModel.DataAnnotations;

namespace EmailSenderService.Models
{

    /// <summary>
    /// This class describes incoming data from the user.
    /// </summary>
    public class EmailModel
    {

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
        public List<string> Recipients { get; set; }
    }
}

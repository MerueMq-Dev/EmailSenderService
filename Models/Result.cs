namespace EmailSenderService.Models
{

    /// <summary>
    /// A class that describes the state of sending an email
    /// </summary>
    public static class Result
    {
        /// <summary>
        /// Email sent successfully
        /// </summary>
        public const string Ok = "Ok";

        /// <summary>
        /// An error occurred while sending the email
        /// </summary>
        public const string Error = "Error";
    }
}

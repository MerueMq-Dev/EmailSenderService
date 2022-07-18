namespace EmailSenderService.Entities
{

    /// <summary>
    /// A class that describes an e-mail adress table in a database
    /// </summary>
    public class EmailAdressEntity
    {

        public EmailAdressEntity()
        {

        }

        public EmailAdressEntity(string emailAdress)
        {
            EmailAdress = emailAdress;
        }

        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public long Id { get; set; }

        /// <summary>
        /// E-mail address
        /// </summary>
        public string EmailAdress { get; set; }

        /// <summary>
        /// Navigation proprty EF Core
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public long EmailEntityId { get; set; }


        /// <summary>
        /// Navigation proprty EF Core
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public EmailEntity EmailEntity { get; set; }
    }
}

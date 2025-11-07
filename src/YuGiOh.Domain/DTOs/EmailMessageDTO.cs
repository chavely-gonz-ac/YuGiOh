namespace YuGiOh.Domain.DTOs
{
    /// <summary>
    /// Represents the content of an email message to be sent through the system.
    /// </summary>
    public class EmailMessageDTO
    {
        /// <summary>
        /// Gets or sets the destination email address of the recipient.
        /// </summary>
        public required string ToAddress { get; set; }

        /// <summary>
        /// Gets or sets the subject line of the email.
        /// </summary>
        public required string Subject { get; set; }

        /// <summary>
        /// Gets or sets the main content of the email message.
        /// </summary>
        public required string Body { get; set; }

        /// <summary>
        /// Indicates whether the email content is formatted as HTML.
        /// If false, the message body will be treated as plain text.
        /// </summary>
        public bool IsHTML { get; set; }

        /// <summary>
        /// Optional plain-text version of the email body, used for non-HTML email clients.
        /// </summary>
        public string? PlainTextBody { get; set; }
    }
}

using YuGiOh.Domain.DTOs;

namespace YuGiOh.Domain.Services
{
    /// <summary>
    /// Defines a contract for sending email messages asynchronously.
    /// </summary>
    public interface IEmailSender
    {
        /// <summary>
        /// Sends an email message asynchronously using the configured delivery mechanism.
        /// </summary>
        /// <param name="emailData">The structured email message to send.</param>
        Task SendMailAsync(EmailMessageDTO emailData);
    }
}

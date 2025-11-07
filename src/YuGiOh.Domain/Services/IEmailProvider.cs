using YuGiOh.Domain.DTOs;

namespace YuGiOh.Domain.Services
{
    /// <summary>
    /// Provides functionality for generating structured email messages
    /// based on specific system events or user actions.
    /// </summary>
    public interface IEmailProvider
    {
        /// <summary>
        /// Generates an email message to confirm user registration.
        /// </summary>
        /// <param name="email">The recipient's email address.</param>
        /// <param name="callbackURL">The URL the user should follow to confirm registration.</param>
        /// <returns>A structured <see cref="EmailMessageDTO"/> representing the confirmation email.</returns>
        EmailMessageDTO GenerateConfirmRegistrationEmail(string email, string callbackURL);
    }
}

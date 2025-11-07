using MediatR;
using YuGiOh.Domain.DTOs;
using YuGiOh.Domain.Services;

namespace YuGiOh.Application.Features.Auth.Commands
{
    /// <summary>
    /// Represents a command request to send a confirmation email to a newly registered user.
    /// </summary>
    /// <remarks>
    /// This command encapsulates the necessary information to generate and send a 
    /// confirmation email, such as the recipient's email address and the callback URL
    /// used to confirm their registration.
    /// </remarks>
    public class SendConfirmationEmailCommand : IRequest
    {
        /// <summary>
        /// Gets or sets the recipient's email address where the confirmation message will be sent.
        /// </summary>
        public required string Email { get; set; }

        /// <summary>
        /// Gets or sets the callback URL included in the confirmation email, allowing the user 
        /// to complete their registration.
        /// </summary>
        public required string CallbackURL { get; set; }
    }

    /// <summary>
    /// Handles the <see cref="SendConfirmationEmailCommand"/> by generating and sending
    /// a confirmation email using the configured email provider and sender services.
    /// </summary>
    /// <remarks>
    /// This handler delegates the email content generation to an <see cref="IEmailProvider"/>
    /// and the sending process to an <see cref="IEmailSender"/> implementation.
    /// </remarks>
    public class SendConfirmationEmailCommandHandler : IRequestHandler<SendConfirmationEmailCommand>
    {
        private readonly IEmailSender _emailSender;
        private readonly IEmailProvider _emailProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="SendConfirmationEmailCommandHandler"/> class.
        /// </summary>
        /// <param name="emailSender">Service responsible for sending emails.</param>
        /// <param name="emailProvider">Service responsible for generating email templates and content.</param>
        /// <exception cref="ArgumentNullException">Thrown when any dependency is null.</exception>
        public SendConfirmationEmailCommandHandler(
            IEmailSender emailSender,
            IEmailProvider emailProvider)
        {
            _emailSender = emailSender ?? throw new ArgumentNullException(nameof(emailSender));
            _emailProvider = emailProvider ?? throw new ArgumentNullException(nameof(emailProvider));
        }

        /// <summary>
        /// Handles the process of composing and sending a confirmation email.
        /// </summary>
        /// <param name="request">The command containing the recipient’s email and callback URL.</param>
        /// <param name="cancellationToken">A token to cancel the operation, if needed.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is null.</exception>
        public async Task Handle(SendConfirmationEmailCommand request, CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            // Generate the confirmation email using the provider
            EmailMessageDTO emailData = _emailProvider.GenerateConfirmRegistrationEmail(request.Email, request.CallbackURL);

            // Send the composed email using the configured sender
            await _emailSender.SendMailAsync(emailData);
        }
    }
}

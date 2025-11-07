using MediatR;
using YuGiOh.Domain.Services;

namespace YuGiOh.Application.Features.Auth.Queries
{
    /// <summary>
    /// Represents a query to confirm a user's email address using a verification token.
    /// </summary>
    /// <remarks>
    /// This query is part of the authentication flow and is typically executed
    /// when a user clicks on a confirmation link sent to their email after registration.
    /// </remarks>
    public class ConfirmEmailQuery : IRequest<bool>
    {
        /// <summary>
        /// Gets or sets the email address of the user whose registration is being confirmed.
        /// </summary>
        public required string Email { get; set; }

        /// <summary>
        /// Gets or sets the confirmation token associated with the user's registration.
        /// </summary>
        public required string Token { get; set; }
    }

    /// <summary>
    /// Handles the execution of the <see cref="ConfirmEmailQuery"/> by invoking
    /// the registration service responsible for validating and confirming the user's email.
    /// </summary>
    /// <remarks>
    /// This handler communicates with the <see cref="IRegisterHandler"/> service in the domain layer
    /// to perform token validation and complete the account activation process.
    /// </remarks>
    public class ConfirmEmailQueryHandler : IRequestHandler<ConfirmEmailQuery, bool>
    {
        private readonly IRegisterHandler _registerHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfirmEmailQueryHandler"/> class.
        /// </summary>
        /// <param name="registerHandler">The service responsible for handling user registration logic.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="registerHandler"/> is null.</exception>
        public ConfirmEmailQueryHandler(IRegisterHandler registerHandler)
        {
            _registerHandler = registerHandler
                ?? throw new ArgumentNullException(nameof(registerHandler));
        }

        /// <summary>
        /// Handles the email confirmation process by validating the provided token for the specified email.
        /// </summary>
        /// <param name="request">The query containing the user's email and confirmation token.</param>
        /// <param name="cancellationToken">A token used to cancel the operation if necessary.</param>
        /// <returns>
        /// <c>true</c> if the confirmation was successful; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="request"/> is null.</exception>
        public async Task<bool> Handle(ConfirmEmailQuery request, CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            // Delegate confirmation logic to the domain registration handler
            return await _registerHandler.ConfirmRegistrationAsync(request.Email, request.Token);
        }
    }
}

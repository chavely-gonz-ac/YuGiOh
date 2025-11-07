using MediatR;
using YuGiOh.Domain.DTOs;
using YuGiOh.Domain.Services;

namespace YuGiOh.Application.Features.Auth.Commands
{
    /// <summary>
    /// Represents a command request to register a new user.
    /// </summary>
    /// <remarks>
    /// This class follows the CQRS pattern using MediatR.
    /// It encapsulates the data required to register a new account and returns a confirmation token string.
    /// </remarks>
    public class RegisterCommand : IRequest<string>
    {
        /// <summary>
        /// Gets or sets the registration data that contains all user information
        /// (e.g., email, password, address, and roles) required for creating the account.
        /// </summary>
        public required RegisterRequestData Data { get; set; }
    }

    /// <summary>
    /// Handles the <see cref="RegisterCommand"/> execution logic.
    /// </summary>
    /// <remarks>
    /// This handler delegates the actual registration process to an injected <see cref="IRegisterHandler"/> service.
    /// It acts as a mediator entry point from the Application layer to the Domain/Infrastructure layer.
    /// </remarks>
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, string>
    {
        private readonly IRegisterHandler _registerHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterCommandHandler"/> class.
        /// </summary>
        /// <param name="registerHandler">The service responsible for handling user registration logic.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="registerHandler"/> is null.</exception>
        public RegisterCommandHandler(IRegisterHandler registerHandler)
        {
            _registerHandler = registerHandler
                ?? throw new ArgumentNullException(nameof(registerHandler));
        }

        /// <summary>
        /// Handles the registration command by invoking the domain registration logic.
        /// </summary>
        /// <param name="request">The registration command containing the user’s registration data.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A <see cref="string"/> representing the confirmation token sent to the user’s email.
        /// </returns>
        public async Task<string> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            // Extract the user registration data from the command
            var registerUser = request.Data;

            // Delegate the registration process to the domain service.
            // The result is typically an email confirmation token.
            return await _registerHandler.RegisterUserAsync(registerUser);
        }
    }
}

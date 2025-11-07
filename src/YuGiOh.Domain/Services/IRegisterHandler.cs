using YuGiOh.Domain.DTOs;

namespace YuGiOh.Domain.Services
{
    /// <summary>
    /// Defines the contract for handling user registration and confirmation processes.
    /// </summary>
    public interface IRegisterHandler
    {
        /// <summary>
        /// Registers a new user based on the provided registration data.
        /// </summary>
        /// <param name="data">The registration data containing user and address details.</param>
        /// <returns>
        /// A string representing a result of the registration, typically a confirmation token
        /// or unique identifier of the newly created account.
        /// </returns>
        Task<string> RegisterUserAsync(RegisterRequestData data);

        /// <summary>
        /// Confirms a user registration using the provided email and confirmation token.
        /// </summary>
        /// <param name="email">The email address of the account to confirm.</param>
        /// <param name="token">The token used to verify the email ownership.</param>
        /// <returns><c>true</c> if the registration was successfully confirmed; otherwise, <c>false</c>.</returns>
        Task<bool> ConfirmRegistrationAsync(string email, string token);
    }
}

using System.Text.RegularExpressions;
using YuGiOh.Domain.Models;

namespace YuGiOh.Domain.DTOs
{
    /// <summary>
    /// Represents the data required to register a new user in the system.
    /// </summary>
    public class RegisterRequestData
    {
        /// <summary>
        /// Gets or sets the user's first name.  
        /// Must contain only Latin letters (validated in the application layer).
        /// </summary>
        public required string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the user's middle name (optional).  
        /// Must contain only Latin letters if provided.
        /// </summary>
        public string? MiddleName { get; set; }

        /// <summary>
        /// Gets or sets the user's first surname.  
        /// Must contain only Latin letters.
        /// </summary>
        public required string FirstSurname { get; set; }

        /// <summary>
        /// Gets or sets the user's second surname.  
        /// Must contain only Latin letters.
        /// </summary>
        public required string SecondSurname { get; set; }

        /// <summary>
        /// Gets the user's full name in a normalized form, 
        /// where each name component is trimmed and separated by '0'.  
        /// Example: <c>John0Michael0Doe0Smith</c>
        /// </summary>
        public string FullName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(MiddleName))
                    return $"{FirstName.Trim()}0{FirstSurname.Trim()}0{SecondSurname.Trim()}";
                else
                    return $"{FirstName.Trim()}0{MiddleName.Trim()}0{FirstSurname.Trim()}0{SecondSurname.Trim()}";
            }
        }

        /// <summary>
        /// Gets or sets the user's email address.
        /// </summary>
        public required string Email { get; set; }

        /// <summary>
        /// Gets or sets the user's password.  
        /// Complexity and format are validated in the application layer.
        /// </summary>
        public required string Password { get; set; }

        /// <summary>
        /// Gets or sets the list of roles assigned to the user during registration.
        /// </summary>
        public IEnumerable<string> Roles { get; set; } = Enumerable.Empty<string>();

        /// <summary>
        /// Gets or sets the user's address information (optional).
        /// </summary>
        public Address? Address { get; set; }

        /// <summary>
        /// Gets or sets the user's International Bank Account Number (optional).
        /// </summary>
        public string? IBAN { get; set; }

        // ---------------------------------------------------------------------

        /// <summary>
        /// Attempts to parse a normalized full name string (using '0' as a separator)
        /// into its name components.
        /// </summary>
        /// <param name="fullName">The normalized full name string from storage.</param>
        /// <param name="names">The resulting name tuple, if parsing succeeds.</param>
        /// <returns><c>true</c> if the format is valid and parsing succeeded; otherwise <c>false</c>.</returns>
        public static bool TryParseFullName(
            string fullName,
            out (string FirstName, string? MiddleName, string FirstSurname, string SecondSurname) names)
        {
            names = default;

            if (string.IsNullOrWhiteSpace(fullName))
                return false;

            // Split using '0' as delimiter
            var parts = fullName.Split('0', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            // Valid combinations: 3 parts (no middle name) or 4 parts (with middle name)
            if (parts.Length == 3)
            {
                names = (parts[0], null, parts[1], parts[2]);
                return true;
            }
            else if (parts.Length == 4)
            {
                names = (parts[0], parts[1], parts[2], parts[3]);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks whether a full name string follows the correct normalized format:
        /// 3 or 4 name parts separated by '0', each containing only Latin letters.
        /// </summary>
        public static bool IsValidFullNameFormat(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                return false;

            var parts = fullName.Split('0', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length is < 3 or > 4) return false;

            return parts.All(p => Regex.IsMatch(p, @"^[A-Za-z]+$"));
        }
    }
}

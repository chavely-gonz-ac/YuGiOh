using YuGiOh.Domain.Exceptions;

namespace YuGiOh.Infrastructure.EmailService
{
    /// <summary>
    /// Represents configuration options for SMTP (Simple Mail Transfer Protocol)
    /// used by the email sending service.
    /// </summary>
    /// <remarks>
    /// This configuration class defines connection parameters and sender information
    /// for outgoing emails. It should be bound from the application's configuration (e.g. appsettings.json)
    /// under a section such as <c>"SMTPOptions"</c>.
    /// </remarks>
    public class SMTPOptions
    {
        private string _server = string.Empty;
        private int _port;
        private string _username = string.Empty;
        private string _password = string.Empty;
        private string _fromAddress = string.Empty;
        private string _fromDisplayName = string.Empty;

        /// <summary>
        /// Gets or sets the SMTP server host name or IP address.
        /// </summary>
        /// <exception cref="APIException">Thrown when the value is null or whitespace.</exception>
        public required string Server
        {
            get => _server;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw APIException.BadRequest("SMTP server address cannot be null or empty.");
                _server = value;
            }
        }

        /// <summary>
        /// Gets or sets the SMTP server port number (e.g., 25, 465, 587).
        /// </summary>
        /// <exception cref="APIException">Thrown when the port is zero or negative.</exception>
        public required int Port
        {
            get => _port;
            set
            {
                if (value <= 0)
                    throw APIException.BadRequest("SMTP port must be greater than zero.");
                _port = value;
            }
        }

        /// <summary>
        /// Gets or sets the username used to authenticate with the SMTP server.
        /// </summary>
        /// <exception cref="APIException">Thrown when the value is null or whitespace.</exception>
        public required string Username
        {
            get => _username;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw APIException.BadRequest("SMTP username cannot be null or empty.");
                _username = value;
            }
        }

        /// <summary>
        /// Gets or sets the password used to authenticate with the SMTP server.
        /// </summary>
        /// <exception cref="APIException">Thrown when the value is null or whitespace.</exception>
        public required string Password
        {
            get => _password;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw APIException.BadRequest("SMTP password cannot be null or empty.");
                _password = value;
            }
        }

        /// <summary>
        /// Gets or sets the default email address to use as the sender for outgoing emails.
        /// </summary>
        /// <exception cref="APIException">Thrown when the value is null or whitespace.</exception>
        public required string FromAddress
        {
            get => _fromAddress;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw APIException.BadRequest("SMTP sender address cannot be null or empty.");
                _fromAddress = value;
            }
        }

        /// <summary>
        /// Gets or sets the friendly display name to appear as the sender of outgoing emails.
        /// </summary>
        /// <exception cref="APIException">Thrown when the value is null or whitespace.</exception>
        public required string FromDisplayName
        {
            get => _fromDisplayName;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw APIException.BadRequest("SMTP sender display name cannot be null or empty.");
                _fromDisplayName = value;
            }
        }
    }
}

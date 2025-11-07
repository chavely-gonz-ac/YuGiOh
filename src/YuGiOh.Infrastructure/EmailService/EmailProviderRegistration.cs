using YuGiOh.Domain.DTOs;
using YuGiOh.Domain.Services;
using YuGiOh.Domain.Exceptions;

namespace YuGiOh.Infrastructure.EmailService
{
    /// <summary>
    /// Provides functionality for generating email messages related to user account operations,
    /// such as registration confirmation.
    /// </summary>
    public partial class EmailProvider : IEmailProvider
    {
        /// <summary>
        /// Generates a formatted email message for confirming user registration.
        /// </summary>
        /// <param name="email">The recipient's email address.</param>
        /// <param name="callbackURL">The confirmation link to verify the user’s account.</param>
        /// <returns>
        /// A populated <see cref="EmailMessageDTO"/> containing both HTML and plain text representations
        /// of the registration confirmation email.
        /// </returns>
        /// <exception cref="APIException">
        /// Thrown when the <paramref name="email"/> or <paramref name="callbackURL"/> is null or invalid.
        /// </exception>
        public EmailMessageDTO GenerateConfirmRegistrationEmail(string email, string callbackURL)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw APIException.BadRequest("Recipient email address cannot be null or empty.", nameof(email));

            if (string.IsNullOrWhiteSpace(callbackURL))
                throw APIException.BadRequest("Callback URL cannot be null or empty.", nameof(callbackURL));

            // Construct the confirmation email with both HTML and plaintext versions
            return new EmailMessageDTO
            {
                ToAddress = email,
                Subject = "Confirm Your Registration - Yu-Gi-Oh!",
                Body = $@"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <style>
        body {{
            font-family: Arial, sans-serif;
            background-color: #f7f5fa;
            color: #333;
            margin: 0;
            padding: 0;
        }}
        .container {{
            max-width: 600px;
            margin: 20px auto;
            background: white;
            border-radius: 12px;
            overflow: hidden;
            box-shadow: 0 4px 10px rgba(0,0,0,0.1);
        }}
        .header {{
            background: linear-gradient(135deg, #6a0dad, #9b30ff);
            color: white;
            text-align: center;
            padding: 20px;
        }}
        .header h1 {{
            margin: 0;
            font-size: 24px;
        }}
        .content {{
            padding: 20px;
            font-size: 16px;
            line-height: 1.6;
        }}
        .btn {{
            display: inline-block;
            margin: 20px 0;
            padding: 12px 20px;
            background: #6a0dad;
            color: white !important;
            text-decoration: none;
            border-radius: 8px;
            font-weight: bold;
        }}
        .btn:hover {{
            background: #8a2be2;
        }}
        .footer {{
            text-align: center;
            font-size: 12px;
            color: #777;
            padding: 15px;
            border-top: 1px solid #eee;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>Welcome to Yu-Gi-Oh!</h1>
        </div>
        <div class='content'>
            <p>Hi Duelist,</p>
            <p>Thank you for registering with <strong>Yu-Gi-Oh! Tournament System</strong>.</p>
            <p>To complete your registration, please confirm your email address by clicking the button below:</p>
            <p style='text-align:center;'>
                <a href='{callbackURL}' class='btn'>Confirm My Email</a>
            </p>
            <p>If you didn’t request this, you can safely ignore this email.</p>
        </div>
        <div class='footer'>
            © {DateTime.UtcNow.Year} Yu-Gi-Oh! Tournament System
        </div>
    </div>
</body>
</html>",
                IsHTML = true,
                PlainTextBody = $@"
Welcome to Yu-Gi-Oh! Tournament System

Thank you for registering.
To complete your registration, confirm your email by visiting this link:

{callbackURL}

If you did not request this, please ignore this message.
"
            };
        }
    }
}

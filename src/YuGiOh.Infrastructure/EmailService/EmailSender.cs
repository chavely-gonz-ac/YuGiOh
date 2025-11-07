using YuGiOh.Domain.DTOs;
using YuGiOh.Domain.Services;
using YuGiOh.Domain.Exceptions;

using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;

namespace YuGiOh.Infrastructure.EmailService
{
    /// <summary>
    /// Provides functionality to send emails using SMTP (Simple Mail Transfer Protocol)
    /// through the MailKit library.
    /// </summary>
    public class EmailSender : IEmailSender
    {
        private readonly SMTPOptions _smtpOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailSender"/> class.
        /// </summary>
        /// <param name="options">The configured SMTP options for connecting to the mail server.</param>
        /// <exception cref="APIException">Thrown when SMTP configuration is null or invalid.</exception>
        public EmailSender(IOptions<SMTPOptions> options)
        {
            if (options?.Value == null)
                throw APIException.Internal("SMTP configuration options cannot be null.");

            _smtpOptions = options.Value;

            // Basic validation
            if (string.IsNullOrWhiteSpace(_smtpOptions.Server))
                throw APIException.BadRequest("SMTP server cannot be null or empty.");

            if (_smtpOptions.Port <= 0)
                throw APIException.BadRequest("SMTP port must be greater than zero.");

            if (string.IsNullOrWhiteSpace(_smtpOptions.Username))
                throw APIException.BadRequest("SMTP username cannot be null or empty.");

            if (string.IsNullOrWhiteSpace(_smtpOptions.Password))
                throw APIException.BadRequest("SMTP password cannot be null or empty.");

            if (string.IsNullOrWhiteSpace(_smtpOptions.FromDisplayName))
                throw APIException.BadRequest("SMTP display name cannot be null or empty.");

            if (string.IsNullOrWhiteSpace(_smtpOptions.FromAddress))
                throw APIException.BadRequest("SMTP sender address cannot be null or empty.");
        }

        /// <summary>
        /// Sends an individual email asynchronously using the configured SMTP server.
        /// </summary>
        /// <param name="emailData">The email message content to be sent.</param>
        /// <exception cref="APIException">
        /// Thrown when the email data is invalid or when sending fails due to SMTP errors.
        /// </exception>
        public async Task SendMailAsync(EmailMessageDTO emailData)
        {
            if (emailData == null)
                throw APIException.BadRequest("Email data cannot be null.");

            if (string.IsNullOrWhiteSpace(emailData.ToAddress))
                throw APIException.BadRequest("Recipient address cannot be null or empty.");

            if (string.IsNullOrWhiteSpace(emailData.Subject))
                throw APIException.BadRequest("Email subject cannot be null or empty.");

            var email = BuildMimeMessage(emailData);

            try
            {
                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(_smtpOptions.Server, _smtpOptions.Port, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_smtpOptions.Username, _smtpOptions.Password);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
            }
            catch (SmtpCommandException ex)
            {
                throw APIException.Internal(
                    $"SMTP command error while sending email: {ex.Message}",
                    detail: $"StatusCode: {ex.StatusCode}",
                    innerException: ex);
            }
            catch (SmtpProtocolException ex)
            {
                throw APIException.Internal("SMTP protocol error while sending email.", ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw APIException.Internal("Unexpected error occurred while sending email.", ex.Message, ex);
            }
        }

        /// <summary>
        /// Sends multiple emails efficiently by reusing a single SMTP connection.
        /// </summary>
        /// <param name="emailBatch">A collection of email messages to send.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="APIException">
        /// Thrown when the input collection is invalid or SMTP transmission fails.
        /// </exception>
        public async Task SendBulkMailAsync(IEnumerable<EmailMessageDTO> emailBatch)
        {
            if (emailBatch == null)
                throw APIException.BadRequest("Email batch cannot be null.");

            var emails = emailBatch.ToList();
            if (emails.Count == 0)
                throw APIException.BadRequest("Email batch cannot be empty.");

            using var smtp = new SmtpClient();
            try
            {
                await smtp.ConnectAsync(_smtpOptions.Server, _smtpOptions.Port, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_smtpOptions.Username, _smtpOptions.Password);

                foreach (var emailData in emails)
                {
                    try
                    {
                        if (emailData == null || string.IsNullOrWhiteSpace(emailData.ToAddress))
                            continue; // Skip invalid entries instead of stopping the batch

                        var email = BuildMimeMessage(emailData);
                        await smtp.SendAsync(email);
                    }
                    catch (SmtpCommandException ex)
                    {
                        // Continue sending remaining emails even if one fails
                        throw APIException.Internal(
                            $"SMTP command error while sending email to {emailData?.ToAddress}.",
                            detail: ex.Message,
                            innerException: ex);
                    }
                    catch (Exception ex)
                    {
                        // Log or handle as needed — we don’t stop the batch
                        throw APIException.Internal(
                            $"Unexpected error while sending email to {emailData?.ToAddress}.",
                            ex.Message,
                            ex);
                    }
                }

                await smtp.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                throw APIException.Internal("Error during bulk email sending session.", ex.Message, ex);
            }
        }

        /// <summary>
        /// Builds a properly formatted <see cref="MimeMessage"/> from an <see cref="EmailMessageDTO"/>.
        /// </summary>
        private MimeMessage BuildMimeMessage(EmailMessageDTO emailData)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_smtpOptions.FromDisplayName, _smtpOptions.FromAddress));
            email.To.Add(MailboxAddress.Parse(emailData.ToAddress));
            email.Subject = emailData.Subject;

            if (emailData.IsHTML)
            {
                var builder = new BodyBuilder
                {
                    HtmlBody = emailData.Body,
                    TextBody = emailData.PlainTextBody ?? string.Empty
                };
                email.Body = builder.ToMessageBody();
            }
            else
            {
                email.Body = new TextPart("plain")
                {
                    Text = emailData.Body ?? string.Empty
                };
            }

            return email;
        }
    }
}

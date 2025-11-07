using System.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using YuGiOh.Domain.DTOs;
using YuGiOh.Domain.Exceptions;
using YuGiOh.Domain.Services;
using YuGiOh.Infrastructure.Persistence;

namespace YuGiOh.Infrastructure.Identity.Services
{
    /// <summary>
    /// Handles the registration workflow for new user accounts, including
    /// creation, role assignment, and email confirmation token generation.
    /// </summary>
    public class RegisterHandler : IRegisterHandler
    {
        private readonly YuGiOhDbContext _dbContext;
        private readonly UserManager<Account> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<RegisterHandler> _logger;
        private readonly RolesSpecificEntitiesManager _rolesSpecificEntitiesManager;

        public RegisterHandler(
            YuGiOhDbContext dbContext,
            UserManager<Account> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration,
            ILogger<RegisterHandler> logger,
            RolesSpecificEntitiesManager rolesSpecificEntitiesManager)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _rolesSpecificEntitiesManager = rolesSpecificEntitiesManager ?? throw new ArgumentNullException(nameof(rolesSpecificEntitiesManager));
        }

        /// <inheritdoc/>
        public async Task<string> RegisterUserAsync(RegisterRequestData data)
        {
            if (data == null)
                throw APIException.BadRequest("Argument Null Exception", nameof(data));
            var strategy = _dbContext.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _dbContext.Database.BeginTransactionAsync();
                try
                {
                    ValidateRoleCombination(data);
                    var account = await CreateAccountAsync(data);
                    await AddRolesAsync(account, data.Roles);

                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(account);

                    await _rolesSpecificEntitiesManager.AddRelatedEntities(data, account.Id);

                    await transaction.CommitAsync();
                    return token;
                }
                catch (APIException)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
                catch (Exception exception)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(exception, "User registration failed for email {Email}", data.Email);
                    throw new APIException(innerException: exception);
                }
            });
        }

        private static void ValidateRoleCombination(RegisterRequestData data)
        {
            // // === Domain Role Conflict Validation ===
            // bool hasStaff = data.Roles.Contains("Staff", StringComparer.OrdinalIgnoreCase);
            // bool hasSponsor = data.Roles.Contains("Sponsor", StringComparer.OrdinalIgnoreCase);
            // bool hasAdmin = data.Roles.Contains("Admin", StringComparer.OrdinalIgnoreCase);

            // // Rule 1: cannot have Staff and Sponsor together
            // if (hasStaff && hasSponsor)
            //     throw APIException.BadRequest(
            //         "Invalid Role Combination",
            //         "An account cannot have both Staff and Sponsor roles at the same time.");

            // // Rule 2: Admin with valid IBAN cannot have Staff or Sponsor
            // // Assuming Account has a property 'IBAN' that may be null or empty.
            // if (hasAdmin && !string.IsNullOrWhiteSpace(data.IBAN) && (hasStaff || hasSponsor))
            //     throw APIException.BadRequest(
            //         "Invalid Role Combination",
            //         "An Admin account with a valid IBAN cannot have Staff or Sponsor roles.");
        }


        /// <summary>
        /// Creates the base identity account and persists it to the store.
        /// </summary>
        private async Task<Account> CreateAccountAsync(RegisterRequestData request)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
                throw APIException.BadRequest("Argument Exception", "Email is required.");
            if (string.IsNullOrWhiteSpace(request.Password))
                throw APIException.BadRequest("Argument Exception", "Password is required.");

            var existing = await _userManager.FindByEmailAsync(request.Email);
            if (existing != null)
                throw APIException.BadRequest("Invalid Operation Exception", $"An account with email {request.Email} already exists.");

            var account = new Account
            {
                UserName = request.FullName,
                Email = request.Email
            };

            var createResult = await _userManager.CreateAsync(account, request.Password);
            if (!createResult.Succeeded)
            {
                var errors = string.Join("; ", createResult.Errors.Select(e => e.Description));
                _logger.LogWarning("Failed to create account {Email}: {Errors}", request.Email, errors);
                throw APIException.BadRequest("Invalid Operation Exception", $"Could not create account: {errors}");
            }

            return account;
        }
        /// <summary>
        /// Assigns the specified roles to the given account,
        /// ensuring domain rules between Staff, Sponsor, and Admin roles are respected.
        /// </summary>
        private async Task AddRolesAsync(Account account, IEnumerable<string> roles)
        {
            if (roles == null)
                throw APIException.BadRequest("Argument Exception", "Roles are required.");

            var toAdd = new List<string>();

            foreach (var roleName in roles.Distinct(StringComparer.OrdinalIgnoreCase))
            {
                if (!await _roleManager.RoleExistsAsync(roleName))
                    throw APIException.BadRequest("Invalid Operation Exception", $"Role does not exist: {roleName}.");

                toAdd.Add(roleName);
            }

            if (!toAdd.Any())
                throw APIException.BadRequest("Argument Exception", "At least one role is required.");

            // === Proceed with role assignment ===
            var addResult = await _userManager.AddToRolesAsync(account, toAdd);
            if (!addResult.Succeeded)
            {
                var errors = string.Join("; ", addResult.Errors.Select(e => e.Description));
                _logger.LogWarning("Failed to assign roles to user {Email}: {Errors}", account.Email, errors);
                throw APIException.BadRequest("Invalid Operation Exception", $"Not all roles could be assigned: {errors}");
            }
        }


        /// <inheritdoc/>
        public async Task<bool> ConfirmRegistrationAsync(string email, string token)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw APIException.BadRequest("Argument Exception", "Email is required.");
            if (string.IsNullOrWhiteSpace(token))
                throw APIException.BadRequest("Argument Exception", "Token is required.");

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                _logger.LogWarning("ConfirmEmail: email not found {Email}", email);
                return false;
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                _logger.LogWarning("ConfirmEmail failed for {Email}: {Errors}", email, string.Join("; ", result.Errors.Select(e => e.Description)));
                return false;
            }

            return true;
        }
    }
}

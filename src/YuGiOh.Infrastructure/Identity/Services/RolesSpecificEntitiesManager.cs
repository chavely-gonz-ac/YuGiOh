using Ardalis.Specification;
using Microsoft.Extensions.Logging;
using YuGiOh.Domain.DTOs;
using YuGiOh.Domain.Exceptions;
using YuGiOh.Domain.Models;

namespace YuGiOh.Infrastructure.Identity.Services
{
    /// <summary>
    /// Handles creation of domain-specific related entities for an account
    /// based on the roles assigned during registration.
    /// </summary>
    public class RolesSpecificEntitiesManager
    {
        private readonly IRepositoryBase<Address> _addressRepository;
        private readonly IRepositoryBase<HasIBAN> _ibanRepository;
        private readonly IRepositoryBase<Player> _playerRepository;
        private readonly ILogger<RolesSpecificEntitiesManager> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="RolesSpecificEntitiesManager"/> class.
        /// </summary>
        /// <param name="addressRepository">Repository for managing <see cref="Address"/> entities.</param>
        /// <param name="ibanRepository">Repository for managing <see cref="HasIBAN"/> entities.</param>
        /// <param name="playerRepository">Repository for managing <see cref="Player"/> entities.</param>
        /// <param name="logger">Logger instance for diagnostic information.</param>
        /// <exception cref="APIException">Thrown if any dependency is null.</exception>
        public RolesSpecificEntitiesManager(
            IRepositoryBase<Address> addressRepository,
            IRepositoryBase<HasIBAN> ibanRepository,
            IRepositoryBase<Player> playerRepository,
            ILogger<RolesSpecificEntitiesManager> logger)
        {
            _addressRepository = addressRepository ?? throw APIException.BadRequest("Address repository cannot be null.", nameof(addressRepository));
            _ibanRepository = ibanRepository ?? throw APIException.BadRequest("IBAN repository cannot be null.", nameof(ibanRepository));
            _playerRepository = playerRepository ?? throw APIException.BadRequest("Player repository cannot be null.", nameof(playerRepository));
            _logger = logger ?? throw APIException.BadRequest("Logger cannot be null.", nameof(logger));
        }

        /// <summary>
        /// Adds role-dependent related entities (Address, HasIBAN, Player) after user registration.
        /// </summary>
        /// <param name="request">The registration request data containing address, IBAN, and roles.</param>
        /// <param name="accountId">The ID of the account that was created.</param>
        /// <exception cref="APIException">Thrown when <paramref name="request"/> or <paramref name="accountId"/> is invalid.</exception>
        public async Task AddRelatedEntities(RegisterRequestData request, string accountId)
        {
            if (request == null)
                throw APIException.BadRequest("Registration request cannot be null.", nameof(request));

            if (string.IsNullOrWhiteSpace(accountId))
                throw APIException.BadRequest("Account ID is required.", nameof(accountId));

            var roles = request.Roles?.Distinct(StringComparer.OrdinalIgnoreCase).ToList() ?? new List<string>();
            if (!roles.Any())
            {
                _logger.LogInformation("No roles provided for account {AccountId}; skipping entity creation.", accountId);
                return;
            }

            var entitiesCreated = new List<string>();

            // === Rule 1: If role includes Player → create Address and Player entities ===
            if (roles.Contains("Player", StringComparer.OrdinalIgnoreCase))
            {
                try
                {
                    // Create Address entity
                    if (request.Address == null)
                        throw APIException.BadRequest("Address information is required for player registration.");

                    var address = new Address
                    {
                        CountryIso2 = request.Address.CountryIso2,
                        StateIso2 = request.Address.StateIso2,
                        City = request.Address.City,
                        StreetTypeId = request.Address.StreetTypeId,
                        StreetName = request.Address.StreetName,
                        Building = request.Address.Building,
                        Apartment = request.Address.Apartment
                    };

                    await _addressRepository.AddAsync(address);
                    entitiesCreated.Add(nameof(Address));

                    // Create Player entity linked to the account and address
                    var player = new Player
                    {
                        Id = accountId,
                        AddressId = address.Id
                    };

                    await _playerRepository.AddAsync(player);
                    entitiesCreated.Add(nameof(Player));

                    _logger.LogInformation("Player entity created for account {AccountId} (AddressId: {AddressId}).", accountId, address.Id);
                }
                catch (APIException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw APIException.Internal(
                        "Failed to create Player-related entities during registration.",
                        ex.Message,
                        ex);
                }
            }

            // === Rule 2: If Staff, Sponsor, or Admin (with valid IBAN) → create HasIBAN entity ===
            bool requiresIban =
                roles.Contains("Staff", StringComparer.OrdinalIgnoreCase) ||
                roles.Contains("Sponsor", StringComparer.OrdinalIgnoreCase) ||
                (roles.Contains("Admin", StringComparer.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(request.IBAN));

            if (requiresIban)
            {
                if (string.IsNullOrWhiteSpace(request.IBAN))
                {
                    _logger.LogWarning(
                        "Account {AccountId} has a role requiring IBAN, but IBAN was not provided.",
                        accountId);
                }
                else
                {
                    var ibanEntity = new HasIBAN
                    {
                        Id = accountId,
                        IBAN = request.IBAN
                    };

                    await _ibanRepository.AddAsync(ibanEntity);
                    entitiesCreated.Add(nameof(HasIBAN));
                }
            }

            // === Logging Summary ===
            if (entitiesCreated.Any())
                _logger.LogInformation(
                    "Created related entities [{Entities}] for account {AccountId}.",
                    string.Join(", ", entitiesCreated), accountId);
            else
                _logger.LogInformation("No related entities created for account {AccountId}.", accountId);
        }
    }
}

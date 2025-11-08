using Ardalis.Specification;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using YuGiOh.Infrastructure.Seeding.Helpers;
using YuGiOh.Infrastructure.Identity;
using YuGiOh.Domain.Models;

namespace YuGiOh.Infrastructure.Seeding.Tools
{
    public partial class SeedingToolsProvider
    {
        public readonly SeedTools<StreetType> StreetTypeSeedingTools;
        public readonly SeedTools<IdentityRole> RoleSeedingTools;
        public readonly SeedTools<Account> AccountSeedingTools;
        public readonly SeedTools<Archetype> ArchetypeSeedingTools;

        public readonly List<object> SeedingTools;

        private readonly ILoggerFactory _loggerFactory;

        public SeedingToolsProvider(
            RoleManager<IdentityRole> roleManager,
            UserManager<Account> userManager,
            IRepositoryBase<StreetType> streetTypeRepository,
            IRepositoryBase<Archetype> archetypeRepository,
            ILoggerFactory loggerFactory
        )
        {
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            StreetTypeSeedingTools = GenerateStreetTypesSeedingTools(streetTypeRepository);
            RoleSeedingTools = GenerateRoleSeedingTools(roleManager);
            AccountSeedingTools = GenerateAccountSeedingTools(userManager);
            ArchetypeSeedingTools = GenerateArchetypeSeedingTools(archetypeRepository);
            SeedingTools = new List<object>()
            {
                StreetTypeSeedingTools,
                RoleSeedingTools,
                AccountSeedingTools,
                ArchetypeSeedingTools
            };
        }

        private SeedTools<Archetype>? GenerateArchetypeSeedingTools(IRepositoryBase<Archetype> archetypeRepository)
        {
            return new SeedTools<Archetype>(
                ArchetypesProvider.FetchArchetypesFromWebAsync,
                async type => await archetypeRepository.AnyAsync(new ArchetypeByNameSpec(type.Name)),
                async types => { foreach (var type in types) await archetypeRepository.AddAsync(type); },
                _loggerFactory.CreateLogger<SeedTools<Archetype>>()
                );
        }

        protected SeedTools<IdentityRole> GenerateRoleSeedingTools(RoleManager<IdentityRole> roleManager)
        {
            return new SeedTools<IdentityRole>(
                async () => (await JsonReaderWrapper.ResolveDataFromJson<string>("IdentityRole")).Select(role => new IdentityRole(role)),
                role => roleManager.RoleExistsAsync(role.Name),
                async roles => { foreach (var role in roles) await roleManager.CreateAsync(role); },
                _loggerFactory.CreateLogger<SeedTools<IdentityRole>>()
            );
        }
        protected SeedTools<StreetType> GenerateStreetTypesSeedingTools(IRepositoryBase<StreetType> staffRepository)
        {
            return new SeedTools<StreetType>(
                async () => await JsonReaderWrapper.ResolveDataFromJson<StreetType>(),
                async type => !(await staffRepository.GetByIdAsync(type.Id) == null),
                async types => { foreach (var type in types) await staffRepository.AddAsync(type); },
                _loggerFactory.CreateLogger<SeedTools<StreetType>>()
                );
        }
        protected SeedTools<Account> GenerateAccountSeedingTools(UserManager<Account> userManager)
        {
            return new SeedTools<Account>(
                async () => (await JsonReaderWrapper.ResolveDataFromJson<AccountDTO>()).Select(accountDTO => MapFromDTO(accountDTO, userManager)),
                async account => !(await userManager.FindByNameAsync(account.UserName) == null),
                async accounts =>
                {
                    foreach (var account in accounts)
                    {
                        string password = account.PasswordHash;
                        account.PasswordHash = null;
                        await userManager.CreateAsync(account, password);
                        await userManager.AddToRoleAsync(account, "Admin");
                    }
                },
                _loggerFactory.CreateLogger<SeedTools<Account>>()
            );
        }
        private static Account MapFromDTO(AccountDTO account, UserManager<Account> userManager)
        {
            var newAccount = new Account
            {
                UserName = account.UserName,
                PasswordHash = account.Password,
                Email = account.Email,
                EmailConfirmed = true
            };
            return newAccount;
        }
        public record AccountDTO
        {
            public string UserName { get; set; } = "";
            public string Password { get; set; } = "";
            public string Email { get; set; } = "";
        }

        public sealed class ArchetypeByNameSpec : Specification<Archetype>
        {
            public ArchetypeByNameSpec(string name)
            {
                // simple case-insensitive match by normalizing to lower
                var lowered = name?.Trim().ToLowerInvariant() ?? string.Empty;
                Query.Where(a => a.Name.ToLower() == lowered);
            }
        }
    }
}
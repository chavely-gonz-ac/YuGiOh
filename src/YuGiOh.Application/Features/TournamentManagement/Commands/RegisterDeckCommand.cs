using Ardalis.Specification;
using MediatR;
using YuGiOh.Domain.Exceptions;
using YuGiOh.Domain.Models;

namespace YuGiOh.Application.Features.TournamentManagement.Commands
{
    /// <summary>
    /// Command used to register (create) a new deck in the Yu-Gi-Oh! tournament system.
    /// </summary>
    /// <remarks>
    /// This command encapsulates the data required to add a new <see cref="Deck"/> to the database.
    /// It is handled by <see cref="RegisterDeckCommandHandler"/> which persists the deck
    /// using the injected repository.
    /// </remarks>
    public record RegisterDeckCommand : IRequest
    {
        /// <summary>
        /// Gets or sets the <see cref="Deck"/> entity to register.
        /// </summary>
        /// <example>
        /// {
        ///   "id": 0,
        ///   "name": "Dark Magician Deck",
        ///   "mainDeckSize": 40,
        ///   "sideDeckSize": 15,
        ///   "extraDeckSize": 15,
        ///   "ownerId": "user-123",
        ///   "archetypeId": 1,
        ///   "createdAt": "2025-11-08T00:00:00Z"
        /// }
        /// </example>
        public required Deck Deck { get; set; }
    }

    /// <summary>
    /// Handles the <see cref="RegisterDeckCommand"/> by persisting the new deck
    /// into the database using the repository pattern.
    /// </summary>
    /// <remarks>
    /// This handler uses the injected <see cref="IRepositoryBase{T}"/> for <see cref="Deck"/>
    /// to add a new deck asynchronously. Any unexpected exceptions are caught and wrapped
    /// into an <see cref="APIException"/> for standardized error responses.
    /// </remarks>
    public class RegisterDeckCommandHandler : IRequestHandler<RegisterDeckCommand>
    {
        private readonly IRepositoryBase<Deck> _deckRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterDeckCommandHandler"/> class.
        /// </summary>
        /// <param name="deckRepository">
        /// The repository responsible for data persistence operations related to <see cref="Deck"/>.
        /// </param>
        public RegisterDeckCommandHandler(IRepositoryBase<Deck> deckRepository)
        {
            _deckRepository = deckRepository;
        }

        /// <summary>
        /// Handles the <see cref="RegisterDeckCommand"/> request.
        /// </summary>
        /// <param name="request">The command containing the deck to be created.</param>
        /// <param name="cancellationToken">A token for cancelling the asynchronous operation.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        /// <exception cref="APIException">
        /// Thrown when an error occurs during the creation of the deck.
        /// </exception>
        public Task Handle(RegisterDeckCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Add the provided Deck entity to the database.
                // The repository will handle persistence and ID generation.
                return _deckRepository.AddAsync(request.Deck);
            }
            catch (Exception ex)
            {
                // Convert the raw exception into a standardized APIException
                // so Swagger and consumers get a clean, descriptive error.
                throw APIException.BadRequest("Error while creating the deck", innerException: ex);
            }
        }
    }
}

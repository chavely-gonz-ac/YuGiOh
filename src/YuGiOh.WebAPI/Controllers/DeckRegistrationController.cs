using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YuGiOh.Application.Features.TournamentManagement.Commands;
using YuGiOh.Application.Features.TournamentManagement.Queries;

namespace YuGiOh.WebAPI.Controllers
{
    /// <summary>
    /// Handles deck registration and archetype retrieval operations.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class DeckRegistrationController : APIControllerBase
    {
        public DeckRegistrationController(IMediator mediator) : base(mediator) { }

        /// <summary>
        /// Registers a new deck for a tournament.
        /// </summary>
        /// <remarks>
        /// 🔒 Requires the user to be authenticated and have the **Player** role.  
        /// 
        /// This endpoint accepts a <see cref="RegisterDeckCommand"/> and delegates handling
        /// to the application layer via MediatR.
        /// </remarks>
        /// <param name="request">The deck registration command payload.</param>
        /// <returns>An <see cref="IActionResult"/> indicating the registration result.</returns>
        /// <response code="200">Deck registered successfully.</response>
        /// <response code="400">Invalid deck data or validation failed.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403">User does not have the Player role.</response>
        /// <response code="500">Unexpected server error.</response>
        [HttpPost("register")]
        [Authorize(Roles = "Player")] // ✅ Restricts access to authenticated Players only
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Register([FromBody] RegisterDeckCommand request)
        {
            await Sender.Send(request);
            return Ok("Deck registered successfully.");
        }

        /// <summary>
        /// Retrieves all available archetypes (paginated).
        /// </summary>
        /// <remarks>
        /// Returns a paginated list of archetypes for deck creation.
        /// </remarks>
        /// <param name="page">The page number (default = 1).</param>
        /// <returns>A paginated list of archetypes.</returns>
        /// <response code="200">Returns the archetype list.</response>
        [HttpGet("all-archetypes")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> AllArchetypes([FromQuery] int page = 1)
        {
            var result = await Sender.Send(new GetAllArchetypesQuery(Page: page));
            return Ok(result);
        }
    }
}

using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YuGiOh.Application.Features.TournamentManagement.Commands;
using YuGiOh.Application.Features.TournamentManagement.Queries;
using YuGiOh.Domain.Exceptions;
using YuGiOh.Domain.Models;

namespace YuGiOh.WebAPI.Controllers
{
    /// <summary>
    /// Provides endpoints for managing Yu-Gi-Oh! tournaments.
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class TournamentController : APIControllerBase
    {

        public TournamentController(IMediator mediator) : base(mediator)
        {
        }

        /// <summary>
        /// Creates a new tournament in the system.
        /// </summary>
        /// <param name="tournament">The tournament data to create.</param>
        /// <returns>A 201 Created response if successful, otherwise an appropriate error.</returns>
        /// <response code="201">Tournament successfully created.</response>
        /// <response code="400">Invalid input or validation failed.</response>
        /// <response code="500">Unexpected server error.</response>
        [HttpPost]
        [Authorize(Roles = "Sponsor,Staff,Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateTournament(
            [FromBody] RegisterTournamentCommand command)
        {
            try
            {
                var tournament = await Sender.Send(command);
                return CreatedAtAction(nameof(CreateTournament), new { id = tournament.Id });
            }
            catch (APIException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw APIException.Internal("Unexpected error while creating tournament.", ex.Message, ex);
            }
        }
        [HttpPost]
        [Authorize(Roles = "Player")]
        public async Task<IActionResult> Register(
            [FromBody] RegisterUserInTournamentCommand command)
        {

            // Extract the "sub" (subject) or "id" claim from the JWT
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                         ?? User.FindFirstValue("sub")
                         ?? User.FindFirstValue("id");
            bool authorized = await Sender.Send(new IsTheOwnerQuery(
                command.DecksId.First(),
                userId
            ));
            if (!authorized)
            {
                throw APIException.Unauthorized("Only the Deck Owner can register it in the tournament.");
            }
            await Sender.Send(command);
            return Ok();
        }
        [HttpPost]
        [Authorize(Roles = "Admin,Sponsor")]
        public async Task<IActionResult> RegisterManyPlayers(
            [FromBody] RegisterUserInTournamentCommand command)
        {
            await Sender.Send(command);
            return Ok();
        }
    }
}

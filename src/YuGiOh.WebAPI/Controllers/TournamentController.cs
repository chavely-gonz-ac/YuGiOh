using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YuGiOh.Application.Features.TournamentManagement.Commands;
using YuGiOh.Domain.Exceptions;
using YuGiOh.Domain.Models;

namespace YuGiOh.WebAPI.Controllers
{
    /// <summary>
    /// Provides endpoints for managing Yu-Gi-Oh! tournaments.
    /// </summary>
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class TournamentController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<TournamentController> _logger;

        public TournamentController(IMediator mediator, ILogger<TournamentController> logger)
        {
            _mediator = mediator;
            _logger = logger;
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
                var tournament = await _mediator.Send(command);
                return CreatedAtAction(nameof(CreateTournament), new { id = tournament.Id });
            }
            catch (APIException apiEx)
            {
                _logger.LogError(apiEx, "API error while creating tournament.");
                return StatusCode((int)apiEx.StatusCode, apiEx.ToSerializableObject());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while creating tournament.");
                var apiEx = APIException.Internal("Unexpected error while creating tournament.", ex.Message, ex);
                return StatusCode((int)apiEx.StatusCode, apiEx.ToSerializableObject());
            }
        }
    }
}

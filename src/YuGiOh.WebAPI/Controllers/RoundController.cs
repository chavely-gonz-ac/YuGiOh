// using System.Security.Claims;
// using MediatR;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Mvc;
// using YuGiOh.Application.Features.TournamentManagement.Commands;
// using YuGiOh.Application.Features.TournamentManagement.Queries;
// using YuGiOh.Domain.Exceptions;
// using YuGiOh.Domain.Models;

// namespace YuGiOh.WebAPI.Controllers
// {
//     [Authorize]
//     [ApiController]
//     [Route("api/[controller]/[action]")]
//     public class RoundController : APIControllerBase
//     {

//         public TournamentController(IMediator mediator) : base(mediator)
//         {
//         }

//         [HttpPost]
//         [Authorize(Roles = "Staff")]
//         public async Task<IActionResult> CreateClassificationRound(
//             [FromBody] RegisterTournamentCommand command)
//         {

//         }
//         [HttpPost]
//         [Authorize(Roles = "Player")]
//         public async Task<IActionResult> Register(
//             [FromBody] RegisterUserInTournamentCommand command)
//         {
//         }
//         [HttpPost]
//         [Authorize(Roles = "Admin,Sponsor")]
//         public async Task<IActionResult> RegisterManyPlayers(
//             [FromBody] RegisterUserInTournamentCommand command)
//         {
//         }
//     }
// }

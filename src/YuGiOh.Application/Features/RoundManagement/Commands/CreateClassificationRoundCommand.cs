// using Ardalis.Specification;
// using MediatR;
// using YuGiOh.Domain.Services;

// namespace YuGiOh.Application.Features.RoundManagement.Commands
// {
//     public class CreateClassificationRoundCommand : IRequest<int>
//     {
//         public int TournamentId { get; set; }
//         public int MaximumNumberOfKnockoutRounds { get; set; }
//     }

//     public class CreateClassificationRoundCommandHandler : IRequestHandler<CreateClassificationRoundCommand, int>
//     {
//         private readonly IRepositoryBase<Round> _registerHandler;

//         public CreateClassificationRoundCommandHandler(IRegisterHandler registerHandler)
//         {
//             _registerHandler = registerHandler
//                 ?? throw new ArgumentNullException(nameof(registerHandler));
//         }
//         public async Task<int> Handle(CreateClassificationRoundCommand request, CancellationToken cancellationToken)
//         {
//         }
//     }
// }

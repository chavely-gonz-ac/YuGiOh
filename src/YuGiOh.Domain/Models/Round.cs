using YuGiOh.Domain.Enums;

namespace YuGiOh.Domain.Models
{
    public class Round
    {
        public int Id { get; set; }
        public RoundType Type { get; set; }
        public int TournamentId { get; set; }
        public Tournament? Tournament { get; set; }

        public IEnumerable<Match> Matches { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
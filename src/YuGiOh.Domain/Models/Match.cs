namespace YuGiOh.Domain.Models
{
    public class Match
    {
        public int Id { get; set; }
        public int RoundId { get; set; }
        public Round? Round { get; set; }
        public bool IsRunning { get; set; }
        public bool IsFinished { get; set; }
        public DateTime StartDate { get; set; }

        public required string WhitePlayerId { get; set; }
        public Player? WhitePlayer { get; set; }
        public int WhitePlayerResult { get; set; }

        public required string BlackPlayerId { get; set; }
        public Player? BlackPlayer { get; set; }
        public int BlackPlayerResult { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
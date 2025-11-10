namespace YuGiOh.Domain.Models
{
    /// <summary>
    /// Represents a sponsor entity that financially supports one or more Yu-Gi-Oh! tournaments.
    /// </summary>
    /// <remarks>
    /// Each sponsor may have multiple sponsorship relationships with different tournaments.
    /// </remarks>
    public class Sponsor
    {
        public string Id { get; set; }


        /// <summary>
        /// Gets or sets the collection of sponsorships associated with this sponsor.
        /// </summary>
        /// <remarks>
        /// Each <see cref="Sponsorship"/> entry links this sponsor to a specific tournament.
        /// </remarks>
        public ICollection<Sponsorship> Sponsorships { get; set; } = new List<Sponsorship>();
    }
}

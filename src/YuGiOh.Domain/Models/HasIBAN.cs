namespace YuGiOh.Domain.Models
{
    /// <summary>
    /// Represents an entity (staff, sponsor, or admin)
    /// that is associated with a financial account (IBAN)
    /// for transactions, payments, or revenue distribution.
    /// </summary>
    public class HasIBAN
    {
        /// <summary>
        /// Gets or sets the unique identifier of this entity.
        /// This corresponds to the <c>Account.Id</c> from the identity system.
        /// </summary>
        public required string Id { get; set; }

        /// <summary>
        /// Gets or sets the International Bank Account Number (IBAN)
        /// used for receiving or sending funds.
        /// </summary>
        public required string IBAN { get; set; }

        /// <summary>
        /// Returns a readable string representation of this entity.
        /// </summary>
        public override string ToString() => $"{Id}:{IBAN}";
    }
}

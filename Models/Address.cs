using System.ComponentModel.DataAnnotations;

namespace tp_hospital.Models;
public class Address
{
    [MaxLength(200)]
    public string Street { get; set; } = string.Empty;

    [MaxLength(100)]
    public string City { get; set; } = string.Empty;

    [MaxLength(10)]
    public string PostalCode { get; set; } = string.Empty;

    [MaxLength(100)]
    public string Country { get; set; } = "France";

    public override string ToString() =>
        string.IsNullOrWhiteSpace(Street) ? "—"
        : $"{Street}, {PostalCode} {City}, {Country}";
}

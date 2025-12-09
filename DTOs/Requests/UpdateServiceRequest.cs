namespace arabia.DTOs.Requests;

public class UpdateServiceRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal? BasePrice { get; set; }
    public bool? IsActive { get; set; }
}

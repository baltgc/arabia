namespace arabia.DTOs.Requests;

public class CreateServiceRequestRequest
{
    public int BusinessId { get; set; }
    public int ServiceId { get; set; }
    public DateTime RequestedDate { get; set; }
    public DateTime? ScheduledDate { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal? EstimatedCost { get; set; }
}

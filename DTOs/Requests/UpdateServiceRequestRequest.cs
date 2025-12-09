namespace arabia.DTOs.Requests;

public class UpdateServiceRequestRequest
{
    public int? EmployeeId { get; set; }
    public string? Status { get; set; }
    public DateTime? ScheduledDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public string? Notes { get; set; }
    public decimal? EstimatedCost { get; set; }
    public decimal? ActualCost { get; set; }
}

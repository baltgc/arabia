namespace arabia.Models;

public class ServiceRequest
{
    public int Id { get; set; }
    public int BusinessId { get; set; }
    public int ServiceId { get; set; }
    public int? EmployeeId { get; set; } // Nullable - can be assigned later
    public string Status { get; set; } = "Pending"; // Pending, Assigned, InProgress, Completed, Cancelled
    public DateTime RequestedDate { get; set; }
    public DateTime? ScheduledDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public decimal? EstimatedCost { get; set; }
    public decimal? ActualCost { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public Business Business { get; set; } = null!;
    public Service Service { get; set; } = null!;
    public Employee? Employee { get; set; }
}

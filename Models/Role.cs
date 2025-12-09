using Microsoft.AspNetCore.Identity;

namespace arabia.Models;

public class Role : IdentityRole<int>
{
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

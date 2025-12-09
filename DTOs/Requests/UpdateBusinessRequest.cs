namespace arabia.DTOs.Requests;

public class UpdateBusinessRequest
{
    public string? Name { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? ZipCode { get; set; }
    public string? ContactEmail { get; set; }
    public string? ContactPhone { get; set; }
    public string? ContactPerson { get; set; }
    public bool? IsActive { get; set; }
}

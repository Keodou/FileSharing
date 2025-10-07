namespace FileSharing.WebApi.Domain.Entities;

public class SharedFile
{
    public Guid Id { get; set; }
    public Guid FileId { get; set; }
    public string? Token { get; set; }
    public Guid? SharedWithUserId { get; set; }
    public DateTime SharedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ExpirationDate { get; set; }
    public FileModel File { get; set; } = null!;
}
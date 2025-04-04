namespace FileSharing.WebApi.Entities;

public class FileModel
{
    public Guid Id { get; set; }
    public required string FileName { get; set; }
    public required string ContentType { get; set; }
    public long Size { get; set; }
    public DateTime UploadDate { get; set; }
    public string? OwnerId { get; set; }
    public required string Path { get; set; }
 }
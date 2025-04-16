namespace FileSharing.WebApi.Entities;

public class FileModel
{
    public Guid Id { get; set; }
    public string FileName { get; set; } 
    public string ContentType { get; set; }
    public long Size { get; set; }
    public DateTime UploadDate { get; set; }
    public Guid OwnerId { get; set; }
    public string Path { get; set; }

    public User Owner { get; set; } = null!;
}
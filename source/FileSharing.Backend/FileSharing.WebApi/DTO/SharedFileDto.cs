namespace FileSharing.WebApi.DTO;

public class SharedFileDto
{
    public bool IsPublic { get; set; } = false;
    public Guid? SharedWithUserId { get; set; }
}
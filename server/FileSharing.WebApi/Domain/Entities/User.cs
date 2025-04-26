using FileSharing.WebApi.Application.Enums;

namespace FileSharing.WebApi.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public UserRole Role { get; set; } = UserRole.User;
        public string PasswordHash { get; set; } = string.Empty;

        public ICollection<FileModel> Files { get; set; } = new List<FileModel>();
    }
}

namespace FileSharing.WebApi.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;

        public ICollection<FileModel> Files { get; set; } = new List<FileModel>();
    }
}

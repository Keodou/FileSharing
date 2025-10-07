using FileSharing.WebApi.Domain.Entities;

namespace FileSharing.WebApi.Application.Interfaces;

public interface ITokenService
{
    string CreateToken(User user);
}
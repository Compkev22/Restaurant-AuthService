// UserDetailDto.cs
namespace AuthService.Application.DTOs;
using System.Text.Json.Serialization;

public class UserDetailsDto
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("userName")]
    public string UserName { get; set; } = string.Empty;

    [JsonPropertyName("userSurname")]
    public string UserSurname { get; set; } = string.Empty;

    [JsonPropertyName("systemUsername")]
    public string SystemUsername { get; set; } = string.Empty;

    [JsonPropertyName("userEmail")]
    public string UserEmail { get; set; } = string.Empty;

    [JsonPropertyName("role")]
    public string Role { get; set; } = string.Empty;

    [JsonPropertyName("userStatus")]
    public string UserStatus { get; set; } = string.Empty;

    [JsonPropertyName("isEmailVerified")]
    public bool IsEmailVerified { get; set; }

    [JsonPropertyName("userCreatedAt")]
    public DateTime UserCreatedAt { get; set; }

    [JsonPropertyName("profilePicture")]
    public string ProfilePicture { get; set; } = string.Empty;

    [JsonPropertyName("phone")]
    public string? Phone { get; set; }

    [JsonPropertyName("branchId")]
    public string? BranchId { get; set; }
}
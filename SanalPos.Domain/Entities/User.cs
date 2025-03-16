using System;
using SanalPos.Domain.Common;
using SanalPos.Domain.Enums;

namespace SanalPos.Domain.Entities
{
    public class User : BaseEntity
    {
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public string Role { get; set; } = null!;
        public bool IsActive { get; set; }
        public DateTime? LastLoginDate { get; set; }

        public string FullName => $"{FirstName} {LastName}";

        public User()
        {
            // EF Core için gerekli
        }

        public User(string email, string username, string passwordHash, string role, string firstName = null, string lastName = null, string phoneNumber = null)
        {
            Email = email;
            UserName = username;
            PasswordHash = passwordHash;
            Role = role;
            FirstName = firstName;
            LastName = lastName;
            PhoneNumber = phoneNumber;
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
        }

        public void UpdateProfile(string firstName, string lastName, string phoneNumber)
        {
            FirstName = firstName;
            LastName = lastName;
            PhoneNumber = phoneNumber;
        }

        public void ChangePassword(string newPasswordHash)
        {
            PasswordHash = newPasswordHash;
        }

        public void UpdateEmail(string email)
        {
            Email = email;
        }

        public void SetLastLogin()
        {
            LastLoginDate = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        public void Activate()
        {
            IsActive = true;
        }

        public void ChangeRole(string newRole)
        {
            Role = newRole;
        }
    }
} 
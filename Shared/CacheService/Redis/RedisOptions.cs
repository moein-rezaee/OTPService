using System.ComponentModel.DataAnnotations;

namespace Shared.CacheService.Redis
{
    public class RedisOptions
    {
        public string Host { get; set; } = "localhost";
        public int Port { get; set; } = 6379;
        
        [Required(ErrorMessage = "Redis password is required")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Redis connection string is required")]
        public string ConnectionString
        {
            get => $"{Password}@{Host}:{Port}";
            set { }
        }

        public bool IsValid(out ICollection<ValidationResult> results)
        {
            results = new List<ValidationResult>();
            return Validator.TryValidateObject(this, new ValidationContext(this), results, true);
        }
    }
}
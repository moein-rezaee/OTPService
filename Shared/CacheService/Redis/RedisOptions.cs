namespace CacheExtension.Redis;

public class RedisOptions
{
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 6379;
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? InstanceName { get; set; }
    public int? DefaultDatabase { get; set; }
}

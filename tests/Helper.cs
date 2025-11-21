using System.Text;
using Microsoft.Extensions.Configuration;

namespace Toolkit.Minio.Tests;

public static class Helper
{
    public const string Client1 = "Minio";

    public const string Client2 = "Minio2";
    
    public const string HostClient = "Host";
    
    public static readonly Dictionary<string, string> InMemorySettings = new () {
        {$"{Client1}:Endpoint", "play.min.io"},
        {$"{Client1}:AccessKey", "accessKey"},
        {$"{Client1}:SecretKey", "secretKey"},
        {$"{Client1}:Region", "region"},
        {$"{Client1}:SessionToken", "sessionToken"},
        {$"{Client1}:Timeout", "2000"},
        {$"{Client1}:SSL", "false"},
        
        {$"{Client2}:Endpoint", "play.min.io"},
        {$"{Client2}:AccessKey", "accessKey2"},
        {$"{Client2}:SecretKey", "secretKey2"},
        {$"{Client2}:Region", "region2"},
        {$"{Client2}:SessionToken", "sessionToken2"},
        {$"{Client2}:Timeout", "2000"},
        {$"{Client2}:SSL", "false"},
        
        // For Match
        {"Match:Endpoint", "play.min.io"},
        {"Match:AccessKey", "accessKey"},
        {"Match:SecretKey", "secretKey"},
        
        {"Match2:Endpoint", "play.min.io"},
        {"Match2:AccessKey", "accessKey"},
        {"Match2:SecretKey", "secretKey"},
        {"Match2:Region", "region"},
        {"Match2:SSL", "false"},
        
        // https://github.com/minio/minio-dotnet/blob/d59bb032d66e6df4664e1e3a6394cad33223523d/Minio.Functional.Tests/Program.cs#L63
        
        {$"{HostClient}:Endpoint", "play.min.io"},
        {$"{HostClient}:AccessKey", "Q3AM3UQ867SPQQA43P2F"},
        {$"{HostClient}:SecretKey", "zuf+tfteSlswRu7BJ86wekitnifILbZam1KYY3TG"},
        {$"{HostClient}:SSL", "true"}
    };

    public static IConfiguration GetConfiguration(Dictionary<string, string>? inMemorySettings = null)
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection((inMemorySettings ?? InMemorySettings)!)
            .Build();
        
        return configuration;
    }
    
    public static string GetRandomObjectName(int length = 5)
    {
        // Server side does not allow the following characters in object names
        // '-', '_', '.', '/', '*'
        const string characters = "abcd+%$#@&{}[]()";
        
        var result = new StringBuilder(length);

        for (var i = 0; i < length; i++) result.Append(characters[Random.Shared.Next(characters.Length)]);
        return result.ToString();
    }
    
    // Generate a random string
    public static string GetRandomName(int length = 5)
    {
        const string characters = "0123456789abcdefghijklmnopqrstuvwxyz";
        if (length > 50) length = 50;

        var result = new StringBuilder(length);
        for (var i = 0; i < length; i++) _ = result.Append(characters[Random.Shared.Next(characters.Length)]);

        return "minio-dotnet-example-" + result;
    }
}
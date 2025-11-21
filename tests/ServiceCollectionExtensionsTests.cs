global using Xunit;
global using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Minio;
using Toolkit.Minio.Entities;
using Toolkit.Minio.Extensions;

namespace Toolkit.Minio.Tests;

//Employee_GetFullName_When_FirstString_IsNull_Throw_ArgumentNullException()

public class ServiceCollectionExtensionsTests
{
    private readonly IServiceCollection _services;
    
    public ServiceCollectionExtensionsTests()
    {
        var configuration = Helper.GetConfiguration();
        
        _services = new ServiceCollection()
            .AddMinio(Helper.Client1, configuration)
            .AddMinio(Helper.Client2, configuration)
            .AddMinio("Match", configuration)
            .AddMinio("Match2", configuration);
    }
    
    
    [Fact]
    public void AddToServices_ReturnContains()
    {
        Assert.Contains(_services, d => d.ServiceType == typeof(IMinioClientFactory));
        Assert.Contains(_services, d => d.ServiceType == typeof(IConfigureOptions<MinioOptions>));
        Assert.Contains(_services, d => d.ServiceType == typeof(IMinioClient));
    }

    
    [Fact]
    public void GetFromServices_Clients_ReturnNotNull()
    {
        using var serviceProvider = _services.BuildServiceProvider();
        
        var factory = serviceProvider.GetService<IMinioClientFactory>();
        Assert.NotNull(factory);
        
        var client = serviceProvider.GetService<IMinioClient>();
        Assert.NotNull(client);
    }
    
    
    [Theory]
    [InlineData("Minio")]
    [InlineData("Minio2")]
    public void Match_OptionsWithAppSettings_ReturnEqual(string name)
    {
        using var serviceProvider = _services.BuildServiceProvider();
        
        var optionsMonitor = serviceProvider.GetService<IOptionsMonitor<MinioOptions>>();
        Assert.NotNull(optionsMonitor);
        
        var options = optionsMonitor.Get(name);
        Assert.NotNull(options);
        
        Assert.Equal(Helper.InMemorySettings[$"{name}:{nameof(options.Endpoint)}"], options.Endpoint);
        Assert.Equal(Helper.InMemorySettings[$"{name}:{nameof(options.AccessKey)}"], options.AccessKey);
        Assert.Equal(Helper.InMemorySettings[$"{name}:{nameof(options.SecretKey)}"], options.SecretKey);
        Assert.Equal(Helper.InMemorySettings[$"{name}:{nameof(options.Region)}"], options.Region);
        Assert.Equal(Helper.InMemorySettings[$"{name}:{nameof(options.SessionToken)}"], options.SessionToken);
        Assert.Equal(Helper.InMemorySettings[$"{name}:{nameof(options.Timeout)}"], options.Timeout.ToString());
        Assert.Equal(Helper.InMemorySettings[$"{name}:{nameof(options.SSL)}"], options.SSL.ToString().ToLower());
    }
    
    [Theory]
    [InlineData("Match")]
    [InlineData("Match2")]
    public void Match_OptionsWithClient_ReturnEqual(string name)
    {
        using var serviceProvider = _services.BuildServiceProvider();
        
        var client = serviceProvider.GetRequiredService<IMinioClientFactory>().CreateClient(name);
        Assert.NotNull(client);
        
        var options = serviceProvider.GetRequiredService<IOptionsMonitor<MinioOptions>>().Get(name);
        Assert.NotNull(options);
        
        var config = client.Config;
        
        Assert.Equal(options.Endpoint, config.BaseUrl);
        Assert.Equal(options.AccessKey, config.AccessKey);
        Assert.Equal(options.SecretKey, config.SecretKey);
        Assert.Equal(options.Region, config.Region);
        Assert.Equal(options.SessionToken, config.SessionToken);
        Assert.Equal(options.Timeout ?? 0, config.RequestTimeout);
        Assert.Equal(options.SSL, config.Secure);
    }
    
    [Fact]
    public void MultipleClients_Factory_ReturnNotSame()
    {
        using var serviceProvider = _services.BuildServiceProvider();

        var client1 = serviceProvider.GetRequiredService<IMinioClient>();
        var client2 = serviceProvider.GetRequiredService<IMinioClientFactory>().CreateClient(Helper.Client2);

        Assert.NotSame(client1, client2);
    }
}
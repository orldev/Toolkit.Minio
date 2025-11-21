using Minio.DataModel.Args;
using Minio.Exceptions;
using Toolkit.Minio.Entities;
using Toolkit.Minio.Extensions;
using Xunit.Abstractions;

namespace Toolkit.Minio.Tests;

public class MethodExtensionsTests
{
    private readonly ITestOutputHelper _testOutputHelper;
    private const int KB = 1024;
    private const int MB = 1024 * 1024;
    private const int GB = 1024 * 1024 * 1024;
    
    private static readonly RandomStreamGenerator Rsg = new(1 * KB);
    
    private readonly IServiceCollection _services;
    
    public MethodExtensionsTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        var configuration = Helper.GetConfiguration();
        
        _services = new ServiceCollection()
            .AddMinio(Helper.HostClient, configuration);
    }
    
    [Fact] 
    public async Task StatObject_ReturnEqual_NotFound_WhenBucketNotExists()
    {
        await using var serviceProvider = _services.BuildServiceProvider();
        var bucketName = Helper.GetRandomName(15);
        var objectName = Helper.GetRandomObjectName(10);
        
        var client = serviceProvider.GetRequiredService<IMinioClientFactory>().CreateClient(Helper.HostClient);
        Assert.NotNull(client);
        
        var statObject = await client.StatObjectAsync(bucketName, objectName);
        
        Assert.Equal(statObject.ErrorType, MinioErrorType.BucketNotFound);
    }
    
    [Fact] 
    public async Task PutObject_Stream_ReturnEqual_NotFound_WhenBucketNotExists()
    {
        await using var serviceProvider = _services.BuildServiceProvider();
        var bucketName = Helper.GetRandomName(15);
        var objectName = Helper.GetRandomObjectName(10);
        const string contentType = "application/octet-stream";
        
        var client = serviceProvider.GetRequiredService<IMinioClientFactory>().CreateClient(Helper.HostClient);
        Assert.NotNull(client);

        await using var filestream = Rsg.GenerateStreamFromSeed(1 * KB);
        var statObject = await client.PutStreamAsync(
            bucketName, 
            filestream, 
            contentType, 
            objectName);
        
        Assert.Equal(statObject.ErrorType, MinioErrorType.BucketNotFound);
    }
    
    [Fact] 
    public async Task RemoveObject_ReturnEqual_NotFound_WhenBucketNotExists()
    {
        await using var serviceProvider = _services.BuildServiceProvider();
        var bucketName = Helper.GetRandomName(15);
        var objectName = Helper.GetRandomObjectName(10);
        
        var client = serviceProvider.GetRequiredService<IMinioClientFactory>().CreateClient(Helper.HostClient);
        Assert.NotNull(client);

        await using var filestream = Rsg.GenerateStreamFromSeed(1 * KB);
        var statObject = await client.RemoveObjectAsync(
            bucketName, 
            objectName);
        
        Assert.Equal(statObject.ErrorType, MinioErrorType.BucketNotFound);
    }
    
    [Fact]
    public async Task GetObject_Stream_ReturnNull_WhenBucketNotExists()
    {
        await using var serviceProvider = _services.BuildServiceProvider();
        var bucketName = Helper.GetRandomName(15);
        var objectName = Helper.GetRandomObjectName(10);
        
        var client = serviceProvider.GetRequiredService<IMinioClientFactory>().CreateClient(Helper.HostClient);
        Assert.NotNull(client);
        
        var objectStat = await client.DownloadObjectAsync(bucketName, objectName);    
            
        Assert.Equal(objectStat.ErrorType, MinioErrorType.BucketNotFound);
    }
    
    [Fact]
    public async Task PutObject_GetObject_ReturnSuccess()
    {
        await using var serviceProvider = _services.BuildServiceProvider();
        var bucketName = Helper.GetRandomName(15);
        var objectName = Helper.GetRandomObjectName(10);
        const string contentType = "application/octet-stream";
        
        var forceFlagHeader = new Dictionary<string, string>
            (StringComparer.Ordinal) { { "x-minio-force-delete", "true" } };
        
        var bucketExistArgs = new BucketExistsArgs()
            .WithBucket(bucketName);
        var makeBucketArgs = new MakeBucketArgs()
            .WithBucket(bucketName);
        var removeBucketArgs = new RemoveBucketArgs()
            .WithBucket(bucketName)
            .WithHeaders(forceFlagHeader);
    
        
        _testOutputHelper.WriteLine($"Bucket: {bucketName} Object: {objectName}");
        
        var client = serviceProvider.GetRequiredService<IMinioClientFactory>().CreateClient(Helper.HostClient);
        Assert.NotNull(client);
        
        try
        {
            var notFound = await client.BucketExistsAsync(bucketExistArgs);
            Assert.False(notFound);
            
            await client.MakeBucketAsync(makeBucketArgs);
            
            var found = await client.BucketExistsAsync(bucketExistArgs);
            Assert.True(found);
            
            await using var filestream = Rsg.GenerateStreamFromSeed(1 * KB);
            var statObject = await client.PutStreamAsync(
                bucketName, 
                filestream, 
                contentType, 
                objectName);
            Assert.True(statObject.IsSuccess);
            // TODO: Fix
            // var objectStat = await client.StatObjectAsync(bucketName, objectName);
            // Assert.True(objectStat.IsSuccess);
        }
        catch (MinioException e)
        {
            _testOutputHelper.WriteLine("{0}", e);
        }
        finally
        {
            await client.RemoveBucketAsync(removeBucketArgs);     
        }
    }
}
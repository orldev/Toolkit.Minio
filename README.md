# Minio

Extension for the framework `Minio` - because why use simple storage when you can have complicated dependency injection with **actual error handling**?

## Connecting the configuration (pick your poison)

```c# 
// Option 1: The basic "I hope this works" approach
builder.Services.AddMinio(builder.Configuration);
```

```c# 
// Option 2: The "I have multiple storage accounts and I like pain" approach
builder.Services.AddMinio("Minio1", builder.Configuration);
```

```c# 
// Option 3: The "I read the documentation (unlike you)" approach
builder.Services.AddMinio(builder.Configuration, configureClient => configureClient
    .WithProxy()                    // Because corporate firewalls are fun!
    .WithRetryPolicy()              // For when hope is not a strategy
    .WithCredentialsProvider()      // Password? What password?
    .WithHttpClient());             // Because why use the default one?
```

## Sample configuration appsettings.json for using Minio

```json
{
  "Minio": {
    "Endpoint": "play.min.io",      // Where your data goes to hide
    "AccessKey": "accessKey",       // The key you'll inevitably commit to GitHub
    "SecretKey": "secretKey",       // The secret you'll rotate every 90 days (or not)
    "Region": "region",             // - optional (like most meetings)
    "SessionToken": "sessionToken", // - optional (temporary, like your motivation)
    "Timeout": 2000,                // - optional (how long to wait before giving up)
    "SSL": false                    // - optional (because security is optional, right?)
  }
}
```

## Actually Useful Error Handling!

Tired of exceptions crashing your party? Meet `MinioResult` - because sometimes failure is an option!

### Civilized Error Handling
```csharp
// Upload files like a pro
var result = await minioClient.PutObjectAsync("bucket", "object", stream);
result.Match(
    onSuccess: response => Console.WriteLine($"Uploaded! ETag: {response.Etag}"),
    onFailure: (errorType, message) => Console.WriteLine($"Failed with {errorType}: {message}")
);

// Download without the drama
var downloadResult = await minioClient.DownloadObjectAsync("bucket", "object");
if (downloadResult.IsSuccess)
{
    using var stream = downloadResult.Value;
    // Do something amazing with your stream
}
else if (downloadResult.ErrorType == MinioErrorType.ObjectNotFound)
{
    Console.WriteLine("The object is on a coffee break");
}

// Remove objects safely
var removeResult = await minioClient.RemoveObjectAsync("bucket", "object");
if (!removeResult.IsSuccess)
{
    _logger.LogWarning("Delete failed, but at least we didn't crash!");
}
```

### Advanced Error Handling (For Overachievers):
```csharp
// Pattern matching FTW!
var result = await minioClient.StatObjectAsync("bucket", "object")
    .Match(
        onSuccess: stat => new { Exists = true, Size = stat.Size },
        onFailure: (errorType, message) => new { Exists = false, Error = errorType }
    );

// Functional programming magic
var fileInfo = await minioClient.GetObjectAsync("bucket", "file.txt")
    .Match(
        onSuccess: objectStat => $"File size: {objectStat.Size} bytes",
        onFailure: (errorType, message) => $"Error: {errorType}"
    );

// Chain operations like a boss
var operation = await minioClient.PutObjectAsync("bucket", "object", stream)
    .Match(
        onSuccess: _ => "Upload successful",
        onFailure: (errorType, _) => errorType switch
        {
            MinioErrorType.Authorization => "Check your credentials",
            MinioErrorType.BucketNotFound => "Bucket doesn't exist",
            MinioErrorType.Connection => "Network issues",
            _ => "Something went wrong"
        }
    );
```

### Available Operations (That Actually Return Useful Results):

| Operation | Returns | When to Use |
|-----------|---------|-------------|
| `PutObjectAsync` | `MinioResult<PutObjectResponse>` | Uploading files with proper error info |
| `PutStreamAsync` | `MinioResult<PutObjectResponse>` | Streaming uploads with auto-naming |
| `GetObjectAsync` | `MinioResult<ObjectStat>` | Getting object metadata + data |
| `DownloadObjectAsync` | `MinioResult<Stream>` | Downloading to MemoryStream |
| `DownloadObjectAsync` (with stream) | `MinioResult<ObjectStat>` | Downloading to existing stream |
| `RemoveObjectAsync` | `MinioResult` | Deleting objects (no return data) |
| `StatObjectAsync` | `MinioResult<ObjectStat>` | Checking if object exists |

### Error Types You Can Actually Handle:

- `Authorization` - Your credentials are lying to you
- `BucketNotFound` - The bucket is in another castle
- `ObjectNotFound` - The object joined the witness protection program
- `Connection` - The server is ignoring your calls
- `Timeout` - The server is taking a nap
- `InvalidBucketName` - You used emojis in the bucket name, didn't you?
- ...and many more!

## What Could Possibly Go Wrong? (Spoiler: Everything, but now you can handle it!)

```csharp
var result = await minioClient.SomeOperation();
if (!result.IsSuccess)
{
    switch (result.ErrorType)
    {
        case MinioErrorType.Authorization:
            await _authService.RefreshToken();
            break;
        case MinioErrorType.BucketNotFound:
            await _notificationService.AlertMissingBucket();
            break;
        case MinioErrorType.Connection:
            await _retryService.RetryWithBackoff();
            break;
        default:
            _logger.LogError("Specific error: {ErrorType}", result.ErrorType);
            break;
    }
}
```

## Documentation (That You Might Actually Read Now)

- [.NET Client API Reference](https://min.io/docs/minio/linux/developers/dotnet/API.html) - The manual you'll open before everything breaks
- [.NET Quickstart Guide](https://min.io/docs/minio/linux/developers/dotnet/minio-dotnet.html) - "Quick" being slightly less relative now

## Source Code (For the Brave & Curious)

- [The original source](https://github.com/appany/Minio.AspNetCore/tree/main) - Where the magic (and hopefully fewer bugs) happen
- [MimeTypeMap](https://github.com/samuelneff/MimeTypeMap) - Because guessing file types is still hard

## License

Snail.Toolkit.Minio is a free and open source project, released under the permissible [MIT license](LICENSE).

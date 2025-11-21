using MimeTypes;
using Xunit.Abstractions;

namespace Toolkit.Minio.Tests;

public class MimeTypeMapTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public MimeTypeMapTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Theory]
    [InlineData("audio/aac")]
    [InlineData("image/bmp")]
    public void GetExtension_ReturnNotEmpty(string type)
    { 
        var extension = MimeTypeMap.GetExtension(type, false);
        _testOutputHelper.WriteLine(extension);
        Assert.NotEmpty(extension);
    }
    
    [Theory]
    [InlineData("test/test")]
    public void GetExtension_ReturnEmpty(string type)
    { 
        var extension = MimeTypeMap.GetExtension(type, false);
        Assert.Empty(extension);
    }
    
    [Theory]
    [InlineData("test/test")]
    public void GetExtension_Throw_ArgumentNullException(string type)
    {
        Assert.Throws<ArgumentException>(()=> MimeTypeMap.GetExtension(type));
    }
    
    [Theory]
    [InlineData("test.png")]
    [InlineData("test.zip")]
    public void GetGetMimeType_ReturnNotEmpty(string str)
    { 
        var mimeType = MimeTypeMap.GetMimeType(str);
        _testOutputHelper.WriteLine(mimeType);
        Assert.NotEmpty(mimeType);
    }
}
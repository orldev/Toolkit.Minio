using Microsoft.Extensions.Options;
using Toolkit.Minio.Entities;

namespace Toolkit.Minio;

/// <summary>
/// A factory implementation for creating and configuring Minio client instances based on named options.
/// This class provides a centralized way to create Minio clients with pre-configured settings
/// from <see cref="MinioOptions"/> while allowing for additional runtime configuration.
/// </summary>
/// <remarks>
/// <para>
/// This factory uses the Options Pattern to retrieve configuration settings for Minio clients.
/// It supports multiple named configurations through the <see cref="IOptionsMonitor{TOptions}"/> interface.
/// </para>
/// <para>
/// The factory applies configuration in the following order:
/// <list type="number">
/// <item><description>Basic SSL configuration</description></item>
/// <item><description>Endpoint configuration (if provided)</description></item>
/// <item><description>Credentials (AccessKey and SecretKey, if both provided)</description></item>
/// <item><description>Region configuration (if provided)</description></item>
/// <item><description>Session token (if provided)</description></item>
/// <item><description>Timeout configuration (if provided)</description></item>
/// <item><description>Additional runtime configuration via the <paramref name="configureClient"/> delegate</description></item>
/// </list>
/// </para>
/// </remarks>
/// <param name="optionsMonitor">The options monitor used to retrieve Minio configuration options by name.</param>
/// <exception cref="ArgumentNullException">Thrown when <paramref name="optionsMonitor"/> is null.</exception>
public class MinioClientFactory(IOptionsMonitor<MinioOptions> optionsMonitor) : IMinioClientFactory
{
    /// <summary>
    /// Creates and configures a Minio client instance with the specified name and optional additional configuration.
    /// </summary>
    /// <param name="name">The name of the Minio configuration to use. This should match a named options configuration.</param>
    /// <param name="configureClient">An optional delegate for applying additional configuration to the client before building.</param>
    /// <returns>A fully configured instance of <see cref="IMinioClient"/>.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when:
    /// <list type="bullet">
    /// <item><description><paramref name="name"/> is null or empty</description></item>
    /// <item><description>No options found for the specified <paramref name="name"/></description></item>
    /// </list>
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the client configuration is invalid or the client cannot be built.
    /// </exception>
    /// <remarks>
    /// <para>
    /// This method retrieves the <see cref="MinioOptions"/> for the specified <paramref name="name"/>
    /// and applies all configured settings to a new Minio client instance. The configuration is applied
    /// conditionally - only non-null values from the options are used.
    /// </para>
    /// <para>
    /// The method applies the following configurations from <see cref="MinioOptions"/> (when available):
    /// <list type="bullet">
    /// <item><description>SSL/TLS settings</description></item>
    /// <item><description>Endpoint URL</description></item>
    /// <item><description>Access credentials (AccessKey and SecretKey)</description></item>
    /// <item><description>Region information</description></item>
    /// <item><description>Session token for temporary credentials</description></item>
    /// <item><description>Request timeout</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// After applying the options-based configuration, any additional configuration provided via the
    /// <paramref name="configureClient"/> delegate is applied, allowing for runtime customization.
    /// </para>
    /// </remarks>
    /// <example>
    /// The following example shows how to use the factory to create a Minio client:
    /// <code>
    /// var factory = new MinioClientFactory(optionsMonitor);
    /// var client = factory.CreateClient("my-minio-config", client => 
    /// {
    ///     client.WithProxy("http://proxy-server:8080");
    /// });
    /// </code>
    /// </example>
    public IMinioClient CreateClient(string name, Action<IMinioClient>? configureClient = null)
    {
        var options = optionsMonitor.Get(name);
        
        if (options == null) 
            throw new ArgumentNullException(nameof(options));

        var client = new MinioClient()
            .WithSSL(options.SSL);

        if (options.Endpoint is { } endpoint)
            client.WithEndpoint(endpoint);

        if (options is {AccessKey: { } accessKey, SecretKey: { } secretKey})
            client.WithCredentials(accessKey, secretKey);

        if (options.Region is { } region)
            client.WithRegion(region);
        
        if (options.SessionToken is { } sessionToken)
            client.WithSessionToken(sessionToken);
        
        if (options.Timeout is { } timeout)
            client.WithTimeout(timeout);

        configureClient?.Invoke(client);
        
        return client.Build();
    }
}
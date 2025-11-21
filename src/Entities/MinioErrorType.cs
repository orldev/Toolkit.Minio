namespace Toolkit.Minio.Entities;

/// <summary>
/// Represents the specific types of errors that can occur during Minio operations.
/// </summary>
/// <remarks>
/// <para>
/// This enumeration provides a comprehensive classification of error scenarios when working with Minio storage.
/// Instead of relying on exception types, operations return <see cref="MinioResult"/> with a specific
/// <see cref="MinioErrorType"/> that allows for precise error handling and recovery strategies.
/// </para>
/// <para>
/// Each error type corresponds to specific failure conditions that can occur during Minio client operations,
/// ranging from authentication issues to network problems and invalid operations.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// var result = await minioClient.PutObjectAsync("bucket", "object");
/// if (!result.IsSuccess)
/// {
///     switch (result.ErrorType)
///     {
///         case MinioErrorType.Authorization:
///             // Handle invalid credentials
///             break;
///         case MinioErrorType.BucketNotFound:
///             // Create bucket or show appropriate message
///             break;
///         case MinioErrorType.Connection:
///             // Retry or show connectivity error
///             break;
///     }
/// }
/// </code>
/// </example>
public enum MinioErrorType
{
    /// <summary>
    /// The operation completed successfully without any errors.
    /// </summary>
    None,

    /// <summary>
    /// The access key or secret key provided for authentication is invalid or expired.
    /// </summary>
    /// <remarks>
    /// This error typically occurs when:
    /// <list type="bullet">
    /// <item><description>The credentials are incorrect</description></item>
    /// <item><description>The credentials have been revoked</description></item>
    /// <item><description>The user doesn't have necessary permissions</description></item>
    /// </list>
    /// Check your Minio server configuration and ensure the credentials are valid.
    /// </remarks>
    Authorization,

    /// <summary>
    /// The specified bucket name does not conform to DNS naming conventions.
    /// </summary>
    /// <remarks>
    /// Bucket names must:
    /// <list type="bullet">
    /// <item><description>Be between 3 and 63 characters long</description></item>
    /// <item><description>Contain only lowercase letters, numbers, and hyphens</description></item>
    /// <item><description>Start and end with a letter or number</description></item>
    /// <item><description>Not contain consecutive hyphens</description></item>
    /// <item><description>Not be formatted as an IP address</description></item>
    /// </list>
    /// </remarks>
    InvalidBucketName,

    /// <summary>
    /// The specified object name is invalid or contains unsupported characters.
    /// </summary>
    /// <remarks>
    /// Object names should be valid UTF-8 strings and avoid characters that might cause issues
    /// with URL encoding or filesystem operations. Avoid using control characters and special
    /// characters that have special meaning in URLs.
    /// </remarks>
    InvalidObjectName,

    /// <summary>
    /// The specified bucket does not exist in the Minio server.
    /// </summary>
    /// <remarks>
    /// This error occurs when trying to perform operations on a bucket that hasn't been created
    /// or has been deleted. Check the bucket name for typos or create the bucket before use.
    /// </remarks>
    BucketNotFound,

    /// <summary>
    /// The specified object does not exist in the bucket.
    /// </summary>
    /// <remarks>
    /// Verify that the object exists in the specified bucket and that you have the necessary
    /// permissions to access it. This error can also occur if the object was recently deleted.
    /// </remarks>
    ObjectNotFound,

    /// <summary>
    /// The requested functionality or extension is not implemented in the current version.
    /// </summary>
    /// <remarks>
    /// This error indicates that you're trying to use a feature that isn't supported by the
    /// current Minio client implementation or server version. Check the documentation for
    /// supported features and version compatibility.
    /// </remarks>
    NotImplemented,

    /// <summary>
    /// The source file for a copy or upload operation could not be found.
    /// </summary>
    /// <remarks>
    /// This error occurs when trying to upload or copy from a local file that doesn't exist
    /// or is inaccessible. Verify the file path and ensure the application has read permissions.
    /// </remarks>
    FileNotFound,

    /// <summary>
    /// An operation was attempted on a disposed stream or object.
    /// </summary>
    /// <remarks>
    /// This typically happens when trying to read from or write to a stream that has already
    /// been disposed. Ensure that streams are properly managed and not disposed prematurely.
    /// </remarks>
    ObjectDisposed,

    /// <summary>
    /// The operation is not supported on the current stream or object.
    /// </summary>
    /// <remarks>
    /// For example, trying to write to a read-only stream or read from a write-only stream.
    /// Check the capabilities of the stream and ensure you're using it appropriately.
    /// </remarks>
    NotSupported,

    /// <summary>
    /// The operation is invalid for the current state of the object or stream.
    /// </summary>
    /// <remarks>
    /// This can occur when trying to perform conflicting operations simultaneously, such as
    /// reading from a stream while another read operation is in progress.
    /// </remarks>
    InvalidOperation,

    /// <summary>
    /// Access was denied due to insufficient permissions or encryption key issues.
    /// </summary>
    /// <remarks>
    /// This error can occur when:
    /// <list type="bullet">
    /// <item><description>The user lacks necessary permissions for the operation</description></item>
    /// <item><description>An incorrect encryption key was provided for encrypted operations</description></item>
    /// <item><description>The bucket policy denies the requested action</description></item>
    /// </list>
    /// </remarks>
    AccessDenied,

    /// <summary>
    /// Unable to establish a connection to the Minio server.
    /// </summary>
    /// <remarks>
    /// This error indicates network connectivity issues. Check:
    /// <list type="bullet">
    /// <item><description>Network connectivity and firewall settings</description></item>
    /// <item><description>The Minio server endpoint and port</description></item>
    /// <item><description>Whether the Minio server is running and accessible</description></item>
    /// <item><description>DNS resolution for the server hostname</description></item>
    /// </list>
    /// </remarks>
    Connection,

    /// <summary>
    /// A required argument was null or missing.
    /// </summary>
    /// <remarks>
    /// This validation error occurs when essential parameters like bucket name, object name,
    /// or credentials are not provided. Ensure all required parameters are supplied.
    /// </remarks>
    ArgumentNull,

    /// <summary>
    /// The operation timed out before completion.
    /// </summary>
    /// <remarks>
    /// This can happen with large file transfers, slow network connections, or when the
    /// Minio server is under heavy load. Consider increasing the timeout value or
    /// optimizing the operation for better performance.
    /// </remarks>
    Timeout,

    /// <summary>
    /// A Minio-specific error occurred that doesn't match other specific error types.
    /// </summary>
    /// <remarks>
    /// This catch-all category covers Minio errors that don't fit into the other specific
    /// types. Check the error message for details about the specific Minio failure.
    /// </remarks>
    UnknownMinioError,

    /// <summary>
    /// An unexpected error occurred that is not specific to Minio operations.
    /// </summary>
    /// <remarks>
    /// This error type covers general .NET exceptions and unexpected system errors that
    /// occur during Minio operations but aren't directly related to Minio functionality.
    /// </remarks>
    UnexpectedError
}
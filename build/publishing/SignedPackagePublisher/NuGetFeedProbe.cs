using System.Net;
using NuGet.Common;
using NuGet.Configuration;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;

namespace SignedPackagePublisher;

public interface IPackageFeedProbe
{
	Task<bool> ExistsAsync(string id, NuGet.Versioning.NuGetVersion version, CancellationToken cancellationToken);
}

public sealed class NuGetFeedProbe : IPackageFeedProbe, IDisposable
{
	private readonly SourceRepository repository;
	private readonly SourceCacheContext cacheContext = new() {
		NoCache = true,
		DirectDownload = true,
	};
	private readonly int maxAttempts;

	public NuGetFeedProbe(Uri serviceIndex, string? token, int maxAttempts)
	{
		if (!serviceIndex.IsAbsoluteUri)
			throw new ArgumentException("The NuGet service index must be an absolute URI.", nameof(serviceIndex));
		if (maxAttempts < 1)
			throw new ArgumentOutOfRangeException(nameof(maxAttempts));

		var packageSource = new PackageSource(serviceIndex.AbsoluteUri);
		if (!string.IsNullOrWhiteSpace(token))
		{
			packageSource.Credentials = PackageSourceCredential.FromUserInput(
				serviceIndex.AbsoluteUri,
				"AzureDevOps",
				token,
				storePasswordInClearText: true,
				validAuthenticationTypesText: "basic");
		}

		repository = Repository.Factory.GetCoreV3(packageSource);
		this.maxAttempts = maxAttempts;
	}

	public async Task<bool> ExistsAsync(
		string id,
		NuGet.Versioning.NuGetVersion version,
		CancellationToken cancellationToken)
	{
		for (int attempt = 1; ; attempt++)
		{
			try
			{
				FindPackageByIdResource resource = await repository.GetResourceAsync<FindPackageByIdResource>(cancellationToken);
				return await resource.DoesPackageExistAsync(id, version, cacheContext, NullLogger.Instance, cancellationToken);
			}
			catch (Exception exception) when (attempt < maxAttempts
				&& FeedFailureClassifier.Classify(exception) == FeedFailureKind.Transient)
			{
				await Task.Delay(Backoff(attempt), cancellationToken);
			}
			catch (Exception exception) when (exception is not OperationCanceledException
				|| !cancellationToken.IsCancellationRequested)
			{
				FeedFailureKind kind = FeedFailureClassifier.Classify(exception);
				throw new FeedQueryException(
					$"NuGet feed lookup failed for '{id} {version.ToNormalizedString()}' ({kind}).",
					exception);
			}
		}
	}

	public void Dispose() => cacheContext.Dispose();

	private static TimeSpan Backoff(int attempt)
		=> TimeSpan.FromMilliseconds(Math.Min(5_000, 250 * Math.Pow(2, attempt - 1)));
}

public static class FeedFailureClassifier
{
	public static FeedFailureKind Classify(Exception exception)
	{
		foreach (Exception current in Enumerate(exception))
		{
			if (current is HttpRequestException httpException && httpException.StatusCode is HttpStatusCode statusCode)
				return Classify(statusCode);

			if (current is TaskCanceledException)
				return FeedFailureKind.Transient;
		}

		return FeedFailureKind.Unknown;
	}

	public static FeedFailureKind Classify(HttpStatusCode statusCode) => statusCode switch {
		HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden => FeedFailureKind.Authentication,
		HttpStatusCode.RequestTimeout or HttpStatusCode.TooManyRequests => FeedFailureKind.Transient,
		>= HttpStatusCode.InternalServerError => FeedFailureKind.Transient,
		_ => FeedFailureKind.Unknown,
	};

	private static IEnumerable<Exception> Enumerate(Exception exception)
	{
		for (Exception? current = exception; current is not null; current = current.InnerException)
			yield return current;
	}
}

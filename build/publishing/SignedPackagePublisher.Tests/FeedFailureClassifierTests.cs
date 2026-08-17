using System.Net;

namespace SignedPackagePublisher.Tests;

public sealed class FeedFailureClassifierTests
{
	[TestCase(HttpStatusCode.Unauthorized, FeedFailureKind.Authentication)]
	[TestCase(HttpStatusCode.Forbidden, FeedFailureKind.Authentication)]
	[TestCase(HttpStatusCode.RequestTimeout, FeedFailureKind.Transient)]
	[TestCase(HttpStatusCode.TooManyRequests, FeedFailureKind.Transient)]
	[TestCase(HttpStatusCode.InternalServerError, FeedFailureKind.Transient)]
	[TestCase(HttpStatusCode.BadGateway, FeedFailureKind.Transient)]
	[TestCase(HttpStatusCode.BadRequest, FeedFailureKind.Unknown)]
	[TestCase(HttpStatusCode.NotFound, FeedFailureKind.Unknown)]
	public void ClassifiesHttpFailuresStrictly(HttpStatusCode statusCode, FeedFailureKind expected)
		=> Assert.That(FeedFailureClassifier.Classify(statusCode), Is.EqualTo(expected));

	[Test]
	public void FindsStatusInInnerException()
	{
		var exception = new InvalidOperationException(
			"wrapper",
			new HttpRequestException("rate limited", null, HttpStatusCode.TooManyRequests));

		Assert.That(FeedFailureClassifier.Classify(exception), Is.EqualTo(FeedFailureKind.Transient));
	}
}

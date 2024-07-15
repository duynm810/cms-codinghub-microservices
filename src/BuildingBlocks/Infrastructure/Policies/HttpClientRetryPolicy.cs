using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;
using Serilog;

namespace Infrastructure.Policies;

public static class HttpClientRetryPolicy
{
    /// <summary>
    /// Configures the HttpClient with an immediate retry policy.
    /// The policy will retry the HTTP request immediately upon encountering a transient error.
    /// </summary>
    /// <param name="builder">The IHttpClientBuilder instance to which the policy will be added.</param>
    /// <param name="retryCount">The maximum number of retry attempts. Default is 3.</param>
    /// <returns>The IHttpClientBuilder instance with the immediate retry policy applied.</returns>
    public static IHttpClientBuilder UseImmediateHttpRetryPolicy(this IHttpClientBuilder builder, int retryCount = 3)
    {
        return builder.AddPolicyHandler(ConfigureImmediateHttpRetry(retryCount));
    }
    
    /// <summary>
    /// Configures the HttpClient with a linear retry policy.
    /// The policy will retry the HTTP request with a fixed delay between each retry attempt upon encountering a transient error.
    /// </summary>
    /// <param name="builder">The IHttpClientBuilder instance to which the policy will be added.</param>
    /// <param name="retryCount">The maximum number of retry attempts. Default is 3.</param>
    /// <returns>The IHttpClientBuilder instance with the linear retry policy applied.</returns>
    public static IHttpClientBuilder UseLinearHttpRetryPolicy(this IHttpClientBuilder builder, int retryCount = 3)
    {
        return builder.AddPolicyHandler(ConfigureLinearHttpRetry(retryCount));
    }

    /// <summary>
    /// Configures the HttpClient with an exponential retry policy.
    /// The policy will retry the HTTP request with an exponentially increasing delay between each retry attempt upon encountering a transient error.
    /// </summary>
    /// <param name="builder">The IHttpClientBuilder instance to which the policy will be added.</param>
    /// <param name="retryCount">The maximum number of retry attempts. Default is 5.</param>
    /// <returns>The IHttpClientBuilder instance with the exponential retry policy applied.</returns>
    public static IHttpClientBuilder UseExponentialHttpRetryPolicy(this IHttpClientBuilder builder, int retryCount = 5)
    {
        return builder.AddPolicyHandler(ConfigureExponentialHttpRetry(retryCount));
    }

    /// <summary>
    /// Configures the HttpClient with a Circuit Breaker policy to handle transient HTTP errors.
    /// The Circuit Breaker policy will break the circuit after a specified number of consecutive failures
    /// and will stay broken for a specified duration before allowing attempts to pass through again.
    /// </summary>
    /// <param name="builder">The IHttpClientBuilder instance to which the policy will be added.</param>
    /// <param name="eventsAllowedBeforeBreaking">The number of consecutive events (failures) that will cause the circuit to break. Default is 3.</param>
    /// <param name="fromSeconds">The duration (in seconds) that the circuit will stay open before it allows attempts again. Default is 30 seconds.</param>
    /// <returns>The IHttpClientBuilder instance with the Circuit Breaker policy applied.</returns>
    public static IHttpClientBuilder UseCircuitBreakerPolicy(this IHttpClientBuilder builder, int eventsAllowedBeforeBreaking = 3, int fromSeconds = 30)
    {
        return builder.AddPolicyHandler(ConfigureCircuitBreakerPolicy(eventsAllowedBeforeBreaking, fromSeconds));
    }

    /// <summary>
    /// Configures the HttpClient with a timeout policy.
    /// The policy sets a maximum time that an HTTP request can take before it is canceled.
    /// </summary>
    /// <param name="builder">The IHttpClientBuilder instance to which the policy will be added.</param>
    /// <param name="seconds">The maximum time (in seconds) an HTTP request can take before it is canceled. Default is 5 seconds.</param>
    /// <returns>The IHttpClientBuilder instance with the timeout policy applied.</returns>
    public static IHttpClientBuilder ConfigureTimeoutPolicy(this IHttpClientBuilder builder, int seconds = 5)
    {
        return builder.AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(seconds));
    }

    private static IAsyncPolicy<HttpResponseMessage> ConfigureCircuitBreakerPolicy(int eventsAllowedBeforeBreaking, int fromSeconds)
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(
                eventsAllowedBeforeBreaking,
                TimeSpan.FromSeconds(fromSeconds)
            );
    }

    private static IAsyncPolicy<HttpResponseMessage> ConfigureImmediateHttpRetry(int retryCount)
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .Or<TimeoutRejectedException>()
            .RetryAsync(retryCount, (exception, retryAttemptCount, context) =>
            {
                Log.Error($"Retry {retryAttemptCount} of {context.PolicyKey} at " +
                          $"{context.OperationKey}, due to: {exception.Exception.Message}");
            });
    }

    private static IAsyncPolicy<HttpResponseMessage> ConfigureLinearHttpRetry(int retryCount)
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .Or<TimeoutRejectedException>()
            .WaitAndRetryAsync(retryCount, retryAttempt => TimeSpan.FromSeconds(3),
                (exception, retryAttemptCount, context) =>
                {
                    Log.Error($"Retry {retryAttemptCount} of {context.PolicyKey} at " +
                              $"{context.OperationKey}, due to: {exception.Exception.Message}");
                });
    }

    private static IAsyncPolicy<HttpResponseMessage> ConfigureExponentialHttpRetry(int retryCount)
    {
        // In this case will wait for
        //  2 ^ 1 = 2 seconds then
        //  2 ^ 2 = 4 seconds then
        //  2 ^ 3 = 8 seconds then
        //  2 ^ 4 = 16 seconds then
        //  2 ^ 5 = 32 seconds
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .Or<TimeoutRejectedException>()
            .WaitAndRetryAsync(retryCount, retryAttempt
                    => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (exception, retryAttemptCount, context) =>
                {
                    Log.Error($"Retry {retryAttemptCount} of {context.PolicyKey} at " +
                              $"{context.OperationKey}, due to: {exception.Exception.Message}");
                });
    }
}
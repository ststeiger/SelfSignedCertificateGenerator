
using Microsoft.AspNetCore.Http; // for WriteAsync 
using Microsoft.Extensions.Logging; // for LogDebug, LogInformation 


namespace TestApplicationHttps
{


    public class MatchingChallenge
    {
        public string Token = "token.txt";
        public string Response = "someText";
    }


    public class LetsEncryptChallengeApprovalMiddleware 
    {

        private const string MagicPrefix = "/.well-known/acme-challenge";
        private static readonly Microsoft.AspNetCore.Http.PathString MagicPrefixSegments = 
            new Microsoft.AspNetCore.Http.PathString(MagicPrefix);

        private readonly Microsoft.AspNetCore.Http.RequestDelegate _next;
        private readonly Microsoft.Extensions.Logging.ILogger<LetsEncryptChallengeApprovalMiddleware> _logger;
        

        public LetsEncryptChallengeApprovalMiddleware(
             Microsoft.AspNetCore.Http.RequestDelegate next
            , Microsoft.Extensions.Logging.ILogger<LetsEncryptChallengeApprovalMiddleware> logger
            // ,IPersistenceService persistenceService
            )
        {
            _next = next;
            _logger = logger;
            // _persistenceService = persistenceService;
        }

        public async System.Threading.Tasks.Task InvokeAsync(Microsoft.AspNetCore.Http.HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments(MagicPrefixSegments))
            {
                try
                {
                    await ProcessAcmeChallenge(context);
                }
                catch (System.Exception ex)
                {
                    context.Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "text/plain";

                    await context.Response.WriteAsync(
                            text: ex.Message,
                            cancellationToken: context.RequestAborted
                        );

                    await context.Response.WriteAsync(
                            text: System.Environment.NewLine + System.Environment.NewLine,
                            cancellationToken: context.RequestAborted
                        );

                    await context.Response.WriteAsync(
                            text: ex.StackTrace,
                            cancellationToken: context.RequestAborted
                        );

                    await context.Response.CompleteAsync();
                }
                
            }
            else
                await _next(context);
        } // End Task InvokeAsync 


        private async System.Threading.Tasks.Task ProcessAcmeChallenge(Microsoft.AspNetCore.Http.HttpContext context)
        {
            string path = context.Request.Path.ToString();
            _logger.LogDebug("Challenge invoked: {challengePath} by {IpAddress}", path, context.Connection.RemoteIpAddress);

            // var allChallenges = await _persistenceService.GetPersistedChallengesAsync();
            MatchingChallenge[] allChallenges = new MatchingChallenge[] { new MatchingChallenge() };
            MatchingChallenge matchingChallenge = null;

            if (path.Length > MagicPrefix.Length)
            {
                string requestedToken = path.Substring($"{MagicPrefix}/".Length);

                for (int i = 0; i < allChallenges.Length; ++i)
                {
                    if (string.Equals(allChallenges[i].Token, requestedToken, System.StringComparison.Ordinal))
                    {
                        matchingChallenge = allChallenges[i];
                        break;
                    } // End if (string.Equals(allChallenges[i].Token, requestedToken, System.StringComparison.Ordinal)) 

                } // Next i 

            } // End if (path.Length > MagicPrefix.Length) 

            if (matchingChallenge == null)
            {
                _logger.LogInformation("The given challenge did not match {challengePath} among {allChallenges}", path, allChallenges);

                context.Response.StatusCode = (int)System.Net.HttpStatusCode.PreconditionFailed;
                context.Response.ContentType = "text/plain";
                await context.Response.WriteAsync(
                    text: "d'OH.",
                    cancellationToken: context.RequestAborted
                );

                await context.Response.CompleteAsync();
                return;
            } // End if (matchingChallenge == null) 

            // token response is always in ASCII so char count would be equal to byte count here
            context.Response.StatusCode = (int) System.Net.HttpStatusCode.OK;
            context.Response.ContentLength = matchingChallenge.Response.Length;
            // context.Response.ContentType = "application/octet-stream";
            context.Response.ContentType = "text/plain; charset=ascii";
            await context.Response.WriteAsync(
                text: matchingChallenge.Response,
                cancellationToken: context.RequestAborted);
        } // End Task ProcessAcmeChallenge 


    } // End Class LetsEncryptChallengeApprovalMiddleware 


} // End Namespace TestApplicationHttps 

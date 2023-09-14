using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Profisee.WebhookTemplate.WebApp.Services
{
    /// <summary>
    /// Overrides the default ASP.NET Core authorization service so that more information can be
    /// logged on fail authorization attempts
    /// </summary>
    internal class ProfiseeAuthorizationService : DefaultAuthorizationService, IAuthorizationService
    {
        private readonly AuthorizationOptions options;
        private readonly IAuthorizationHandlerContextFactory contextFactory;
        private readonly IAuthorizationHandlerProvider handlers;
        private readonly IAuthorizationEvaluator evaluator;
        private readonly ILogger logger;

        public ProfiseeAuthorizationService(IAuthorizationPolicyProvider policyProvider,
            IAuthorizationHandlerProvider handlers,
            ILogger<DefaultAuthorizationService> logger,
            IAuthorizationHandlerContextFactory contextFactory,
            IAuthorizationEvaluator evaluator,
            IOptions<AuthorizationOptions> options)
            : base(policyProvider, handlers, logger, contextFactory, evaluator, options)
        {
            this.options = options.Value;
            this.handlers = handlers;
            this.logger = logger;
            this.evaluator = evaluator;
            this.contextFactory = contextFactory;
        }

        public new async Task<AuthorizationResult> AuthorizeAsync(
            ClaimsPrincipal user,
            object resource,
            IEnumerable<IAuthorizationRequirement> requirements)
        {
            if (requirements == null)
            {
                throw new ArgumentNullException(nameof(requirements));
            }

            var authContext = contextFactory.CreateContext(requirements, user, resource);
            var handlers = await this.handlers.GetHandlersAsync(authContext);

            foreach (var handler in handlers)
            {
                await handler.HandleAsync(authContext);

                if (!options.InvokeHandlersAfterFailure && authContext.HasFailed)
                {
                    break;
                }
            }

            var authResult = evaluator.Evaluate(authContext);
            if (authResult.Succeeded)
            {
                logger.LogInformation($"Authorization has succeeded for {JsonConvert.SerializeObject(requirements, Formatting.Indented)}");
            }
            else
            {
                var builder = new StringBuilder();
                builder.AppendLine($"Authorization failed for: ");
                foreach (var failedRequirement in authResult.Failure.FailedRequirements)
                {
                    builder.AppendLine(JsonConvert.SerializeObject(failedRequirement, Formatting.Indented));
                }
                logger.LogError(builder.ToString());
            }

            return authResult;
        }
    }
}
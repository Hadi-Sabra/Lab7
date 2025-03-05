using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Threading.Tasks;

namespace Lab7.Authorization
{
    
    public class BranchAuthorizationHandler : IAuthorizationHandler
    {
        public Task HandleAsync(AuthorizationHandlerContext context)
        {
            // Checking if the user has the required "BranchId" claim
            var branchIdClaim = context.User?.Claims?.FirstOrDefault(c => c.Type == "BranchId")?.Value;

            // If the user has the claim and it matches the required branch for the resource
            if (branchIdClaim != null && context.Resource is string requiredBranch && branchIdClaim == requiredBranch)
            {
                context.Succeed(requirement: new BranchRequirement(requiredBranch));
            }

            return Task.CompletedTask;
        }
    }

    
    public class BranchRequirement : IAuthorizationRequirement
    {
        public string BranchId { get; }
        public BranchRequirement(string branchId)
        {
            BranchId = branchId;
        }
    }
}
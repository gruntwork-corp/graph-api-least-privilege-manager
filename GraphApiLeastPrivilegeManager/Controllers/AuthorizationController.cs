#if DEBUG
using System.Security.Claims;
using GraphApiLeastPrivilegeManager.Models.Enums;
using GraphApiLeastPrivilegeManager.Models.Groups;
using GraphApiLeastPrivilegeManager.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GraphApiLeastPrivilegeManager.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthorizationController : Controller
{
    private readonly ILogger<AuthorizationController> _logger;

    public AuthorizationController(ILogger<AuthorizationController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    [Authorize(Roles = "Group.Read,Group.ReadWriteControl.OwnedBy,Group.ReadWrite.UserPrincipals,Group.ReadWrite.ServicePrincipals,Group.ReadWrite.OwnedBy")]
    [Route("All")]
    public ActionResult<AzurePrincipal> GetAll()
    {
        var principal = PrincipalUtils.ExtractPrincipal(User);
        return principal is not null? Ok(principal) : StatusCode(412);
    }
    
    [HttpGet]
    [Authorize(Roles = "Group.ReadWrite.OwnedBy")]
    [Route("GroupReadWriteOwnedBy")]
    public ActionResult<AzurePrincipal> GetGroupReadWriteOwnedBy()
    {
        var principal = PrincipalUtils.ExtractPrincipal(User);
        return principal is not null? Ok(principal) : StatusCode(412);
    }
}
#endif
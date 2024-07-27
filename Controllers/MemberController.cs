using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using netAPI.Models;

namespace netAPI.Controllers;

[ApiController]
public class MemberController : ControllerBase
{
    private AppDbContext dbContext;

    public MemberController(AppDbContext appDbContext)
    {
        dbContext = appDbContext;
    }

    [HttpPost]
    [Route("Member/Login")]
    public async Task<Object> MemberLogin(MemberLoginViewModel model)
    {
        var member = dbContext.Member.Where(a => a.Email == model.Email).FirstOrDefault();

        if(member != null)
        {
            return new {
                status = 1,
                message = "Login Successful",
                Member = new {
                    member.Id,
                    member.Email,
                    member.Name
                }
                
            };
        }
        return new {
                status = 0,
                message = "Invalid User"
            };
    }
}
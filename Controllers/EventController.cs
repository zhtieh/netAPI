using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using netAPI.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace netAPI.Controllers;

[ApiController]
public class EventController : ControllerBase
{
    private AppDbContext dbContext;

    public EventController(AppDbContext appDbContext)
    {
        dbContext = appDbContext;
    }

    [HttpGet]
    [Route("Event/GetEvents")]
    public async Task<IEnumerable<Object>> GetEvents()
    {
        return dbContext.DonationList.Select(x => new {
            EventId = x.event_id,
            title = x.name,
            goal = x.total_amount,
            imageSrc = x.ImageUrl,
            amount = dbContext.MemberDonation.Where(a => a.EventId == x.event_id).Select(y => y.Amount).ToList().AsQueryable().Sum(),
            //Percentage = Math.Round(dbContext.MemberDonation.Where(a => a.EventId == x.event_id).Select(y => y.Amount).ToList().AsQueryable().Sum() / x.total_amount * 100, 2),
        });
    }
}
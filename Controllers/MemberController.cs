using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using netAPI.Models;
using RestSharp;

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
    [HttpGet]
    [Route("Member/GetProfile/{Id}")]
    public async Task<Object> GetProfileDetails(int Id)
    {
        var walletAddress = dbContext.MemberWallet.Where(x => x.UserId == Id).Select(a => a.WalletAddress).FirstOrDefault();
        var options = new RestClientOptions("https://service.maschain.com") 
            { 
                ThrowOnAnyError = true, 
            }; 

            var client = new RestClient(options); 

            var request = new RestRequest("api/token/balance", Method.Post); 
            request.AddHeader("client_id", "0c4ea9c297c4351887d6cb92520f9f09198bcd03b5e141a126a73e9a021ed061"); 
            request.AddHeader("client_secret", "sk_f2146fd8c39157338bdb66f749e03b2fb5a734b56189db966aaff82de6968101"); 
            request.AddHeader("Content-Type", "application/json"); 

            var body = new 
            { 
                wallet_address = walletAddress,
                contract_address = "0xFED52beB05e61867515fDb663276E9E458106c29"
            }; 

            request.AddJsonBody(body); 
 

            RestResponse response = await client.ExecuteAsync(request); 
            Decimal token = new decimal(0.00);
 
            if (response.IsSuccessful) 
            { 
                Console.WriteLine(response.Content); 
                JsonDocument doc = JsonDocument.Parse(response.Content);

                // Get the root element
                
                JsonElement root = doc.RootElement;
                token = Decimal.Parse(root.GetProperty("result").GetString());
            } 
            else 
            { 
                Console.WriteLine($"Error: {response.StatusCode} - {response.Content}");
            }
        return new
        {
            UserId = Id,
            TotalToken = token,
            Transactions = dbContext.MemberDonation.Where(x => x.UserId == Id).Select(a => new 
            {
                WalletAddress = walletAddress,
                Event = dbContext.DonationList.Where(b => b.event_id == a.EventId).Select(c => c.name).FirstOrDefault(),
                Amount = a.Amount,
                FinalAmount = a.AmountAfterDeduction,
                Date = a.DonationDate
            }).OrderByDescending(k => k.Date)
        };
    }
}
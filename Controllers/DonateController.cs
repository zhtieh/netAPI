using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using netAPI.Models;

namespace netAPI.Controllers;

[ApiController]
public class DonateController : ControllerBase
{
    private AppDbContext dbContext;
    private readonly HttpClient _httpClient;

    public DonateController(AppDbContext appDbContext, HttpClient httpClient)
    {
        dbContext = appDbContext;
        _httpClient = httpClient;
    }

    [HttpPost]
    [Route("Donate/MemberDonation")]
    public async Task<Object> MemberDonation(MemberDonationViewModel model)
    {
        var member = dbContext.Member.Where(a => a.Id == model.UserId).FirstOrDefault();

        if(member != null)
        {
            var mb = new MemberDonation
            {
                UserId = model.UserId,
                EventId = model.EventId,
                Amount = model.Amount,
                DonationDate = DateTime.UtcNow
            };

            dbContext.MemberDonation.Add(mb);
            dbContext.SaveChanges();

            var walletAddress = dbContext.MemberWallet.Where(a => a.UserId == model.UserId).Select(x => x.WalletAddress).FirstOrDefault();

            MintToken(model.Amount, walletAddress);

            return new {
                status = 1,
                message = "Successfully Donated RM " + model.Amount
            };
        }

        return new {
                status = 0,
                message = "Invalid User"
            };
    }

    private async void MintToken(Decimal amount, string walletAddress)
    {
        var requestModel = new
        {
            wallet_address = "0x7Bce030BeFAa4Feb0c16808Be9E8b273d47CdD58",
            to = walletAddress,
            amount = amount.ToString(),
            contract_address = "0xFED52beB05e61867515fDb663276E9E458106c29",
            callback_url = "https://postman-echo.com/post?",
            signed_data = ""
        };

        var json = JsonSerializer.Serialize(requestModel);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var request = new HttpRequestMessage(HttpMethod.Post, "https://service.maschain.com/api/token/mint")
        {
            Content = content
        };

        request.Headers.Add("client_id","0c4ea9c297c4351887d6cb92520f9f09198bcd03b5e141a126a73e9a021ed061");
        request.Headers.Add("client_secret","sk_f2146fd8c39157338bdb66f749e03b2fb5a734b56189db966aaff82de6968101");

        var response = await _httpClient.SendAsync(request);

        response.EnsureSuccessStatusCode();

//         if (response.IsSuccessStatusCode)
//         {
//             // if the call was successful, read the response content
//             var content = await response.Content.ReadAsStringAsync();
// xs
//             // do something with the response content
//             // ...
//             Console.WriteLine(content);
//         }

        Console.WriteLine(response);
    }
}
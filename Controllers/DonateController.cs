using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using netAPI.Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Diagnostics;
using Jint;
using RestSharp;
using Microsoft.Extensions.ObjectPool;


namespace netAPI.Controllers;

[ApiController]
public class DonateController : ControllerBase
{
    private AppDbContext dbContext;
    private readonly IHttpClientFactory _clientFactory;

    public DonateController(AppDbContext appDbContext, IHttpClientFactory clientFactory)
    {
        dbContext = appDbContext;
        _clientFactory = clientFactory;
    }

    [HttpPost]
    [Route("Donate/MemberDonation")]
    public async Task<Object> MemberDonation(MemberDonationViewModel model)
    {
        var member = dbContext.Member.Where(a => a.Id == model.UserId).FirstOrDefault();
        var deduction = Math.Round(model.Amount * (decimal) 0.1,2);

        if(member != null)
        {
            var mb = new MemberDonation
            {
                UserId = model.UserId,
                EventId = model.EventId,
                Amount = model.Amount,
                DonationDate = DateTime.UtcNow,
                AmountAfterDeduction = model.Amount - deduction
            };

            dbContext.MemberDonation.Add(mb);

            var walletAddress = dbContext.MemberWallet.Where(a => a.UserId == model.UserId).Select(x => x.WalletAddress).FirstOrDefault();
            dbContext.Income.Add(new Income
            {
                UserId = model.UserId,
                EventId = model.EventId,
                Amount = deduction,
            });

            var txnHash = await MintToken(model.Amount, walletAddress);
            SetTransferRecord(walletAddress, txnHash, model.UserId, model.Amount);
            dbContext.SaveChanges();

            return new {
                status = 1,
                message = "Successfully Donated RM " + model.Amount,
                DonatedAmount = new {
                    InitialAmount = model.Amount,
                    Deduction = deduction,
                    FinalAmount = model.Amount - deduction
                },
                Event = dbContext.DonationList.Where(a => a.event_id == model.EventId).Select(x => new{
                    EventId = x.event_id,
                    title = x.name,
                    goal = x.total_amount,
                    imageSrc = x.ImageUrl,
                    amount = dbContext.MemberDonation.Where(a => a.EventId == x.event_id).Select(y => y.Amount).ToList().AsQueryable().Sum(),
                }).FirstOrDefault()
            };
        }

        return new {
                status = 0,
                message = "Invalid User"
            };
    }

    [HttpPost]
    [Route("Donate/Callback")]
    public async void TransactionCallback(TransactionCallbackViewModel model)
    {
        if(model.result.status == "success" && model.status == 200)
        {
            var txn = dbContext.TokenTransaction.Where(x => x.TxnHash == model.result.transactionHash).Select(x => x).FirstOrDefault();
            if(txn != null)
            {
                txn.Status = "S";
                dbContext.SaveChanges();
            }
        }
        else
        {
            var txn = dbContext.TokenTransaction.Where(x => x.TxnHash == model.result.transactionHash).Select(x => x).FirstOrDefault();
            if(txn != null)
            {
                txn.Status = "F";
                dbContext.SaveChanges();
            }
        }
    }
    private async void SetTransferRecord(string walletAddress, string txnHash, int UserId, decimal Amount)
    {
        dbContext.TokenTransaction.Add(new TokenTransaction
        {
            UserId = UserId,
            WalletAddress = walletAddress,
            Amount = Amount,
            Status = "P",
            TxnHash = txnHash
        });
    }

    private async Task<string> MintToken(Decimal Amount, string walletAddress)
    {
        try{
            var options = new RestClientOptions("https://service.maschain.com") 
            { 
                ThrowOnAnyError = true, 
            }; 

            var client = new RestClient(options); 

            var request = new RestRequest("api/token/mint", Method.Post); 
            request.AddHeader("client_id", "0c4ea9c297c4351887d6cb92520f9f09198bcd03b5e141a126a73e9a021ed061"); 
            request.AddHeader("client_secret", "sk_f2146fd8c39157338bdb66f749e03b2fb5a734b56189db966aaff82de6968101"); 
            request.AddHeader("Content-Type", "application/json"); 

            var body = new 
            { 
                wallet_address = "0x7Bce030BeFAa4Feb0c16808Be9E8b273d47CdD58", 
                to = walletAddress, 
                amount = Amount, 
                contract_address = "0xFED52beB05e61867515fDb663276E9E458106c29", 
                callback_url = "http://subnetapi.runasp.net/Donate/Callback", 
                signed_data = "" 
            }; 

            request.AddJsonBody(body); 
 

            RestResponse response = await client.ExecuteAsync(request); 
 
            if (response.IsSuccessful) 
            { 
                Console.WriteLine(response.Content); 
                JsonDocument doc = JsonDocument.Parse(response.Content);

                // Get the root element
                
                JsonElement root = doc.RootElement;
                JsonElement res = root.GetProperty("result");
                return res.GetProperty("transactionHash").GetString();
            } 
            else 
            { 
                Console.WriteLine($"Error: {response.StatusCode} - {response.Content}");
            } 
            return null; 
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex);
            return null;
        }
    }
}
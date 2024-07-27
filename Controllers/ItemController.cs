using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using netAPI.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace netAPI.Controllers;

[ApiController]
public class ItemController : ControllerBase
{
    private AppDbContext dbContext;

    public ItemController(AppDbContext appDbContext)
    {
        dbContext = appDbContext;
    }

    [HttpGet]
    [Route("Item/GetItems")]
    public async Task<IEnumerable<Item>> GetItemsAsync()
    {
        return dbContext.Item;
    }

    [HttpPost]
    [Route("Item/AddItem")]
    public async Task<bool> AddItem(AddItemViewModel model)
    {
        var item = new Item
        {
            Id = model.Id,
            Name = model.Name,
            Status = model.Status
        };

        dbContext.Item.Add(item);
        dbContext.SaveChanges();

        return true;
    }

    [HttpGet]
    [Route("Item/TestAPI")]
    public async Task<IEnumerable<Employee>> TestAPI()
    {
        var httpClient = new HttpClient();
        var response = await httpClient.GetAsync("https://dummy.restapiexample.com/api/v1/employees");

        string apiResponse = await response.Content.ReadAsStringAsync();
        JsonDocument doc = JsonDocument.Parse(apiResponse);
        JsonElement root = doc.RootElement;
        var data = root.GetProperty("data");

        JObject json = JObject.Parse(apiResponse);

        var Employees = new List<Employee>();
        Employees = JsonConvert.DeserializeObject<List<Employee>>(data.ToString());
        return Employees;
    }
}
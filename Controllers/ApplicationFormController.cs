using Microsoft.AspNetCore.Mvc;
using netAPI.Models;

namespace netAPI.Controllers;

[ApiController]
public class ApplicationFormController : ControllerBase
{
    private AppDbContext dbContext;

    public ApplicationFormController(AppDbContext appDbContext)
    {
        dbContext = appDbContext;
    }

    [HttpPost]
    [Route("ApplicationForm/Create")]
    public async Task<bool> CreateApplicationForm(CreateApplicationFormViewModel model)
    {
        dbContext.ApplicationForm.Add(new ApplicationForm
        {
            Name = model.Name,
            IC = model.IC,
            DOB = model.DOB, 
            Email = model.Email, 
            PhoneNumber = model.PhoneNumber, 
            HomeAddress = model.HomeAddress, 
            NameOfInstitution = model.NameOfInstitution, 
            YearOfStudy = model.YearOfStudy, 
            FieldOfStudy = model.FieldOfStudy, 
            StudentID = model.StudentID, 
            FundingType = model.FundingType, 
            FundingDescription = model.FundingDescription, 
            ReceiveFundingBefore = model.ReceiveFundingBefore 
        });
        dbContext.SaveChanges();
        return true;
    }
}
using CRUDExample.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using ServiceContracts.DTO;

namespace CRUDProject.Filters.ActionFilters
{
    public class PersonsListActionFilter : IAsyncActionFilter
    {
        private readonly ILogger<PersonsListActionFilter> _logger;

        public PersonsListActionFilter(ILogger<PersonsListActionFilter> logger)
        {
            _logger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            _logger.LogInformation("{FilterName}.{MethodName} before", nameof(PersonsListActionFilter), nameof(OnActionExecutionAsync));

            if (context.ActionArguments.ContainsKey("searchBy"))
            {
                List<string> searchables = new()
                {
                    nameof(PersonResponse.PersonName),
                    nameof(PersonResponse.Email),
                    nameof(PersonResponse.DateOfBirth),
                    nameof(PersonResponse.CountryID),
                    nameof(PersonResponse.Gender),
                    nameof(PersonResponse.Address)
                };

                if (!searchables.Contains(context.ActionArguments["searchBy"]))
                {
                    _logger.LogInformation("Invalid searchBy value: {searchBy}", context.ActionArguments["searchBy"]?.ToString());

                    context.ActionArguments["searchBy"] = nameof(PersonResponse.PersonName);

                    _logger.LogInformation("Default searchBy value is set to: {searchBy}", context.ActionArguments["searchBy"]!.ToString());
                }
                else
                {
                    _logger.LogInformation("Valid {searchBy} value", context.ActionArguments["searchBy"]!.ToString());
                }
            }

            context.HttpContext.Items["actionArgs"] = context.ActionArguments;

            await next();

            _logger.LogInformation("{FilterName}.{MethodName} after", nameof(PersonsListActionFilter), nameof(OnActionExecutionAsync));

            PersonsController personsController = (PersonsController)context.Controller;

            if (context.HttpContext.Items["actionArgs"] is Dictionary<string, object> actionArgs)
            {
                if (actionArgs.ContainsKey("searchBy"))
                {
                    personsController.ViewBag.CurrentSearchBy = actionArgs["searchBy"]?.ToString();
                }

                if (actionArgs.ContainsKey("searchString"))
                {
                    personsController.ViewBag.CurrentSearchString = actionArgs["searchString"]?.ToString();
                }

                if (actionArgs.ContainsKey("sortBy"))
                {
                    personsController.ViewBag.CurrentSortBy = actionArgs["sortBy"]?.ToString();
                }
                else
                { 
                    personsController.ViewBag.CurrentSortBy = nameof(PersonResponse.PersonName);
                }

                if (actionArgs.ContainsKey("sortOrder"))
                {
                    personsController.ViewBag.CurrentSortOrder = actionArgs["sortOrder"]?.ToString();
                }
                else
                { 
                    personsController.ViewBag.CurrentSortOrder = "ASC";
                }
            }

            personsController.ViewBag.SearchFields = new Dictionary<string, string>()
              {
                { nameof(PersonResponse.PersonName), "Person Name" },
                { nameof(PersonResponse.Email), "Email" },
                { nameof(PersonResponse.DateOfBirth), "Date of Birth" },
                { nameof(PersonResponse.Gender), "Gender" },
                { nameof(PersonResponse.CountryID), "Country" },
                { nameof(PersonResponse.Address), "Address" }
              };
        }
    }
}

using CS.Mediator.Contract;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Sample.WebApi;

public static class ValidationResultExtensions
{
    public static ModelStateDictionary ToModelStateDictionary(this ValidationResults results)
    {
        var modelStateDictionary = new ModelStateDictionary();
        foreach (var result in results)
        {
            modelStateDictionary.AddModelError(result.Key, result.Message);
        }

        return modelStateDictionary;
    }
}
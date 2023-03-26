using Mediator.Contract;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Sample.WebApi;

public static class ProcessingResultExtensions
{
    public static ModelStateDictionary ToModelStateDictionary(this ProcessingResults results)
    {
        var modelStateDictionary = new ModelStateDictionary();
        foreach (var result in results)
        {
            modelStateDictionary.AddModelError(result.Key, result.Message);
        }

        return modelStateDictionary;
    }
}
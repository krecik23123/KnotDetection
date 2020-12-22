using KnotDetectorFunctions;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.WindowsAzure.Storage;
using System;

[assembly: FunctionsStartup(typeof(Startup))]
namespace KnotDetectorFunctions
{
    class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage", EnvironmentVariableTarget.Process);

            var storageAccount = CloudStorageAccount.Parse(connectionString);

            var client = storageAccount.CreateCloudBlobClient();

            var container = client.GetContainerReference("models");

            var model = container.GetBlockBlobReference("MLModel.zip");

            var uri = model.Uri.AbsoluteUri;

            builder.Services.AddPredictionEnginePool<ModelInput, ModelOutput>().FromUri(uri);
        }
    }
}

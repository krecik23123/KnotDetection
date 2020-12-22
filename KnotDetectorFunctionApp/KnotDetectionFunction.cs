using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Extensions.ML;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using System;
using Microsoft.WindowsAzure.Storage;
using KnotDetectorFunctionApp;

namespace KnotDetectionFunctions
{
    public class KnotDetectionFunction
    {
        private readonly PredictionEnginePool<ModelInput, ModelOutput> _predictionEnginePool;

        public KnotDetectionFunction(PredictionEnginePool<ModelInput, ModelOutput> predictionEnginePool)
        {
            _predictionEnginePool = predictionEnginePool;
        }

        [FunctionName("KnotDetectionFunction")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log, ExecutionContext context)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            
            var input = JsonConvert.DeserializeObject<ModelInput>(requestBody);

            var fileName = input.ImageSource;

            var engine = _predictionEnginePool.GetPredictionEngine();

            var connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage", EnvironmentVariableTarget.Process);

            var storageAccount = CloudStorageAccount.Parse(connectionString);

            var client = storageAccount.CreateCloudBlobClient();

            var container = client.GetContainerReference("knots");

            var blockBlob = container.GetBlobReference(fileName);

            var savePath = Path.Combine(Path.GetTempPath(), fileName);
            var fileStream = new FileStream(savePath, FileMode.Create);

            await blockBlob.DownloadToStreamAsync(fileStream);

            fileStream.Close();

            var res = engine.Predict(new ModelInput() { ImageSource = savePath });

            string modelOutput = JsonConvert.SerializeObject(res);

            return new OkObjectResult(modelOutput);
        }
    }
}

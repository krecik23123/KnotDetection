using KnotDetectorML.Model;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace KnotDetectionLogic
{
    public class KnotDetectorService
    {
        private static readonly HttpClient client = new HttpClient();

        public async Task<string> DetectKnotAsync(string fileName)
        {
            var modelInput = new ModelInput() { ImageSource = fileName };
            string modelInputJson = JsonConvert.SerializeObject(modelInput);

            var response = await client.PostAsync(
                "https://knotdetectionfunctions20201213122414.azurewebsites.net/api/KnotDetectionFunction?code=PoT4ZsZpmRD1DH5JJWATcrr6Uq/2h/1kWJ0Oh5n1vXMaqDrLUU/cGA==",
                 new StringContent(modelInputJson, Encoding.UTF8, "application/json"));

            var modelOutput = JsonConvert.DeserializeObject<ModelOutput>(await response.Content.ReadAsStringAsync());

            string message = modelOutput.Prediction == "seki" ? "Knot Detected." : "Knot not found.";

            return message;
        }
    }
}

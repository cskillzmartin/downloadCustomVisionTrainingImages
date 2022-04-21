using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Web;

namespace Download_CustomVision_TrainingImages
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var trainingKey = "";//change this to your training key
            var iterationId = "";//iteration id guid https://docs.microsoft.com/en-us/rest/api/customvision/training3.3/get-iterations/get-iterations#code-try-0
            var projectId = "";//project id guid
            var endPoint = "";//endpoint name
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Training-key", trainingKey);
                var skipp = 0; //leave this at 0
                var TotalImages = 0; //change this to the number of images in your training set
                while (skipp < TotalImages)
                {
                    var uri = $"https://{endPoint}.cognitiveservices.azure.com/customvision/v3.0/training/projects/{projectId}/images/tagged?iterationId={iterationId}&take=256&skip={skipp}";
                    var response = client.GetAsync(uri);
                    var json = JsonConvert.DeserializeObject<Root[]>(response.Result.Content.ReadAsStringAsync().Result);
                    foreach (var item in json)
                    {
                        Console.WriteLine(item.originalImageUri);

                        using (var client2 = new HttpClient())
                        using (var request = new HttpRequestMessage())
                        {
                            request.Method = HttpMethod.Get;
                            request.RequestUri = new Uri($"{item.originalImageUri}");
                            request.Headers.Add("Training-key", trainingKey);
                            var response2 = client2.SendAsync(request).Result;
                            if (response2.IsSuccessStatusCode)
                            {
                                var image = response2.Content.ReadAsByteArrayAsync().Result;
                                var imageFile = File.Create($"D:\\Downloads\\TrainingImages\\{item.tags[0].tagName} {item.id}.jpg");
                                imageFile.Write(image, 0, image.Length);
                                imageFile.Close();
                            }
                        }
                    }
                    
                    skipp += 256; //advance to the next 256 images
                }
            }

            Console.ReadKey();
        }
    }

    public class Tag
    {
        public string tagId { get; set; }
        public string tagName { get; set; }
        public DateTime created { get; set; }
    }

    public class Root
    {
        public string id { get; set; }
        public DateTime created { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public string resizedImageUri { get; set; }
        public string thumbnailUri { get; set; }
        public string originalImageUri { get; set; }
        public List<Tag> tags { get; set; }
    }
}
}

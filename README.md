## Download Custom Vision Training Images

A small .NET 5 console app that downloads the original training images from a Microsoft Azure Custom Vision project. Custom Vision (part of Cognitive Services) does not provide a built-in export of your original training images, so this utility uses the Training API to retrieve them.

### What it does
- **Enumerates tagged images** for a given project iteration via the Custom Vision Training API.
- **Downloads the original image** for each item (`originalImageUri`).
- **Saves to disk** using the pattern: `"{firstTagName} {imageId}.jpg"`.
- **Paginates in batches of 256** using `take=256` and `skip` until all images are fetched.

### Prerequisites
- **.NET SDK**: .NET 5.0 or later
- **Custom Vision resources**: A project with training images
- **Training key**: From your Custom Vision Training resource
- **Endpoint (region subdomain)**: e.g., `eastus`, `westeurope`, etc.
- **Project ID (GUID)**: Your Custom Vision project ID
- **Iteration ID (GUID)**: The iteration to download images from
- **Writable output folder**: Update the output path in `Program.cs` or create the folder

### Getting the required values
- **Training key and endpoint**: In the Azure Portal, open your Custom Vision Training resource → Keys and Endpoint. The app uses: `https://{endpoint}.cognitiveservices.azure.com`.
- **Project ID**: In the Custom Vision web portal, open your project → Project Settings.
- **Iteration ID**: In the portal under Iterations (or Performance for a published iteration). You can also list them via the Training API (see Microsoft docs: "Get Iterations").

### Configure
Open `Program.cs` and set the following variables near the top of `Main`:
- `trainingKey` – your Training key
- `iterationId` – the iteration GUID
- `projectId` – the project GUID
- `endPoint` – your region subdomain (e.g., `eastus`)
- `TotalImages` – set to at least the number of tagged images in your training set; adding 256 ensures the final page is included
- Output path – update the hard-coded path in `File.Create(...)` to where you want files saved

Example output path line to change:
```csharp
var imageFile = File.Create($"D:\\Downloads\\TrainingImages\\{item.tags[0].tagName} {item.id}.jpg");
```

### Build and run
From the repository root:
```bash
dotnet restore
dotnet build
dotnet run --project Download_CustomVision_TrainingImages.csproj
```

### Notes and tips
- **Tagged images only**: The current endpoint fetches tagged images. To include untagged, change the API path accordingly.
- **Multiple tags**: The filename uses only the first tag (`item.tags[0]`). Adjust if you need different naming or per-tag folders.
- **Pagination size**: Uses `take=256`. Keep `skip` increments aligned with this size.
- **Auth/region errors**: 401 → check Training key; 404/403 → ensure endpoint region matches your resource and IDs are correct.
- **Do not commit secrets**: Keep your Training key out of source control.

### Why this exists
There is no built-in way in Custom Vision to retrieve original training images for a project iteration. I wrote this program to fill that gap and make it easy to export your dataset.


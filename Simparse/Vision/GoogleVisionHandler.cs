using Google.Cloud.Storage.V1;
using Google.Cloud.Vision.V1;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simparse.Vision
{
    public interface IGoogleVisionHandler
    {
        Task<VisionExtract> ImageRunner(Guid filekey, string fileExtension, string bucketName, int width, int height);
        Task<VisionExtract> PDFRunner(Guid filekey, string fileExtension, string bucketName);
    }
    public class GoogleVisionHandler : IGoogleVisionHandler
    {
        private string _projectId;

        public GoogleVisionHandler(string googleProjectId)
        {
            _projectId = googleProjectId;
        }

        public async Task<VisionExtract> ImageRunner(Guid filekey, string fileExtension, string bucketName, int width, int height)
        {
            string objectName = $"{filekey}{fileExtension}";
            var uri = new Uri($"gs://{bucketName}/{objectName}");

            var image = Google.Cloud.Vision.V1.Image.FromUri(uri);
            ImageAnnotatorClient client = ImageAnnotatorClient.Create();

            IReadOnlyList<EntityAnnotation> visionResult = await client.DetectTextAsync(image);

            VisionPageExtract page = new VisionPageExtract(1, width, height);

            foreach (EntityAnnotation annotation in visionResult.Skip(1))
            {
                page.Words.Add(new VisionWordExtract(annotation, width, height));
            }

            VisionExtract extractResponse = new VisionExtract(1);
            extractResponse.Pages.Add(page);

            return extractResponse;
        }

        public async Task<VisionExtract> PDFRunner(Guid filekey, string fileExtension, string bucketName)
        {
            string objectName = $"{filekey}{fileExtension}";
            string sourceBucketPath = $"gs://{bucketName}/{objectName}";
            string destination = $"gs://simparse-extract/";
            string sourceDestinationWithprefix = $"{destination}/{filekey}";
            var client = ImageAnnotatorClient.Create();
            var storageClient = StorageClient.Create();
            try
            {
                await storageClient.CreateBucketAsync(_projectId, "simparse-extract");
            }
            catch (Google.GoogleApiException e) when (e.Error.Code == 409) { }
            var asyncRequest = new AsyncAnnotateFileRequest
            {
                InputConfig = new InputConfig
                {
                    GcsSource = new GcsSource
                    {
                        Uri = sourceBucketPath,
                    },
                    MimeType = "application/pdf",
                },
                OutputConfig = new OutputConfig
                {
                    BatchSize = 2,
                    GcsDestination = new GcsDestination
                    {
                        Uri = sourceDestinationWithprefix
                    },
                }
            };
            asyncRequest.Features.Add(new Feature
            {
                Type = Feature.Types.Type.DocumentTextDetection
            });
            List<AsyncAnnotateFileRequest> requests = new List<AsyncAnnotateFileRequest>();
            requests.Add(asyncRequest);
            var operation = client.AsyncBatchAnnotateFiles(requests);
            operation.PollUntilCompleted();
            var blobList = storageClient.ListObjects("simparse-extract", filekey.ToString());
            var output = blobList.Where(x => x.Name.Contains(".json")).First();
            var jsonString = "";
            using (var stream = new MemoryStream())
            {
                storageClient.DownloadObject(output, stream);
                jsonString = Encoding.UTF8.GetString(stream.ToArray());
            }
            var response = JsonParser.Default.Parse<AnnotateFileResponse>(jsonString);
            int pageCount = response.Responses.LastOrDefault().Context.PageNumber;
            VisionExtract extractResponse = new VisionExtract(pageCount);
            foreach (var item in response.Responses)
            {
                foreach (var page in item.FullTextAnnotation.Pages)
                {
                    VisionPageExtract pageExt = new VisionPageExtract(item.Context.PageNumber, page.Width, page.Height);
                    foreach (var block in page.Blocks)
                    {
                        foreach (var para in block.Paragraphs)
                        {
                            foreach (var word in para.Words)
                            {
                                var wordData = new VisionWordExtract(word, page.Width, page.Height);

                                if (!string.IsNullOrWhiteSpace(wordData.Text))
                                {
                                    pageExt.Words.Add(wordData);
                                }
                            }
                        }

                    }
                    extractResponse.Pages.Add(pageExt);
                }
            }

            return extractResponse;
        }
    }

    public class VisionExtract
    {
        public VisionExtract(int pageCount)
        {
            PageCount = pageCount;
            Pages = new List<VisionPageExtract>();
        }

        public int PageCount { get; set; }

        public List<VisionPageExtract> Pages { get; set; }

    }



    public class VisionPageExtract
    {
        public VisionPageExtract(int pageNumber, int width, int height)
        {
            Words = new List<VisionWordExtract>();
            Width = width;
            Height = height;
            PageNumber = pageNumber;
        }

        public List<VisionWordExtract> Words { get; set; }

        public int PageNumber { get; set; }

        public int Height { get; set; }

        public int Width { get; set; }
    }



    public class VisionWordExtract
    {
        public VisionWordExtract(EntityAnnotation annotation, int width, int height)
        {
            BoundingPointsOfWord = new List<VisionVerticie>();
            foreach (var vert in annotation.BoundingPoly.Vertices)
            {
                BoundingPointsOfWord.Add(new VisionVerticie(vert.X, vert.Y));
            }
            Text = annotation.Description;
            SetTextSize(BoundingPointsOfWord, width);
            var cent = GetCentroid(BoundingPointsOfWord);
            CenterPointOfWord = new VisionVerticie(cent.X, cent.Y);
        }

        public VisionWordExtract(Word word, int width, int height)
        {
            BoundingPointsOfWord = new List<VisionVerticie>();
            foreach (var vert in word.BoundingBox.NormalizedVertices)
            {
                float x = vert.X;
                float y = vert.Y;

                if (x < 1)
                {
                    x *= width;
                }

                if (y < 1)
                {
                    y *= height;
                }

                BoundingPointsOfWord.Add(new VisionVerticie(x, y));
            }

            SetText(word.Symbols);
            var cent = GetCentroid(BoundingPointsOfWord);
            CenterPointOfWord = new VisionVerticie(cent.X, cent.Y);
            SetTextSize(BoundingPointsOfWord, width);

        }

        void SetText(Google.Protobuf.Collections.RepeatedField<Symbol> symbols)
        {
            StringBuilder newWord = new StringBuilder();

            foreach (var character in symbols)
            {
                newWord.Append(character.Text);
            }

            Text = newWord.ToString();
        }

        void SetTextSize(List<VisionVerticie> poly, int width)
        {
            float biggestY = poly.Select(m => m.Y).OrderByDescending(m => m).FirstOrDefault();
            float smallestY = poly.Select(m => m.Y).OrderBy(m => m).FirstOrDefault();
            float height = biggestY - smallestY;
            decimal point = (decimal)height * 72 / 96;
            TextSize = (int)decimal.Round(point);
        }


        private PointF GetCentroid(List<VisionVerticie> poly)
        {
            float accumulatedArea = 0.0f;
            float centerX = 0.0f;
            float centerY = 0.0f;

            for (int i = 0, j = poly.Count - 1; i < poly.Count; j = i++)
            {
                float temp = poly[i].X * poly[j].Y - poly[j].X * poly[i].Y;
                accumulatedArea += temp;
                centerX += (poly[i].X + poly[j].X) * temp;
                centerY += (poly[i].Y + poly[j].Y) * temp;
            }

            if (Math.Abs(accumulatedArea) < 1E-7f)
                return PointF.Empty;  // Avoid division by zero

            accumulatedArea *= 3f;
            return new PointF(centerX / accumulatedArea, centerY / accumulatedArea);
        }

        public string Text { get; set; }

        public int TextSize { get; set; }

        public VisionVerticie CenterPointOfWord { get; set; }

        public List<VisionVerticie> BoundingPointsOfWord { get; set; }

    }

    public class VisionVerticie
    {

        public VisionVerticie()
        {

        }

        public VisionVerticie(float x, float y)
        {
            X = x;
            Y = y;
        }

        public float X { get; set; }

        public float Y { get; set; }
    }






}

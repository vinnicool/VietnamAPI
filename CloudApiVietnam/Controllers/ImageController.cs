using CloudApiVietnam.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using AllowAnonymousAttribute = System.Web.Http.AllowAnonymousAttribute;
using AuthorizeAttribute = System.Web.Http.AuthorizeAttribute;
using System.Web.Management;
using Newtonsoft.Json;
using RouteAttribute = System.Web.Http.RouteAttribute;
using RoutePrefixAttribute = System.Web.Http.RoutePrefixAttribute;
using System.Net.Http.Headers;

namespace CloudApiVietnam.Controllers
{
    [Authorize]
    public class ImagesController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // To be set AzureStorage if not, sql database will be used to store images (less than 32gb not advised)
        private string ImageStoragetype = System.Configuration.ConfigurationManager.AppSettings["ImageStoragetype"];
        // GET specific Image
        public async Task<HttpResponseMessage> Get(string id)
        {
            var imageStream = new MemoryStream();
            HttpResponseMessage result;

            if (id == "")
                return IdIsNullResponse(out result);

            if (ImageStoragetype == "AzureStorage")
            {
                var container = getStorageAccount();
                var blockBlob = container.GetBlockBlobReference(id);

                try
                {
                    await blockBlob.DownloadToStreamAsync(imageStream);
                    imageStream.Position = 0;
                    if (imageStream == null)
                    {
                        result = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                        {
                            Content = new StringContent("Requested image could not be downloaded..")
                        };
                        return result;
                    }
                }
                catch (Exception e)
                {
                    if (e.Message.Contains("(404)"))
                    {
                        result = new HttpResponseMessage(HttpStatusCode.NotFound)
                        {
                            Content = new StringContent("Image not found")
                        };
                        return result;
                    }
                    else
                    {
                        result = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                        {
                            Content = new StringContent(e.Message)
                        };
                        return result;
                    }

                }
            }
            else
            {
                try
                {
                    var image = db.Image.Where(f => f.Name == id).FirstOrDefault();
                    imageStream = new MemoryStream(image.ImageData);
                }
                catch (Exception e)
                {
                    result = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                    {
                        Content = new StringContent(e.Message)
                    };
                    return result;
                }
            }

            result = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StreamContent(imageStream)
            };
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
            return result;
        }

        //POST
        public async Task<HttpResponseMessage> PostImage(FormImageModel model)
        {           
            if (ImageStoragetype == "AzureStorage")
            {
                try
                {
                    for (int i = 0; i < model.Image.Count; i++)
                    {
                        var imageName = String.Format($"{model.TemplateName}_{model.Name}_{model.BirthYear}_{i}");
                        using (var imageStream = new MemoryStream(model.Image[i]))
                        {
                            await AzureStorageAsync(imageStream, imageName);
                        }  
                    }
                }
                catch (Exception e)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, e.Message);
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        // POST
        public async Task<HttpResponseMessage> Post()
        {
            HttpResponseMessage result;

            if (!Request.Content.IsMimeMultipartContent())
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

            var provider = new MultipartMemoryStreamProvider();

            try
            {
                await Request.Content.ReadAsMultipartAsync(provider);
            }
            catch (Exception e)
            {
                if (provider.Contents.Count == 0)              
                    return result = Request.CreateResponse(HttpStatusCode.BadRequest, "Uploaded file was not received, missing image or wrong content-type header");               
                else              
                    return CheckIfFileIsToBig(e);               
            }

            var imageList = new List<Image>();

            foreach (var file in provider.Contents)
            {
                var filename = file.Headers.ContentDisposition.FileName.Trim('\"');
                // Checking for file type and size
                string imageType = filename.Split('.')[1];
                var types = new List<string> { "jpeg", "jpg", "png" };               
                if (!types.Contains(imageType, StringComparer.OrdinalIgnoreCase))
                    return result = Request.CreateResponse(HttpStatusCode.BadRequest, "Types of Digital Image should be: jpeg, jpg or png");
            }
            foreach (var file in provider.Contents)
            {
                var imageStream = await file.ReadAsStreamAsync();
                var blobNameReference = $"{Guid.NewGuid().ToString()}";

                if (ImageStoragetype == "AzureStorage")
                {
                    try
                    {
                        await AzureStorageAsync(imageStream, blobNameReference);
                        Image image = new Image();
                        image.Name = blobNameReference;
                        imageList.Add(image);
                    }
                    catch (Exception e)
                    {
                        return result = Request.CreateResponse(HttpStatusCode.InternalServerError, e.Message);
                    }
                }
                else
                {
                    try
                    {
                        Image image = new Image
                        {
                            Name = blobNameReference,
                            ImageData = ConvertStreamToBytes(imageStream)
                        };
                        SqlStorage(image);
                        image.ImageData = null;
                        imageList.Add(image);
                    }
                    catch (Exception e)
                    {
                        return result = Request.CreateResponse(HttpStatusCode.InternalServerError, e.Message);
                    }
                }
            }
            result = Request.CreateResponse(HttpStatusCode.Created, imageList);
            return result;

        }

        // DELETE specific image
        public async Task<HttpResponseMessage> Delete(string id)
        {
            HttpResponseMessage result;

            if (id == "")
                return IdIsNullResponse(out result);

            if (ImageStoragetype == "AzureStorage")
            {
                var container = getStorageAccount();

                var blockBlob = container.GetBlockBlobReference(id);
                // Delete the blob.
                try
                {
                    await blockBlob.DeleteAsync();
                }
                catch (Exception e)
                {
                    if (e.Message.Contains("(404)"))
                    {
                        result = new HttpResponseMessage(HttpStatusCode.NotFound)
                        {
                            Content = new StringContent("Image not found")
                        };
                        return result;
                    }
                    else
                    {
                        result = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                        {
                            Content = new StringContent(e.Message)
                        };
                        return result;
                    }

                }

            }
            else
            {
                try
                {
                    var image = db.Image.Where(f => f.Name == id).FirstOrDefault();
                    db.Image.Remove(image);
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    result = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                    {
                        Content = new StringContent(e.Message)
                    };
                    return result;
                }
            }

            result = new HttpResponseMessage(HttpStatusCode.NoContent);
            return result;
        }

        private HttpResponseMessage CheckIfFileIsToBig(Exception e)
        {
            // Returning a clean file is to big message.
            var errors = new List<string> { "De maximale aanvraaglengte is overschreden.", "Maximum request length exceeded.", "Độ dài yêu cầu tối đa đã bị vượt quá" };
            HttpResponseMessage result;
            if (errors.Contains(e.InnerException.Message, StringComparer.OrdinalIgnoreCase))
            {
                result = new HttpResponseMessage(HttpStatusCode.RequestEntityTooLarge)
                {
                    Content = new StringContent("Image size should be smaller than 5,0 MBytes")
                };
                return result;
            }
            else
            {
                result = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(e.Message)
                };
                return result;
            }
        }

        private async Task AzureStorageAsync(Stream image, string reference)
        {
            var container = getStorageAccount();
            await container.CreateIfNotExistsAsync();
            await container.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
            // Get Blob
            var blob = container.GetBlockBlobReference(reference);

            // Upload image
            await blob.UploadFromStreamAsync(image);
        }

        private void SqlStorage(Image image)
        {
            db.Image.Add(image);
            db.SaveChanges();
        }

        private CloudBlobContainer getStorageAccount()
        {
            // Get storage account
            CloudStorageAccount storageAccount = new CloudStorageAccount(
                new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials(System.Configuration.ConfigurationManager.AppSettings["azureStorageAccount"],
                System.Configuration.ConfigurationManager.AppSettings["azureStorageAccountKey"]), true);

            // Create Blob reference
            var blobClient = storageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference("patientimages");
            return container;
        }

        private byte[] ConvertStreamToBytes(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        private HttpResponseMessage IdIsNullResponse(out HttpResponseMessage result)
        {
            result = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("Image reference not given")
            };
            return result;
        }
    }
}

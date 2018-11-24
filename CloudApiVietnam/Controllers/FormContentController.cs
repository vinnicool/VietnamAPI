using CloudApiVietnam.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CloudApiVietnam.Controllers
{
    [Authorize]
    public class FormContentController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET alle FormContent
        public HttpResponseMessage Get()
        {
            try
            {
                var formContent = db.FormContent.ToList();
                if (formContent == null)               
                    return Request.CreateResponse(HttpStatusCode.NoContent, "No FormContent found");
                
                return Request.CreateResponse(HttpStatusCode.OK, formContent);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        // GET specefiek FormContent
        public HttpResponseMessage Get(int id)
        {
            var formContent = db.FormContent.Where(f => f.Id == id).FirstOrDefault();
            if (formContent == null)           
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No FormContent found with id: " + id.ToString());    
            else           
                return Request.CreateResponse(HttpStatusCode.OK, formContent);
            
        }

        // POST een FormContent
        public HttpResponseMessage Post(FormContentBindingModel formContentBindingModel)
        {
            try
            {
                var isJson = IsValidJson(formContentBindingModel.Content); // Check of JSON klopt en maak resultaat object
                if (!isJson.Status) // als resultaat object status fals is return error               
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "JSON in 'content' is not correct JSON: " + isJson.Error);
                
                var headersCheck = ContentEqualsHeaders(formContentBindingModel);
                if (!headersCheck.Status)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, headersCheck.Error);
                }

                FormContent formContent = new FormContent
                {
                    Content = formContentBindingModel.Content,
                    FormulierenId = formContentBindingModel.FormId
                };


                db.FormContent.Add(formContent);
                db.SaveChanges();

                return Request.CreateResponse(HttpStatusCode.OK, formContent);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        // PUT FormContent by Id
        public HttpResponseMessage Put(int id, [FromBody]FormContentBindingModel UpdateObject)
        {
            var formContent = db.FormContent.Where(f => f.Id == id).FirstOrDefault();

            if (formContent == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No FormContent found with id: " + id.ToString());
            }
            else
            {
                formContent.FormulierenId = UpdateObject.FormId;

                var isJson = IsValidJson(UpdateObject.Content); // Check of JSON klopt en maak resultaat object
                if (!isJson.Status) // als resultaat object status fals is return error                
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "JSON in 'content' is not correct JSON: " + isJson.Error);
                

                var headersCheck = ContentEqualsHeaders(UpdateObject);
                if (!headersCheck.Status)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, headersCheck.Error);
                }

                formContent.Content = UpdateObject.Content;
                try
                {
                    db.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK, formContent);
                }
                catch (Exception ex)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
                }
            }
        }

        // DELETE FormContent 
        public HttpResponseMessage Delete(int id)
        {
            var formContent = db.FormContent.Where(f => f.Id == id).FirstOrDefault();

            if (formContent == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No FormContent found with id: " + id.ToString());
            }
            else
            {
                try
                {
                    db.FormContent.Remove(formContent);
                    db.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                catch (Exception ex)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
                }
            }
        }


        private static IsJSON IsValidJson(string strInput)
        {
            IsJSON result = new IsJSON();
            strInput = strInput.Trim();
            if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || //For object
                (strInput.StartsWith("[") && strInput.EndsWith("]"))) //For array
            {
                try
                {
                    var obj = JToken.Parse(strInput);
                    result.Status = true;
                    return result;
                }
                catch (JsonReaderException jex)
                {
                    //Exception in parsing json
                    result.Error = jex.Message;
                    result.Status = false;
                    return result;
                }
                catch (Exception ex) //some other exception
                {
                    result.Error = ex.ToString();
                    result.Status = false;
                    return result;
                }
            }
            else
            {
                result.Status = false;
                result.Error = "JSON doesn't start or and with with '{/}' or '[/]' ";
                return result;
            }
        }

        private ContentEqualsHeadersCheck ContentEqualsHeaders(FormContentBindingModel formContentBindingModel)
        {
            var result = new ContentEqualsHeadersCheck();
            var Formulier = db.Formulieren.Where(f => f.Id == formContentBindingModel.FormId).FirstOrDefault(); //Haalt bijbehorende formulier op

            var obj = JToken.Parse(formContentBindingModel.Content); //Maak object van mee gegeven content

            foreach (var item in obj) //loop door mee gegeven content
            {
                //Pak de propery naam
                string jsonName = item.First.Path.ToString();
                var splitPath = jsonName.Split('.');
                string propertyName = splitPath[1];

                if (!Formulier.FormTemplate.Contains(propertyName))
                {
                    result.Status = false;
                    result.Error = "'" + propertyName + "'" + " is not found in the headers of the matching Formulier";
                    return result;
                }
            }
            result.Status = true;
            return result;
        }
    }
}

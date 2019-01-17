using CloudApiVietnam.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CloudApiVietnam.Controllers
{
    [AllowAnonymous]
    public class FormulierenController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET alle Formulieren
        [AllowAnonymous]
        public HttpResponseMessage Get()
        {
            try
            {
                var formulieren = db.Formulieren.ToList();
                return Request.CreateResponse(HttpStatusCode.OK, formulieren);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        // GET specifiek Formulier
        public HttpResponseMessage Get(int id)
        {
            var formulier = db.Formulieren.Where(f => f.Id == id).FirstOrDefault();
            if (formulier == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No Form found with id: " + id.ToString());
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK, formulier);
            }
        }


        // POST een Formulier
        public HttpResponseMessage Post(FormulierenBindingModel formulierenBindingModel)
        {
            try
            {
                var isJson = IsValidJson(formulierenBindingModel.FormTemplate); // Check of JSON klopt en maak resultaat object
                if (!isJson.Status) // als resultaat object status fals is return error                
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "JSON in 'content' is not correct JSON: " + isJson.Error);
                
                var formulier = new Formulieren
                {
                    Name = formulierenBindingModel.Name,
                    Region = formulierenBindingModel.Region,
                    FormTemplate = formulierenBindingModel.FormTemplate
                };

                db.Formulieren.Add(formulier);
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        // PUT api/values/5
        public HttpResponseMessage Put(int id, [FromBody]FormulierenBindingModel UpdateObject)
        {
            try
            {
                var form = db.Formulieren.Where(f => f.Id == id).FirstOrDefault();

                if (form == null)
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No form found with id: " + id.ToString());

                if (form.FormTemplate == UpdateObject.FormTemplate) //check if template is changed
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "The template hasn't been changed. Please submit a changed template.");

                IsJSON isJson = IsValidJson(UpdateObject.FormTemplate);

                if (!isJson.Status) // Check if new formTemplate is correct JSON
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "JSON in 'template' is not correct JSON: " + isJson.Error);

                form.Name = UpdateObject.Name;
                form.Region = UpdateObject.Region;
                form.FormTemplate = UpdateObject.FormTemplate;

                var formContentList = db.FormContent.Where(s => s.FormulierenId == id).ToList(); //get all the formContents related to the form

                if (formContentList.Count == 0)
                {
                    db.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK, form);
                }

                var formContentArray = new List<JArray>();

                foreach (var formContent in formContentList)
                    formContentArray.Add(JArray.Parse(formContent.Content)); //parse db data to JSON list

                var formTemplate = JArray.Parse(form.FormTemplate); //parse new template to JSON

                if (formTemplate.Count - formContentArray.FirstOrDefault().Count > 1 || formTemplate.Count - formContentArray.FirstOrDefault().Count < -1) //check if only 1 key is edited
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Only 1 key can be added/removed/edited at a time");

                UpdateFormContent(formContentArray, formTemplate);

                foreach (var content in formContentList)               
                    foreach (var newContent in formContentArray)
                    {
                        content.Content = newContent.ToString();
                        db.Entry(content).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                
                return Request.CreateResponse(HttpStatusCode.OK, form);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex); ;
            }

        }

        public HttpResponseMessage Delete(int id)
        {
            var formulier = db.Formulieren.Where(f => f.Id == id).FirstOrDefault();

            if (formulier == null)            
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No Form found with id: " + id.ToString());            
            else
            {
                try
                {
                    db.Formulieren.Remove(formulier);
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
            var result = new IsJSON();
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
                    Console.WriteLine(jex.Message);
                    result.Error = jex.Message;
                    result.Status = false;
                    return result;
                }
                catch (Exception ex) //some other exception
                {
                    Console.WriteLine(ex.ToString());
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

        private static void UpdateFormContent(List<JArray> formContentArray, JArray formTemplate)
        {
            foreach (var formContent in formContentArray.ToList()) //loop through formContent related to form
            {
                bool changed = false;
                var unchangedTokens = new List<string>();
                foreach (JObject formContentToken in formContent.ToList()) //loop through the tokens of each formContent
                {
                    var formContentProperty = formContentToken.Properties().ToList(); //property is being used to get the key

                    foreach (JObject formTemplateToken in formTemplate.ToList()) //loop through the tokens of formTemplate
                    {
                        var formTemplateProperty = formTemplateToken.Properties().ToList();

                        if (formContentProperty.First().Name != formTemplateProperty.First().Name && !changed && !unchangedTokens.Contains(formContentProperty.First().Name) && !unchangedTokens.Contains(formTemplateProperty.First().Name))
                        {
                            if (formContent.Count == formTemplate.Count) //check if a token is being edited, added or removed
                            {
                                formContentToken[formTemplateProperty.First().Name] = formContentProperty.First().Value;
                                formContentToken.Remove(formContentProperty.First().Name);//
                            }
                            if (formContent.Count < formTemplate.Count) //more headers than tokens
                                formContent.Add(formTemplateToken);

                            if (formContent.Count > formTemplate.Count) //more tokens than headers
                                formContentToken.Remove();

                            changed = true;
                        }
                        else if (formContentProperty.First().Name == formTemplateProperty.First().Name) //if a token is unchanged, put it in this list
                        {
                            unchangedTokens.Add(formTemplateProperty.First().Name);
                        }
                    }
                    formContentProperty.Remove(formContentProperty.First());
                }
                if (!changed && formContent.Count > formTemplate.Count) //remove last token of formContent if no token has been removed
                {
                    formContent.Remove(formContent.Last);
                }
                else if (!changed && formContent.Count < formTemplate.Count) //add last token of formTemplate if no token has been added
                {
                    formContent.Add(formTemplate.Last);
                }
            }
        }
    }
}

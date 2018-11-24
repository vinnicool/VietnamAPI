using Microsoft.VisualStudio.TestTools.UnitTesting;
using CloudApiVietnam.Controllers;
using CloudApiVietnam.Models;
using System.Net.Http;
using System.Net;
using System.Web.Http;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace CloudApiVietnam.Tests.Controllers
{
    /// <summary>
    /// Summary description for AccountControllerTest
    /// </summary>
    [TestClass]
    public class AccountControllerTest
    {  
        [TestMethod]
        [TestCleanup()]
        public void Delete_Succes()
        {
            // Act
            HttpResponseMessage result = controller.Delete(GetFormuContentId());
            // Assert
            Assert.AreEqual(result.StatusCode, HttpStatusCode.OK);
        }

        [TestMethod]
        public void Delete_Fail()
        {
            HttpResponseMessage result = controller.Delete(-99999);
            var resultContent = result.Content.ReadAsAsync<System.Web.Http.HttpError>().Result;
            // Assert
            Assert.AreEqual(result.StatusCode, HttpStatusCode.NotFound);
            Assert.AreEqual(resultContent.Message, "No FormContent found with id: -99999");
        }

        [TestMethod]
        public void GetById_Succes()
        {
            // Act
            HttpResponseMessage result = controller.Get(FormContentId);
            var resultContent = result.Content.ReadAsAsync<dynamic>().Result;
            // Assert
            Assert.AreEqual(result.StatusCode, HttpStatusCode.OK);
            Assert.IsNotNull(resultContent);
        }

        [TestMethod]
        public void GetById_Fail()
        {
            // Act
            HttpResponseMessage result = controller.Get(-99999);
            var resultContent = result.Content.ReadAsAsync<System.Web.Http.HttpError>().Result;
            // Assert
            Assert.AreEqual(result.StatusCode, HttpStatusCode.NotFound);
            Assert.AreEqual(resultContent.Message, "No FormContent found with id: -99999");
        }

        [TestMethod]
        public void GetAll_Succes()
        {
            // Act
            HttpResponseMessage result = controller.Get();
            var resultContent = result.Content.ReadAsAsync<dynamic>().Result;
            // Assert
            Assert.AreEqual(result.StatusCode, HttpStatusCode.OK);
            Assert.IsNotNull(resultContent);

        }

        [TestMethod]
        public void Put_Succes()
        {

            FormContentBindingModel formContentBindingModel = new FormContentBindingModel();

            formContentBindingModel.FormId = GetFormulierenTemplateId();
            formContentBindingModel.Content = "[{'Naam':'testnaam'},{'Leeftijd':'22'},{'Afwijking':'ADHD'}]";

            HttpResponseMessage result = controller.Put(FormContentId, formContentBindingModel);
            var resultContent = result.Content.ReadAsAsync<dynamic>().Result;
            // Assert
            Assert.AreEqual(result.StatusCode, HttpStatusCode.OK);
            Assert.IsNotNull(resultContent);

        }

        [TestMethod]
        [TestInitialize()]
        public void Post_Succes()
        {
            FormContentBindingModel formContentBindingModel = new FormContentBindingModel
            {
                Content = "[{'Naam':'testnaam'},{'Leeftijd':'22'},{'Afwijking':'ADHD'}]",
                FormId = GetFormulierenTemplateId()
            };

            // Act
            HttpResponseMessage result = controller.Post(formContentBindingModel);
            var resultContent = result.Content.ReadAsAsync<FormContent>().Result;
            FormContentId = resultContent.Id;

            // Assert
            Assert.AreEqual(result.StatusCode, HttpStatusCode.OK);
            Assert.IsNotNull(resultContent);
        }

        public int GetFormulierenTemplateId()
        {

            FormulierenController formulierencontroller = new FormulierenController
            {
                Request = new System.Net.Http.HttpRequestMessage(),
                Configuration = new HttpConfiguration()
            };

            // Act
            HttpResponseMessage actionResult = formulierencontroller.Get();

            // Assert
            List<Formulieren> formulier;
            Assert.IsTrue(actionResult.TryGetContentValue<List<Formulieren>>(out formulier));
            return formulier.FirstOrDefault().Id;
        }

        public int GetFormuContentId()
        {
            // Act
            HttpResponseMessage actionResult = controller.Get();

            // Assert
            List<FormContent> FormContentId;
            Assert.IsTrue(actionResult.TryGetContentValue<List<FormContent>>(out FormContentId));
            return FormContentId.FirstOrDefault().Id;
        }




        private static int FormContentId { get; set; }

        FormContentController controller = new FormContentController
        {
            Request = new System.Net.Http.HttpRequestMessage(),
            Configuration = new HttpConfiguration()
        };
    }
}

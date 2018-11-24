using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CloudApiVietnam;
using CloudApiVietnam.Controllers;
using CloudApiVietnam.Models;
using System.Net;

namespace CloudApiVietnam.Tests.Controllers
{
    [TestClass]
    public class FormContentControllerTest
    {
        private static int FormContentId { get; set; }

        FormContentController controller = new FormContentController
        {
            Request = new System.Net.Http.HttpRequestMessage(),
            Configuration = new HttpConfiguration()
        };



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


        [TestMethod]
        public void Post_Fail_JSON()
        {
            FormContentBindingModel formContentBindingModel = new FormContentBindingModel
            {
                Content = "[{Naam':'testnaam'},{'Leeftijd':'22'},{'Afwijking':'ADHD'}]",
                FormId = GetFormulierenTemplateId()
            };

            // Act
            HttpResponseMessage result = controller.Post(formContentBindingModel);
            var resultContent = result.Content.ReadAsAsync<System.Web.Http.HttpError>().Result;

            // Assert
            Assert.AreEqual(result.StatusCode, HttpStatusCode.BadRequest);
            Assert.AreEqual(resultContent.Message, "JSON in 'content' is not correct JSON: Invalid JavaScript property identifier character: '. Path '[0]', line 1, position 6.");
        }

        [TestMethod]
        public void Post_Fail_BracketMissing()
        {
            FormContentBindingModel formContentBindingModel = new FormContentBindingModel
            {
                Content = "'Naam':'testnaam'},{'Leeftijd':'22'},{'Afwijking':'ADHD'}]",
                FormId = GetFormulierenTemplateId()
            };

            // Act
            HttpResponseMessage result = controller.Post(formContentBindingModel);
            var resultContent = result.Content.ReadAsAsync<System.Web.Http.HttpError>().Result;

            // Assert
            Assert.AreEqual(result.StatusCode, HttpStatusCode.BadRequest);
            Assert.AreEqual(resultContent.Message, "JSON in 'content' is not correct JSON: JSON doesn't start or and with with '{/}' or '[/]' ");
        }

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
            var resultContent = result.Content.ReadAsAsync<FormContent>().Result;
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


    }
}

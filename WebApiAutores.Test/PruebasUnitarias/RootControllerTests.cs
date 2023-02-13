using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebApiAutores.Controllers.V1;
using WebApiAutores.Test.Mocks;

namespace WebApiAutores.Test.PruebasUnitarias
{
    [TestClass]
    public class RootControllerTests
    {
        [TestMethod]
        public async Task SiUsuarioEsAdmin_Devuelve4Links() 
        {
            //Preparacion
            var authorizationService = new AuthorizationServiceSuccessMock();
            authorizationService.resultado = AuthorizationResult.Success();
            var rootController = new RootController(authorizationService);
            rootController.Url = new URLHelperMock();
            //Ejecucion
            var resultado = await rootController.Get();
            //Verificacion
            Assert.AreEqual(4,resultado.Value.Count());
        }
        [TestMethod]
        public async Task SiUsuarioNoEsAdmin_Devuelve2Links()
        {
            //Preparacion
            var authorizationService = new AuthorizationServiceSuccessMock();
            authorizationService.resultado = AuthorizationResult.Failed();
            var rootController = new RootController(authorizationService);
            rootController.Url = new URLHelperMock();
            //Ejecucion
            var resultado = await rootController.Get();
            //Verificacion
            Assert.AreEqual(2, resultado.Value.Count());
        }
        [TestMethod]
        public async Task SiUsuarioNoEsAdmin_Devuelve2LinksUsandoMock()
        {
            //Preparacion
            var mockAuthorization = new Mock<IAuthorizationService>();
            mockAuthorization.Setup(x => x.AuthorizeAsync(
                It.IsAny<ClaimsPrincipal>(),
                It.IsAny<object>(),
                It.IsAny<IEnumerable<IAuthorizationRequirement>>()
                )).Returns(Task.FromResult(AuthorizationResult.Failed()));

            mockAuthorization.Setup(x => x.AuthorizeAsync(
               It.IsAny<ClaimsPrincipal>(),
               It.IsAny<object>(),
               It.IsAny<string>()
               )).Returns(Task.FromResult(AuthorizationResult.Failed()));

            var mockUrlHelper = new Mock<IUrlHelper>();
            mockUrlHelper.Setup(x =>
                x.Link(It.IsAny<string>(),
                It.IsAny<object>())).Returns(string.Empty);

            var rootController = new RootController(mockAuthorization.Object);
            rootController.Url = mockUrlHelper.Object;
            //Ejecucion
            var resultado = await rootController.Get();
            //Verificacion
            Assert.AreEqual(2, resultado.Value.Count());
        }
    }
}

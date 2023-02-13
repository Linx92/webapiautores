using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using WebApiAutores.DTOs;

namespace WebApiAutores.Servicios
{
    public class GeneradorEnlaces
    {
        private readonly IAuthorizationService authorizationService;
        private readonly IHttpContextAccessor httpContextAccesor;
        private readonly IActionContextAccessor actionContextAccessor;

        public GeneradorEnlaces(IAuthorizationService authorizationService, 
            IHttpContextAccessor httpContextAccesor, IActionContextAccessor actionContextAccessor)
        {
            this.authorizationService = authorizationService;
            this.httpContextAccesor = httpContextAccesor;
            this.actionContextAccessor = actionContextAccessor;
        }
        private IUrlHelper ConstruirUrlHelper() 
        {
            var factoria = httpContextAccesor.HttpContext.RequestServices.GetRequiredService<IUrlHelperFactory>();
            return factoria.GetUrlHelper(actionContextAccessor.ActionContext);
        }
        private async Task<bool> EsAdmin() 
        {
            var httpcontext = httpContextAccesor.HttpContext;
            var resultado = await authorizationService.AuthorizeAsync(httpcontext.User, "esAdmin");
            return resultado.Succeeded;
        }
        public async Task GenerarEnlaces(AutorDTO autorDTO)
        {
            var esAdmin = await EsAdmin();
            var Url = ConstruirUrlHelper();
            autorDTO.Enlaces.Add(new DatoHATEOAS(
                enlace: Url.Link("obtenerAutor", new { Id = autorDTO.Id }), descripcion: "self", metodo: "GET"));
            if (esAdmin)
            {
                autorDTO.Enlaces.Add(new DatoHATEOAS(
                enlace: Url.Link("actualizarAutor", new { Id = autorDTO.Id }), descripcion: "autor - actualizar", metodo: "PUT"));
                autorDTO.Enlaces.Add(new DatoHATEOAS(
                    enlace: Url.Link("borrarAutor", new { Id = autorDTO.Id }), descripcion: "self", metodo: "DELETE"));
            }

        }
    }
}

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApiAutores.DTOs;

namespace WebApiAutores.Controllers.V1
{
    [ApiController]
    [Route("api/v1")] 
    [Authorize(AuthenticationSchemes =JwtBearerDefaults.AuthenticationScheme)]
    public class RootController:ControllerBase
    {
        private readonly IAuthorizationService authorizationService;

        public RootController(IAuthorizationService authorizationService)
        {
            this.authorizationService = authorizationService;
        }

        [HttpGet(Name = "ObtenerRoot")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<DatoHATEOAS>>> Get() 
        {
            var esAdmin = await authorizationService.AuthorizeAsync(User, "esAdmin");
            var datoshateoas = new List<DatoHATEOAS>();
            datoshateoas.Add(new DatoHATEOAS(enlace: Url.Link("ObtenerRoot", new { }), descripcion: "self", metodo: "GET"));
            datoshateoas.Add(new DatoHATEOAS(enlace:Url.Link("obtenerAutores", new { }), descripcion:"autores", metodo:"GET"));
            if (esAdmin.Succeeded) 
            {
                datoshateoas.Add(new DatoHATEOAS(enlace: Url.Link("crearAutor", new { }), descripcion: "crear - autor", metodo: "POST"));
                datoshateoas.Add(new DatoHATEOAS(enlace: Url.Link("crearLibro", new { }), descripcion: "crear - libro", metodo: "POST"));
            }
         
            return datoshateoas;
        }
    }
}

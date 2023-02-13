using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.DTOs;
using WebApiAutores.Entidades;
using WebApiAutores.Filtros;
using WebApiAutores.Utilidades;

namespace WebApiAutores.Controllers.V2
{
    [ApiController]
    //[Route("api/v2/autores")]//Tambien se puede encontrar como
                             //api/[controller](no es recomendable
                             //por si se cambia el nombre del controlador)
    [Route("api/autores")]
    [CabeceraEstaPresente("x-version", "2")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,Policy ="EsAdmin")]
    public class AutoresController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IAuthorizationService authorizationService;

        public AutoresController(ApplicationDbContext context, IMapper mapper, IAuthorizationService authorizationService)
        {
            this.context = context;
            this.mapper = mapper;
            this.authorizationService = authorizationService;
        }
        //ruta principal del controlador api/autores
        ////[HttpGet("listado")]//Se agrega a la ruta principal  api/autores/listado
        ////[HttpGet("/listado")]//Reemplaza la ruta principal por /listado
        //[ResponseCache(Duration =10)] Carga en memoria información que se mostrará por los proximos 10 segundos
        //[Authorize]//Filtro de acceso a la api
        // [ServiceFilter(typeof(MiFiltrodeAccion))]
        [HttpGet(Name = "obtenerAutoresv2")]
        [AllowAnonymous]
        [ServiceFilter(typeof(HATEOASAutorFilterAttribute))]
        public async Task<ActionResult<List<AutorDTO>>> Get()
        {
          
            var autores = await context.Autores.ToListAsync();
            autores.ForEach(autor => autor.Nombre = autor.Nombre.ToUpper());
            return mapper.Map<List<AutorDTO>>(autores);
         
        }
        [HttpGet("{id:int}", Name = "obtenerAutorv2")]//{id:int}/{param1}/{param2}--Sirve para agregar mas parametros;
                             //{param?}acepta valores nulos
        [AllowAnonymous]
        [ServiceFilter(typeof(HATEOASAutorFilterAttribute))]
        public async Task<ActionResult<AutorDTOConLibros>> Get(int id) 
        {
      
            var autor = await context.Autores
                .Include(x=>x.AutorLibro)
                .ThenInclude(x=>x.Libro)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (autor == null) 
            {
                return NotFound();
            }
            //var esAdmin = await authorizationService.AuthorizeAsync(User, "esAdmin");
            var dto = mapper.Map<AutorDTOConLibros>(autor);
           // await GenerarEnlaces(dto, esAdmin.Succeeded);
            return dto;
        }
      

        [HttpGet("{nombre}",Name = "obtenerAutorPorNombrev2")]
        public async Task<ActionResult<List<AutorDTO>>> GetPorNombre(string nombre)
        {
            var autores = await context.Autores.Where(x => x.Nombre.Contains(nombre)).ToListAsync();

            return mapper.Map<List<AutorDTO>>(autores);
        }

        [HttpPost(Name = "crearAutorv2")]
        public async Task<ActionResult> Post(AutorCreacionDTO autorCreacionDTO)
        {
            var existeAutorConElMismoNombre = await context.Autores.AnyAsync(x => x.Nombre == autorCreacionDTO.Nombre);

            if (existeAutorConElMismoNombre)
            {
                return BadRequest($"Ya existe un autor con el nombre {autorCreacionDTO.Nombre}");
            }

            var autor = mapper.Map<Autor>(autorCreacionDTO);
            context.Add(autor);
            context.SaveChanges();
            var autorDTO = mapper.Map<AutorDTO>(autor);
            return CreatedAtRoute("obtenerAutorv2", new {id= autorDTO.Id }, autorDTO);
        }
        [HttpPut("{id:int}", Name = "actualizarAutorv2")]
        public async Task<ActionResult> Put(AutorCreacionDTO autorcreacionDTO, int id) 
        {
            var existe = await context.Autores.AnyAsync(x => x.Id == id);
            if (!existe) 
            {
                return NotFound();
            }
            var autor = mapper.Map<Autor>(autorcreacionDTO);
            context.Update(autor);
            await context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id:int}", Name = "borrarAutorv2")]
        public async Task<ActionResult> Delete(int id) 
        {
            var existe = await context.Autores.AnyAsync(x => x.Id == id);
            if (!existe) 
            {
                return NotFound();
            }

            context.Remove(new Autor { Id = id });
            await context.SaveChangesAsync();

            return Ok();
        }
    }
}

using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.DTOs;
using WebApiAutores.Entidades;
using WebApiAutores.Utilidades;

namespace WebApiAutores.Controllers.V1
{
    [ApiController]
    [Route("api/v1/libros/{libroId:int}/comentario")]
    public class ComentariosController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly UserManager<IdentityUser> userManager;

        public ComentariosController(ApplicationDbContext context, IMapper mapper,
            UserManager<IdentityUser> userManager)
        {
            this.context = context;
            this.mapper = mapper;
            this.userManager = userManager;
        }
        [HttpGet(Name ="obtenerComentarios")]
        public async Task<ActionResult<List<ComentarioDTO>>> Get(int libroId, [FromQuery] PaginacionDTO paginacionDTO)
        {
            var existeLibro = await context.Libros.AnyAsync(x => x.Id == libroId);
            if (!existeLibro)
            {
                return NotFound();
            }
            var queryable = context.Comentarios.Where(x => x.LibroId == libroId).AsQueryable();
            await HttpContext.InsertarParametrosPaginacionEnCabecera(queryable);
            var comentarios = await queryable.OrderBy(x => x.Id).Paginar(paginacionDTO).ToListAsync();
            return mapper.Map<List<ComentarioDTO>>(comentarios);
        }
        [HttpGet("{id:int}", Name ="obtenerComentario")]
        public async Task<ActionResult<ComentarioDTO>> GetById(int id) 
        {
            var comentario = await context.Comentarios.FirstOrDefaultAsync(x => x.Id == id);
            if (comentario == null) 
            {
                return NotFound();
            }

            return mapper.Map<ComentarioDTO>(comentario);
        }
        [HttpPost(Name ="crearComentario")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Post(int libroId, ComentarioCreacionDTO comentarioCreacionDTO) 
        {

            var email = HttpContext.User.Claims.Where(x => x.Type == "email").FirstOrDefault().Value;
            var usuario = await userManager.FindByEmailAsync(email);
            var usuarioId = usuario.Id;
            var existeLibro = await context.Libros.AnyAsync(x => x.Id == libroId);
            if (!existeLibro) 
            {
                return NotFound();
            }
            var comentario = mapper.Map<Comentario>(comentarioCreacionDTO);
            comentario.LibroId = libroId;
            comentario.UsuarioId = usuarioId;
            context.Add(comentario);
            await context.SaveChangesAsync();

            var comentarioDTO = mapper.Map<ComentarioDTO>(comentario);
            return CreatedAtRoute("obtenerComentario", new { id=comentarioDTO.Id, libroId = comentario.LibroId}, comentarioDTO);
        }
        [HttpPut("{id:int}", Name ="actualizarComentario")]
        public async Task<ActionResult> Put(int libroId, int id, ComentarioCreacionDTO comentarioCreacionDTO)
        {
            var existeLibro = await context.Libros.AnyAsync(x => x.Id == libroId);
            if (!existeLibro) { return NotFound(); }
            var existeComentario = await context.Comentarios.AnyAsync(x => x.Id == id);
            if (!existeComentario) { return NotFound(); }

            var comentario = mapper.Map<Comentario>(comentarioCreacionDTO);
            comentario.Id = id;
            comentario.LibroId = libroId;
            context.Update(comentario);
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}

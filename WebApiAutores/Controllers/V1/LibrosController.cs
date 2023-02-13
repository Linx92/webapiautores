using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.DTOs;
using WebApiAutores.Entidades;
using WebApiAutores.Utilidades;

namespace WebApiAutores.Controllers.V1
{
    [ApiController]
    [Route("api/v1/libros")]
    public class LibrosController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public LibrosController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }
        [HttpGet(Name ="obtenerLibros")]
        public async Task<ActionResult<List<LibroDTOConAutores>>> Get([FromQuery] PaginacionDTO  paginacionDTO)
        {
            var queryable = context.Libros
                .Include(x => x.Comentarios)
                .Include(x => x.AutorLibro)
                .ThenInclude(x => x.Autor).AsQueryable();
            await HttpContext.InsertarParametrosPaginacionEnCabecera(queryable);
            var libros = await queryable.OrderBy(o => o.Titulo).Paginar(paginacionDTO).ToListAsync();

            return mapper.Map<List<LibroDTOConAutores>>(libros);
        }
        [HttpGet("{id:int}", Name = "obtenerLibro")]
        public async Task<ActionResult<LibroDTOConAutores>> Get(int id)
        {
            var libro = await context.Libros
                .Include(x => x.Comentarios)
                .Include(x => x.AutorLibro)
                .ThenInclude(x => x.Autor).FirstOrDefaultAsync(x => x.Id == id);
            if (libro == null) 
            {
                return NotFound();
            }
            libro.AutorLibro = libro.AutorLibro.OrderBy(x => x.Orden).ToList();
            return mapper.Map<LibroDTOConAutores>(libro);
        }
        [HttpPost(Name ="crearLibro")]
        public async Task<ActionResult> Post(LibroCreacionDTO libroCreacion)
        {
            if (libroCreacion.AutoresIds == null)
            {
                return BadRequest("No se puede crear libros sin autores");
            }
            var autoresIds = await context.Autores.Where(x => libroCreacion.AutoresIds.Contains(x.Id))
                .Select(x => x.Id).ToListAsync();
            if (autoresIds.Count != libroCreacion.AutoresIds.Count)
            {
                return BadRequest("Uno de los autores no existen en la Base de Datos");
            }
            var libro = mapper.Map<Libro>(libroCreacion);
            AsignarOrdenAutores(libro);


            context.Add(libro);
            await context.SaveChangesAsync();

            var libroDTO = mapper.Map<LibroDTO>(libro);
            return CreatedAtRoute("obtenerLibro", new { id = libroDTO.Id }, libroDTO);
        }
        [HttpPut("{id:int}", Name ="actualizarLibro")]
        public async Task<ActionResult> Put(int id, LibroCreacionDTO libroCreacionDTO)
        {
            var libroBD = await context.Libros
                .Include(x => x.AutorLibro).FirstOrDefaultAsync(x => x.Id == id);
            if (libroBD == null)
            {
                return NotFound();
            }

            libroBD = mapper.Map(libroCreacionDTO, libroBD);

            AsignarOrdenAutores(libroBD);
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{id:int}", Name ="patchLibro")]
        public async Task<ActionResult> Patch(int id, JsonPatchDocument<LibroPatchDTO> libroPatchDocument) 
        {
            if (libroPatchDocument==null) 
            {
                return BadRequest();
            }

            var libroBD = await context.Libros.FirstOrDefaultAsync(x => x.Id == id);

            if (libroBD == null) 
            {
                return BadRequest();
            }
            var libroDTO = mapper.Map<LibroPatchDTO>(libroBD);
            libroPatchDocument.ApplyTo(libroDTO, ModelState);

            var esValido = TryValidateModel(libroDTO);
            if (!esValido) 
            {
                return BadRequest(ModelState);
            }

            mapper.Map(libroDTO, libroBD);

            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}", Name ="borrarLibro")]
        public async Task<ActionResult> Delete(int id)
        {
            var existe = await context.Libros.AnyAsync(x => x.Id == id);
            if (!existe)
            {
                return NotFound();
            }

            context.Remove(new Libro { Id = id });
            await context.SaveChangesAsync();
            return NoContent();
        }
        private void AsignarOrdenAutores(Libro libro) 
        {
            if (libro.AutorLibro != null)
            {
                for (int i = 0; i < libro.AutorLibro.Count; i++)
                {
                    libro.AutorLibro[i].Orden = i;
                }
            }
        }
    }
}

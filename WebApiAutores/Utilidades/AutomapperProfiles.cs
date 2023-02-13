using AutoMapper;
using WebApiAutores.DTOs;
using WebApiAutores.Entidades;

namespace WebApiAutores.Utilidades
{
    public class AutomapperProfiles:Profile
    {
        public AutomapperProfiles()
        {
            CreateMap<AutorCreacionDTO, Autor>();//Insertar registros a la BD
            CreateMap<Autor, AutorDTO>();
            CreateMap<Autor,AutorDTOConLibros>()
                .ForMember(autor=>autor.Libros, opciones=>opciones.MapFrom(MapLibroAutores));//Extraer registros de la BD
            CreateMap<AutorDTO, Autor>();//Actualizar registros de la BD
            CreateMap<Libro, LibroDTO>().ReverseMap();
            CreateMap<LibroPatchDTO, Libro>().ReverseMap();
            CreateMap<Libro,LibroDTOConAutores>()
                .ForMember(libro=>libro.Autor, opciones=>opciones.MapFrom(MapLibroDTOAutor));
            CreateMap<LibroCreacionDTO, Libro>()
                .ForMember(libro=>libro.AutorLibro,opciones=>opciones.MapFrom(MapAutoresLibros));
            CreateMap<ComentarioCreacionDTO, Comentario>();
            CreateMap<Comentario, ComentarioDTO>();
            CreateMap<ComentarioDTO, Comentario>();

        }
        private List<LibroDTO> MapLibroAutores(Autor autor, AutorDTO autorDTO)
        {
            var resultado = new List<LibroDTO>();
            if (autor.AutorLibro == null) { return resultado; }
            foreach (var libro in autor.AutorLibro) 
            {
                resultado.Add(new LibroDTO() { Id = libro.LibroId, Titulo = libro.Libro.Titulo });
            }
            return resultado;
        }
        private List<AutorDTO> MapLibroDTOAutor(Libro libro, LibroDTO libroDTO) 
        {
            var resultado = new List<AutorDTO>();
            if (libro.AutorLibro == null) { return resultado; }
            foreach (var autor in libro.AutorLibro)
            {
                resultado.Add(new AutorDTO() {Id=autor.AutorId,Nombre=autor.Autor.Nombre });
            }

            return resultado;            
        }
        private List<AutorLibro> MapAutoresLibros(LibroCreacionDTO libroCreacionDTO, Libro libro)
        {
            var resultado = new List<AutorLibro>();
            if (libroCreacionDTO == null) { return resultado; }

            foreach (var autorId in libroCreacionDTO.AutoresIds) 
            {
                resultado.Add(new AutorLibro() { AutorId=autorId});
            }
            return resultado;
        }
    }
}

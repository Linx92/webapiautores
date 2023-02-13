using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApiAutores.Validaciones;

namespace WebApiAutores.Entidades
{
    public class Autor
    {
        public int Id { get; set; }
        [PrimeraLetraMayuscula]
        [Required(ErrorMessage ="El campo nombre es requerido")]//Siempre vamos a tener que recibir el valor del campo nombre
        [StringLength(maximumLength:120,ErrorMessage ="El campo {0} no puede tener mas de {1} caracteres")]
        public string Nombre { get; set; }
        public List<AutorLibro> AutorLibro { get; set; }

    }
}

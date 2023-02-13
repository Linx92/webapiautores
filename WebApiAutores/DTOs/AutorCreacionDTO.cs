using System.ComponentModel.DataAnnotations;
using WebApiAutores.Validaciones;

namespace WebApiAutores.DTOs
{
    public class AutorCreacionDTO
    {
        [PrimeraLetraMayuscula]
        [Required(ErrorMessage = "El campo nombre es requerido")]//Siempre vamos a tener que recibir el valor del campo nombre
        [StringLength(maximumLength: 120, ErrorMessage = "El campo {0} no puede tener mas de {1} caracteres")]
        public string Nombre { get; set; }

    }
}

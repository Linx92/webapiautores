using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.DataAnnotations;
using WebApiAutores.Validaciones;

namespace WebApiAutores.Test.PruebasUnitarias
{
    [TestClass]
    public class PrimeraLetraMayusculaAttributeTests
    {
        [TestMethod]
        public void PrimeraLetraMinuscula_DevuelveError()
        {
            //Preparacion
            var primeraLetraMayuscula = new PrimeraLetraMayusculaAttribute();
            var valor = "cadena";
            var valContext = new ValidationContext(new { Nombre = valor});
            //Ejecucion
            var resultado = primeraLetraMayuscula.GetValidationResult(valor, valContext);

            //Verificacion
            Assert.AreEqual("La primera letra debe ser mayúscula",resultado.ErrorMessage);
        }
        [TestMethod]
        public void ValorNulo_NoDevuelveError()
        {
            //Preparacion
            var primeraLetraMayuscula = new PrimeraLetraMayusculaAttribute();
            string valor = null;
            var valContext = new ValidationContext(new { Nombre = valor });
            //Ejecucion
            var resultado = primeraLetraMayuscula.GetValidationResult(valor, valContext);

            //Verificacion
            Assert.IsNull(resultado);
        }
        [TestMethod]
        public void ValorConPrimeraLetraMayuscula_NoDevuelveError()
        {
            //Preparacion
            var primeraLetraMayuscula = new PrimeraLetraMayusculaAttribute();
            string valor = "Linford";
            var valContext = new ValidationContext(new { Nombre = valor });
            //Ejecucion
            var resultado = primeraLetraMayuscula.GetValidationResult(valor, valContext);

            //Verificacion
            Assert.IsNull(resultado);
        }
    }
}
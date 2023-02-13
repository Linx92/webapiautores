namespace WebApiAutores.Servicios
{
    public interface IServicio
    {
        void RealizarTarea();
    }
    public class ServicioA : IServicio
    {
        //private readonly ILogger logger;

        //public ServicioA(ILogger logger) 
        //{
        //    this.logger = logger;
        //}
        public void RealizarTarea()
        {
            throw new NotImplementedException();
        }
    }
}

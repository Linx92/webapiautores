namespace WebApiAutores.Servicios
{
    public class EscribirEnArchivo : IHostedService
    {
        private readonly IWebHostEnvironment env;
        private readonly string nombreArchivo = "Archivo 1.txt";
        private Timer timer;

        public EscribirEnArchivo(IWebHostEnvironment env) 
        {
            this.env = env;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            EscribirMensaje("Proceso Iniciado");
            timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
            return Task.CompletedTask;
        }
        private void DoWork(object state) 
        {
            EscribirMensaje("Procesos en ejecución" + DateTime.Now.ToString("dd/mm/yyyy hh:mm:ss"));
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            timer.Dispose();
            EscribirMensaje("Procesos Finalizado");
            return Task.CompletedTask;
        }
        private void EscribirMensaje(string mensaje) 
        {
            var ruta = $@"{env.ContentRootPath}\wwwroot\{nombreArchivo}";
            using (StreamWriter writer = new StreamWriter(ruta, append: true)) 
            {
                writer.WriteLine(mensaje);
            }
        }
    }
}

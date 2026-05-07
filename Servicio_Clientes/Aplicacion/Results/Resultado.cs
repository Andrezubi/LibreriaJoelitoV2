namespace Servicio_Clientes.Aplicacion.Results
{
    public class Resultado
    {
        public bool EsExito { get; private set; }
        public string Mensaje { get; private set; }
        public List<string> Errores { get; private set; }

        private Resultado(bool exito, string mensaje, List<string>? errores = null)
        {
            EsExito = exito;
            Mensaje = mensaje;
            Errores = errores ?? new List<string>();
        }

        public static Resultado Success(string mensaje = "Operación exitosa")
        {
            return new Resultado(true, mensaje);
        }

        public static Resultado Failure(string error)
        {
            return new Resultado(false, "Error en la operación", new List<string> { error });
        }

        public static Resultado Failure(List<string> errores)
        {
            return new Resultado(false, "Errores de validación", errores);
        }
    }
}

namespace Core.DTOs
{
    public class IM_WMS_EstadoTrasladosDTO
    {
        public string TRANSFERID { get; set; }
        public string Estado { get; set; }
        public int QTY  { get; set; }
        public int Enviado { get; set; }
        public int Recibido { get; set; }

    }
}

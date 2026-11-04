namespace Core.DTOs.AuditoriaInterna
{
    // QR de la caja: "OP,Caja"
    public class AbrirCajaAuditoriaRequest
    {
        public string QRCaja { get; set; }
        public string Usuario { get; set; }
    }

    public class EscanearPiezaAuditoriaRequest
    {
        public int AuditoriaId { get; set; }
        public string Codigo { get; set; }
        public string Usuario { get; set; }
    }

    // Reabrir y reiniciar: solo usuarios con la pantalla AuditoriaInternaReabrir en IM_WMS_UsuarioPorPantalla.
    public class ReabrirCajaAuditoriaRequest
    {
        public int AuditoriaId { get; set; }
        public string Motivo { get; set; }
        public string Usuario { get; set; }
    }

    // Cerrar = false guarda las observaciones y deja la caja abierta.
    public class CerrarCajaAuditoriaRequest
    {
        public int AuditoriaId { get; set; }
        public string Observaciones { get; set; }
        public string Usuario { get; set; }
        public bool Cerrar { get; set; } = true;
    }
}

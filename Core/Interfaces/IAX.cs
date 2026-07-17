using Core.DTOs.IM_PrepEnvOp;
using Core.DTOs.InventarioCiclicoTela;
using Core.DTOs.RecepcionYUbicacionAX;
using Core.DTOs.Serigrafia;
using Core.DTOs.Serigrafia.ClaseRespuesta;
using Core.DTOs.TejidoPunto;
using Core.DTOs.UbiacacionRollos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IAX
    {
        public Task<string> RegistrarMovimientoRollosEnDiario(List<MovimientoRolloDto> rollosAMover);
        public Task<Respuesta<string>> AgregarNuevaUbicacion(string empresa, string ubicacion, string almacen, string pasillo);

        public string InsertDeleteMovimientoLine(string JOURNALID, string ITEMBARCODE, string PROCESO, string IMBOXCODE);
        public string EnviarRecibirTraslados(string TRANSFERID, string ESTADO);
        public Task<Respuesta<string>> RecibirTrasladoYCambioUbiacion(string trasladoId, InformacionEmpresa informacion);
        public string INsertDeleteReduccionCajas(string ITEMBARCODE, string PROCESO, string IMBOXCODE);
        public string InsertDeleteEntradaMovimientoLine(string JOURNALID, string ITEMBARCODE, string PROCESO);
        public string InsertDeleteTransferirMovimientoLine(string JOURNALID, string ITEMBARCODE, string PROCESO);
        public string InsertAddInventarioCiclicoTelaLine(List<INVENTARIOCICLICOTELALINE> LIST);
        public string InsertCajasRecicladas(string qty, string CentroCosto,string diario);
        public Task<IM_PrepEnvOp_TrasladoResultDTO> MarcarTrasladoComoRecibido(string inventTransferId, string tipo);
        public Task<string> CambioIniciadoEstadoOpSerigrafia(OpPorBaseDTO orde);
        public Task<List<Respuesta<string>>> CambioANotificadoEstadoOPSerigrafia(List<IM_WMS_SRG_DatosParaNotificarRespuesta> datos);
        public Task<string> AjustarCantidadPorOP(OpPorBaseDTO orden);
        public Task<string> CrearTrasladosPorArticulo(TrasladoDTO trasladoDTO);
        public string CrearDiario(DiarioHeaderDTO headerDTO, DiarioLineasDTO lineasDTO, string accíon);
        public Task<List<Respuesta<string>>> NotificacionSubcontratacionTejidoPunto(List<IM_WMS_NOTIFICARSUBCONTRATACIONTEJIDOPUNTO> datos);
        public Task<Respuesta<string>> ConfirmacionRecepcionDePedidoDeCompra(ConfirmacionRecepcionDTO confirmacionRecepcion);
    }
}

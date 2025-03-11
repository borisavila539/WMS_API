using Core.DTOs.MB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IWMSMBRespository
    {
        
        public Task<IM_WMS_MB_InsertBox> getInsertBox(string Orden, int Caja, string Ubicacion, int Consolidado, string usuarioRecepcion, string Camion);
        public Task<List<IM_WMS_MB_InsertBox>> getCajasEscaneadasRack(string ubicacion);
        public Task<string> postEnviarCorreoRecepcion(List<IM_WMS_MB_InsertBox> data);
        public Task<List<IM_WMS_MB_CajasDisponibles>> GetCajasDisponibles(FiltroCajasDisponiblesMB filtro);
        public Task<IM_WMS_MB_CajasDisponibles> getActualizarCajasParaDespacho(int id,Boolean PickToDespacho);
        public Task<List<IM_WMS_MB_ResumenArticulosSeleccionados>> GetResumenArticulosSeleccionados();
        public Task<IM_WMS_MB_CrearDespacho> getGenerarDespacho(string usuario);
        public Task<List<IM_WMS_MB_CrearDespacho>> getDespachosPendientes();
        public Task<List<IM_WMS_MB_PICKING>> getPicking(int DespachoID);
        public Task<IM_WMS_MB_PICKING> getUpdatePicking(int id, string usuario);
    }
}

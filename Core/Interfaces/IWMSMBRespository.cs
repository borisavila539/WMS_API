using Core.DTOs.Despacho_PT;
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
        public Task<List<IM_WMS_MB_CajasDisponibles2>> GetCajasDisponibles(FiltroCajasDisponiblesMB filtro);
        public Task<List<IM_WMS_MB_CajasDisponibles>> GetCajasDisponiblesTodo(FiltroCajasDisponiblesMB Filtro);
        public Task<IM_WMS_MB_CajasDisponibles> getActualizarCajasParaDespacho(int id,Boolean PickToDespacho);
        public Task<List<IM_WMS_MB_ResumenArticulosSeleccionados>> GetResumenArticulosSeleccionados();
        public Task<IM_WMS_MB_CrearDespacho> getGenerarDespacho(string usuario);
        public Task<List<IM_WMS_MB_CrearDespacho>> getDespachosPendientes();
        public Task<List<IM_WMS_MB_PICKING>> getPicking(int DespachoID);
        public Task<IM_WMS_MB_PICKING> getUpdatePicking(int id, string usuario);
        public Task<List<IM_WMS_MB_PACKING>> getPacking(int DespachoID);
        public Task<IM_WMS_MB_PACKING> getUpdatePacking(int id, string usuario,string Pallet);
        public Task<List<IM_WMS_MB_Tracking>> getTracking(int id);

        public Task<List<IM_WMS_MB_Trackingpallet>> getTrackingPallet(int id);
        public Task<List<IM_WMS_MB_ResumenDespachoPallet>> GetResumenDespachoPallets(int id);
        public  Task<List<IM_WMS_Correos_DespachoPTDTO>> getCorreosDespachoMB();

    }
}

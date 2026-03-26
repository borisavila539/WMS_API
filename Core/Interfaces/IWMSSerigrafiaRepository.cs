using Core.DTOs;
using Core.DTOs.Serigrafia;
using Core.DTOs.Serigrafia.ClaseRespuesta;
using Core.DTOs.Serigrafia.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IWMSSerigrafiaRepository
    {
        public Task<List<ConsultaLote>> GetLotesAsync();
        public Task<List<MateriaPrimaPorOpDTO>> GetMateriaPrimaPorOpAsync(string lote);
        public Task<List<OpPorBaseDTO>> GetOpsPorBaseAsync(string ItemId, string lote);
        public Task<string> CreaOpsPreparadasAsync(string ItemId, string lote, ConsolidadoOpsPorColorDTO consolidadoPorColorPrerarado);

        public Task<List<OpPorBaseDTO>> GetOpsPrepardasAsync(string ItemId);

        public Task<string> GestionarOPBaseLocal(OpPorBaseDTO opPorBaseDTO, string ItemBase, int stToUpdate); 
        public Task<bool> ExisteOpEnBaseLocal(string orden);

        public Task<List<ArticulosDisponiblesTraslado>> GetArticulosPisponiblesParaTraslado(string loteId);

        public Task<List<LineasTrasladoDTO>> GetLineasDeTraslado(string ItemId);
        public Task<string> CrearTrasladoBaseLocal(TrasladoDTO trasladoDTO, string transferId);
        public Task<List<TrasladoDespachoDTO>> GetTrasladoPorLote(string LoteId);
        public Task<List<TrasladoDespachoDTO>> GetTrasladoPorLoteGeneral(string LoteId);
        public Task<List<DiariosAbiertosDTO>> GetDiariosAbiertosAsync(string userId, string diarioId);
        public Task<List<IM_WMS_SRG_TipoDiario>> GetTiposDiario();
        public Task<string> CrearDespacho(IM_WMS_SRG_Despacho despacho);
        public Task<string> AgregarTrasladoDespacho(string DespachoId, TrasladoDespachoDTO trasladoDespacho);
        public Task<string> EliminarTrasladoDespacho(TrasladoDespachoDTO trasladoDespacho);
        public Task<List<IM_WMS_SRG_Despacho>> GetDespachosByBatchId(string batchId, int tipo);
        public Task<List<IM_WMS_SRG_Despacho_Lines_Packing>> GetDespachoLinesByIdAEnviar(string despachoId);
        public Task<List<TrasladoDespachoDTO>> GetDespachoTrasladosById(string despachoId);
        public Task<int> SetPackingAsync(PackingRequestDTO requestDTO);
        public Task<string> ChangeEstadoTraslado(int despachoId, string trasladoId);
        public Task<List<IM_WMS_SRG_Despacho_Lines_Packing>> GetDespachoLinesByIdARecibir(string despachoId);
        public Task<int> SetReceiveAsync(PackingRequestDTO packingRequestDTO);

        public Task<Respuesta<List<IM_WMS_SRG_ArticulosGenericosSegundas>>> GetArticulosGenericoSegundas(string itemId);

        public Task<List<IM_WMS_SRG_UsuarioAccion>> GetUsuariosPorAccion(string accion);

    }
}

using Core.DTOs.IM_PrepEnvOp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IIM_PrepEnvOpRepository
    {
        Task<ListadoDeOpResponseDTO> GetListadoDeOp(DateTime fechaInicioSemana, DateTime fechaFinSemana, string? area);
        Task<IM_PrepEnvOp_UpdateOpPreparadaDTO> UpdateOpPreparada(int idOpPreparada,  string userCode);
        Task<List<IM_PrepEnvOp_CorreosHabilitadosDTO>> CorreosHabilitados();
        Task<List<IM_PrepEnvOp_ListaOpPorEnviarDTO>> ListaOpPorEnviar(DateTime fechaInicioSemana, DateTime fechaFinSemana);
        Task<IM_PrepEnvOp_PostDetalleOpEnviadaDTO> PostDetalleOpEnviada(PostDetalleOpEnviadaResponseDTO response);
        Task<string> PostPrintEtiquetasMateriales(List<ArticuloDTO> data, string? ipImpresora);
        Task<string> PostPrintEtiquetasEnvio(List<IM_PrepEnvOp_OpEtiqueta> data, string? ipImpresora);
        Task<List<IM_PrepEnvOp_ListaDeTrasladosPorOpDTO>> ListaDeTrasladosPorOp(string ordenTrabajo, List<string>? areas);
        Task<List<IM_PrepEnvOp_UpdateOpPreparadaLikeEmpaquetadaDTO>> UpdateOpPreparadaEmpaquetada(string ordenTrabajo, string traslado, string userCode);
        Task<int> CantidadEstimadaPorOP(string ordenTrabajo);
        Task<List<IM_PrepEnvOp_ListadoDeOpConDetalleDTO>> HistoricoOpPreparada(DateTime fechaInicioSemana, DateTime fechaFinSemana, string? area);
        Task<IM_PrepEnvOp_FirmaBase64> FirmaByIdDetalle(int idDetalleOpEnviada);
        Task<List<IM_PrepEnvOp_ListadoDeOpConDetalleDTO>> HistoricoByOp(string ordenDeProduccion);
        Task<List<IM_PrepEnvOp_ListadoDeOpDTO>> SyncListadoDeOp(DateTime fechaInicioSemana, DateTime fechaFinSemana);


        }
}

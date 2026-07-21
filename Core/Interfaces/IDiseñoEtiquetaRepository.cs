using Core.DTOs.ClaseRespuesta;
using Core.DTOs.DiseñoEtiqueta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IDiseñoEtiquetaRepository
    {
        public Task<Respuesta<bool>> ImprimirPrueba(SolicitudImpresionDto request);
        public Task<Respuesta<bool>> GuardarDiseño(List<ElementoEtiquetaDto> elementos);
        public Task<Respuesta<List<ElementoEtiquetaDto>>> ObtenerDiseño(string codigoDiseño = "UBICACION_RACK");

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.IM_WMS_RecTela
{
    public record ParamsTelasFilterDTO
    (
        string? JournalId = null,
        string? Color = null,
        string? InventBatchId = null,
        string? InventSerialId = null,
        bool? IsScanning = null,
        int? TelaPickingDefectoId = null,
        string? Reference = null,
        string? ubicacion = null,
        string? vendRoll = null,
        int PageNumber = 1,
        int PageSize = 10
    );
}

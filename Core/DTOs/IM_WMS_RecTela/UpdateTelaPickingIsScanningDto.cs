using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.IM_WMS_RecTela
{
    public record UpdateTelaPickingIsScanningDto(
        string userCode,
        string vendroll,
        string journalId,
        string inventSerialId,
        string location,
        int? TelaPickingDefectoId
    );
}

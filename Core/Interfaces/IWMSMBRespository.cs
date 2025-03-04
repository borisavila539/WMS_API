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
    }
}

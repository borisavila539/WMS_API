using Core.DTOs.Despacho_PT.GenerarDespachoPorImportacionExcel;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IIM_WMS_DespachoPTRepository
    {
        public  Task<DespachoImportResultDto> ProcesarYGuardarDespachoExcel(IFormFile file, int userCreated);
        public Task<DespachoImportResultDto> ObtenerDespachoPorId(int despachoID);


    }
}

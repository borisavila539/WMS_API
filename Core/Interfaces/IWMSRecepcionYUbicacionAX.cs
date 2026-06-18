using Core.DTOs.RecepcionYUbicacionAX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IWMSRecepcionYUbicacionAX
    {
        public Task<List<TrasladoAxDto>> GetTrasladosAXAsync();
        public Task<List<ReporteInformacionAXTrasladoDto>> GetReporteInformacionAXTrasladoAsync(string idTraslado);
        public Task<InformacionEmpresa> GetUbicacionEmpresa(string nombre);
    }
}

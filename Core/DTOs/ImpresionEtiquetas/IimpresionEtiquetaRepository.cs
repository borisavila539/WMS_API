using Core.DTOs.RecepcionYUbicacionAX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.ImpresionEtiquetas
{
    public interface IimpresionEtiquetaRepository
    {
        public  Task<List<DatosEtiqueta>> GetItemsAsyncById2Async(string cmpCode,string saleOrderId,string routeId);
        public Task<List<DatosEtiqueta>> ExecuteLoadEtiquetasItemsCommand2Refactored(
        string receiveEmpresa,
        string listaEmpaque,
        string pedido,
        string ipPrinter);
    }
}

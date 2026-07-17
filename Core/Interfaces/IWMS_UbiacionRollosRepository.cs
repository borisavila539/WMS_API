using Core.DTOs;
using Core.DTOs.UbiacacionRollos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IWMS_UbiacionRollosRepository
    {
        public Task<RespuestaExistenciaUbicacion> GetExistenciaUbiacion(string ubicacion, string almacen);
        public Task<RespuestaConsultarRollo> GetRolloParaCambioDeUbiacion(string codigoBarra);
        public Task<List<InventarioRolloPorUbiacionAlmacenDto>> GetConsultarRollosPorUbicacion(string almacen, string ubicacion);
        public Task<List<InventarioRolloPorAlmacenDto>> ConsultarInventarioRollosPorAlmacen(string almacen);


    }
}

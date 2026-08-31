using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.GeneracionPrecios
{
    // Coincide exactamente con las columnas Exito/Mensaje/Id que devuelven
    // IM_WMS_TallasConfiguracionPrecio_Insertar/_Actualizar/_Eliminar.
    // El mapeador de ExecuteProcedure empareja por nombre de propiedad, así que
    // los nombres deben ser idénticos a los alias del SELECT del procedimiento.
    public class RespuestaSP
    {
        public bool Exito { get; set; }
        public string Mensaje { get; set; }
        public int Id { get; set; }
    }
}

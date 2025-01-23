using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Devoluciones
{
    public class IM_WMS_Devolucion_Detalle_RecibirPlanta
    {
        public int ID { get; set; }
        public int IdDevolucion { get; set; }
        public string Articulo { get; set; }
        public string Talla { get; set; }
        public string Color { get; set; }
        public int Cantidad { get; set; }
        public int RecibidaPlanta { get; set; }
        public string NombreArticulo { get; set; }
        public string UsuarioRecepcionPlanta { get; set; }
        public string UsuarioRecepcionCD { get; set; }
        public int RecibidaCD { get; set; }
        public string ITEMBARCODE { get; set; }
        public DefectosDevolucion[] Defecto { get; set; }
    }

    public class DefectosDevolucion
    {
        public int Id { get; set; }
        public int IDDevolucionDetalle { get; set; }
        public string Defecto { get; set; }
        public string Tipo { get; set; }
        public bool Reparacion { get; set; }
        public string Area { get; set; }
        public string Operacion { get; set; }





    }
}

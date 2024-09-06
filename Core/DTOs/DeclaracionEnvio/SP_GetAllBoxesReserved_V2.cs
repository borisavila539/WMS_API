using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.DeclaracionEnvio
{
    public class SP_GetAllBoxesReserved_V2
    {
        public string Caja { get; set; }
        public string Pais { get; set; }
        public string CuentaCliente { get; set; }
        public string NombreCliente { get; set; }
        public string PedidoVenta { get; set; }
        public string ListaEmpaque { get; set; }
        public string Albaran { get; set; }
        public int NumeroCaja { get; set; }
        public int cantidad { get; set; }
        public string Empacador { get; set; }
        public string ubicacion { get; set; }
        public DateTime fecha { get; set; }
        public string factura { get; set; }
        public string calle { get; set; }
        public string ciudad { get; set; }
        public string departamento { get; set; }
        public int paginas { get; set; }
        public int Cajas { get; set; }
        public int Unidades { get; set; }



    }
}

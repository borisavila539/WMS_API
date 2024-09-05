using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.DeclaracionEnvio
{
    public class FiltroDeclaracionEnvio
    {
        public string Caja { get; set; }
        public string Pais { get; set; }
        public string CuentaCliente { get; set; }
        public string NombreCliente { get; set; }
        public string PedidoVenta { get; set; }
        public string ListaEmpaque { get; set; }
        public string Albaran { get; set; }
        public string Ubicacion { get; set; }
        public string Factura { get; set; }
        public int Page { get; set; }
        public int Size { get; set; }
    }
}

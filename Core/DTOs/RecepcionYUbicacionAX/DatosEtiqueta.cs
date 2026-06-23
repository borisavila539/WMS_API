using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.RecepcionYUbicacionAX
{
    public class DatosEtiqueta
    {
        public string Pedido { get; set; }
        public string PedidoIntegrado { get; set; }
        public string OrdenCompra { get; set; } = string.Empty;
        public string ListaEmpaque { get; set; }

        public string ClienteNombre { get; set; }
        public string ClienteCuenta { get; set; }
        public string Empresa { get; set; }

        public string Direccion { get; set; }
        public string Departamento { get; set; }
        public string Pais { get; set; }
        public string Ciudad { get; set; }

        public string Asesor { get; set; }
        public string TelefonoCliente { get; set; }

        public string NumeroCaja { get; set; }
        public int NumeroCajaLocal { get; set; }

        public string ItemIdOriginal { get; set; }
        public string ItemId { get; set; }

        public string Size { get; set; }
        public string Color { get; set; }
        public string ColorDescription { get; set; }

        public int Cantidad { get; set; }

        public string Marca { get; set; }
    }
}

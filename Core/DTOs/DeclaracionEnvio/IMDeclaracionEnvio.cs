using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.DeclaracionEnvio
{
    public class IMDeclaracionEnvio
    {
        public string Departamento { get; set; }
        public string Ciudad { get; set; }
        public string NombreCliente { get; set; }
        public string Albaran { get; set; }
        public string Factura { get; set; }
        public string ListaEmpaque { get; set; }
        public int Cajas { get; set; }
        public int Unidades { get; set; }

    }
}

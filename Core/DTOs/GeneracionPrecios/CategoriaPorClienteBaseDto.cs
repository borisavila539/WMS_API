using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.GeneracionPrecios
{
    public class CategoriaPorClienteBaseDto
    {
        public string CuentaCliente { get; set; }
        public string Base { get; set; }
        public string Categoria { get; set; }
        public string SubCategoria { get; set; }
        public string Coleccion { get; set; }
    }
}

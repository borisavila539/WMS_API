using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.GeneracionPrecios
{
    public class ImpresionEtiqueta
    {
        public bool EsGeneracionLibre { get; set; }
        public string CodigoArticulo { get; set; } 
        public string Talla {  get; set; } 
        public string Color { get; set; } 
        public string CantidadImprimir { get; set; } 
        public string Pedido { get; set; } = string.Empty;
        public string Ruta { get; set; } = string.Empty ;
        public string Caja { get; set; }
        public string Fecha { get; set; }
        public string Impresora { get; set; }

        public ImpresionEtiqueta Normalizar(ImpresionEtiqueta parms)
        {
            parms.Pedido = parms.Pedido == null ? "" : parms.Pedido;
            parms.Ruta = parms.Ruta == null ? "" : parms.Ruta;
            parms.Caja = parms.Caja == null ? "" : parms.Caja;
            parms.CodigoArticulo = parms.CodigoArticulo == null ? "" : parms.CodigoArticulo;
            parms.Talla = parms.Talla == null ? "" : parms.Talla;
            parms.Color = parms.Color == null ? "" : parms.Color;
            parms.Impresora = parms.Impresora == null ? null : parms.Impresora;
            parms.Fecha = parms.Fecha == null ? "-" : parms.Fecha; 
            return parms;
        }
    }

}

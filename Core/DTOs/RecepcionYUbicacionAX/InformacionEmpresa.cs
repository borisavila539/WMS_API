using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.RecepcionYUbicacionAX
{
    public class InformacionEmpresa
    {
        public string Ubicacion { get; set; }
        public string UbicacionIrregular { get; set; }
        public string UbicacionTercera { get; set; }
        public string LOTEIRREGULAR { get; set; }
        public string LOTETERCERA { get; set; }
        public string ALMACEN { get; set; }
    }
}

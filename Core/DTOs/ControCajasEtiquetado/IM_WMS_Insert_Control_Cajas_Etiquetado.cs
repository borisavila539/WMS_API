using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.ControCajasEtiquetado
{
    public class IM_WMS_Insert_Control_Cajas_Etiquetado
    {
        public int ID { get; set; }
        public string BoxNum { get; set; }
        public string Employee { get; set; }
        public DateTime InitDate { get; set; }
        public DateTime finalDate { get; set; }

    }
}

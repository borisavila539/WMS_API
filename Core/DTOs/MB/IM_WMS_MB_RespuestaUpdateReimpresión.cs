using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.MB
{
    public class IM_WMS_MB_RespuestaUpdateReimpresión
    {
        public int Cantidad {  get; set; }  
        public int CantidadOriginal { get; set; }
        public int ReimpresionNum {  get; set; }
    }
}

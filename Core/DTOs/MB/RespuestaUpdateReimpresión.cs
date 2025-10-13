using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.MB
{
    public class RespuestaUpdateReimpresión
    {
        public int Cantidad {  get; set; }  
        public int CantidadOriginal { get; set; }
        public int ReimpresionNum {  get; set; }
    }
}

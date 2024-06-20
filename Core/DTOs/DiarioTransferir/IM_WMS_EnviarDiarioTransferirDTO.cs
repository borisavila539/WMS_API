using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.DiarioTransferir
{
    public class IM_WMS_EnviarDiarioTransferirDTO
    {
        public int ID { get; set; }
        public string JournalID { get; set; }
        public string userID { get; set; }
        public DateTime Fecha { get; set; }
    }
}

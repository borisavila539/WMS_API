using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.DiarioTransferir
{
    public class IM_Encabezado_Diario_TransferirDTO
    {
        public string JOURNALID { get; set; }
        public string INVENTLOCATIONID { get; set; }
        public string IM_INVENTLOCATIONID_TO { get; set; }

        public string PERSONNELNUMBER { get; set; }

    }
}

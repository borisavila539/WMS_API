using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Despacho_PT.GenerarDespachoPorImportacionExcel
{
    public class DespachoImportResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public long DespachoID { get; set; }
        public DespachoCabeceraDto Cabecera { get; set; }
        public List<DespachoDetalleDto> Detalle { get; set; } = new List<DespachoDetalleDto>();
        public List<EstatusUnidadesDto> EstatusOP { get; set; } = new List<EstatusUnidadesDto>();
    }

    public class DespachoCabeceraDto
    {
        public long Id { get; set; }
        public string Driver { get; set; }
        public string Truck { get; set; }
        public int EstadoID { get; set; }
        public int UserCreated { get; set; }
        public string CreatedDateTime { get; set; }
        public string Almacen { get; set; }
        public int CajaSegundas { get; set; }
        public int CajasTerceras { get; set; }
    }

    public class DespachoDetalleDto
    {
        public string ProdCutSheetID { get; set; }
        public int Box { get; set; }
        public string Size { get; set; }
        public string Color { get; set; }
        public string ItemID { get; set; }
        public string ProdID { get; set; }
        public int Qty { get; set; }
    }

    public class EstatusUnidadesDto
    {
        public string ProdID { get; set; }
        public string Size { get; set; }
        public int Costura1 { get; set; }
        public int Textil1 { get; set; }
        public int Costura2 { get; set; }
        public int Textil2 { get; set; }
    }
}

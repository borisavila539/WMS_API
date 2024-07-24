using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Despacho_PT
{
    public class IM_WMS_Detalle_Despacho_Excel
    {
        public string InventLocation { get; set; }
        public string Base { get; set; }
        public string ItemID { get; set; }
        public string Nombre {get;set;}
        public string Tl { get; set; }
        public string Color {get;set;}
        public string Size {get;set;}
        public string ProdID {get;set;}
        public string InventRefId {get;set;}
        public int Planificado {get;set;}
        public int Cortado {get;set;}
        public int Primeras {get;set;}
        public int Costura1 {get;set;}
        public int Textil1 { get; set; }
        public int Costura2 { get; set; }
        public int Textil2 { get; set; }
        public int TotalUnidades { get; set; }
        public int DifPrdVrsPlan { get; set; }
        public int DifCortVrsExport { get; set; }
        public decimal PorCostura { get; set; }
        public decimal PorTextil { get; set; }
        public decimal Irregulares1PorcCostura { get; set; }
        public decimal IrregularesCobrarCostura { get; set; }
        public decimal Irregulares1PorcTextil { get; set; }
        public decimal IrregularesCobrarTextil { get; set; }
        public int Cajas { get; set; }
        public int CajasSegundas { get; set; }

        public int CajasTerceras { get; set; }

        public decimal TotalDocenas { get; set; }
        public string Programa { get; set; }

    }
}

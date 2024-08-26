using Core.DTOs.InventarioCiclicoTela;
using System.Collections.Generic;

namespace Core.Interfaces
{
    public interface IAX
    {
        public string InsertDeleteMovimientoLine(string JOURNALID, string ITEMBARCODE, string PROCESO, string IMBOXCODE);
        public string EnviarRecibirTraslados(string TRANSFERID, string ESTADO);
        public string INsertDeleteReduccionCajas(string ITEMBARCODE, string PROCESO, string IMBOXCODE);
        public string InsertDeleteEntradaMovimientoLine(string JOURNALID, string ITEMBARCODE, string PROCESO);
        public string InsertDeleteTransferirMovimientoLine(string JOURNALID, string ITEMBARCODE, string PROCESO);
        public string InsertAddInventarioCiclicoTelaLine(List<INVENTARIOCICLICOTELALINE> LIST);
    }
}

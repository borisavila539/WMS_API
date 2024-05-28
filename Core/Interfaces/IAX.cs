namespace Core.Interfaces
{
    public interface IAX
    {
        public string InsertDeleteMovimientoLine(string JOURNALID, string ITEMBARCODE, string PROCESO, string IMBOXCODE);
        public string EnviarRecibirTraslados(string TRANSFERID, string ESTADO);
        public string INsertDeleteReduccionCajas(string ITEMBARCODE, string PROCESO, string IMBOXCODE);

    }
}

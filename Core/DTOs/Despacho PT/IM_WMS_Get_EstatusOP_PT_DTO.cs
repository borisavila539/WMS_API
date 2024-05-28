namespace Core.DTOs.Despacho_PT
{
    public class IM_WMS_Get_EstatusOP_PT_DTO
    {
        public string UserPicking { get; set; }
        public string Prodcutsheetid { get; set; }
        public string prodid { get; set; }
        public string Size { get; set; }
        public int Escaneado { get; set; }
        public int Cortado { get; set; }
        public string Costura1 { get; set; }
        public string Textil1 { get; set; }
        public string Costura2 { get; set; }
        public string Textil2 { get; set; }
    }
}

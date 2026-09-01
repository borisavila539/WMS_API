namespace Core.DTOs.UbiacacionRollos
{
    public class DIARIO_MOVIMIENTO_DELETE_LINE
    {
        public DeleteLineJournalData COMPANY { get; set; }
    }

    public class DeleteLineJournalData
    {
        public string CODE { get; set; }
        public string JOURNALID { get; set; }
        public string LINENUM { get; set; }
    }
}

using Core.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IWMSRepository
    {
        public Task<LoginDTO> PostLogin(LoginDTO datos);
        public Task<List<DiariosAbiertosDTO>> GetDiariosAbiertos(string user, string filtro);
        public Task<List<LineasDTO>> GetLineasDiario(string diario);
        public Task<List<EtiquetaDTO>> GetDatosEtiquetaMovimiento(string diario, string IMBoxCode);
        public Task<string> GetImprimirEtiquetaMovimiento(string diario, string IMBoxCode,string PRINT);
        public Task<List<ImpresoraDTO>> getImpresoras();


    }
}

using Core.DTOs.Serigrafia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IWMSSerigrafiaRepository
    {
        public Task<List<MateriaPrimaPorOpDTO>> GetMateriaPrimaPorOpAsync();
        public Task<List<OpPorBaseDTO>> GetOpsPorBaseAsync(string ItemId);
        public Task<List<ConsolidadoOpsPorColorDTO>> GetConsolidadoOpsPorColorAsync(string ItemId);
        public Task<List<OpPorBaseDTO>> CreaOpsPreparadasAsync(string ItemId, ConsolidadoOpsPorColorDTO consolidadoPorColorPrerarado);

        public Task<List<OpPorBaseDTO>> GetOpsPrepardasAsync(string ItemId);
    }
}

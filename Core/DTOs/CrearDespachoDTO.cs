using System;

namespace Core.DTOs
{
    public class CrearDespachoDTO
    {
        public int ID { get; set; }
        public string RecIDTraslados { get; set; }
        public string chofer { get; set; }
        public string camion { get; set; }
        public Boolean Estado { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Serigrafia.ClaseRespuesta
{
    public class Respuesta<T>
    {
        public bool Exito { get; set; }
        public string Mensaje { get; set; }
        public T Datos { get; set; }

        public Respuesta() { }

        public Respuesta(bool exito, string mensaje, T datos)
        {
            Exito = exito;
            Mensaje = mensaje;
            Datos = datos;
        }

        public static Respuesta<T> Ok(T datos, string mensaje = "Operación exitosa")
        {
            return new Respuesta<T>(true, mensaje, datos);
        }

        public static Respuesta<T> Error(string mensaje)
        {
            return new Respuesta<T>(false, mensaje, default);
        }
    }

}

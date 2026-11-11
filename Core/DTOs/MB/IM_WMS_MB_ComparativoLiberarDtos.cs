using System;
using System.Collections.Generic;

namespace Core.DTOs.MB
{
    /// <summary>Fila del comparativo MB vs AX (almacen 22, ubicacion MB-CAJAS) por articulo + talla + color.</summary>
    public class IM_WMS_MB_ComparativoAX
    {
        public string Articulo { get; set; }
        public string Descripcion { get; set; }
        public string Talla { get; set; }
        public string Color { get; set; }
        public string NombreColor { get; set; }

        // Seccion disponible: MB sin despacho vs AX disponible fisico
        public int MBSinDespacho { get; set; }
        public int CajasSinDespacho { get; set; }
        public int AXDisponible { get; set; }
        public int DiferenciaDisponible { get; set; }
        public string EstadoDisponible { get; set; }

        // Seccion despacho: MB en despacho no enviado vs AX en pedido
        public int MBEnDespacho { get; set; }
        public int CajasEnDespacho { get; set; }
        public int AXEnPedido { get; set; }
        public int DiferenciaDespacho { get; set; }
        public string EstadoDespacho { get; set; }
    }

    public class IM_WMS_MB_LiberarPermiso
    {
        public bool PuedeLiberar { get; set; }
    }

    public class IM_WMS_MB_LiberarDespacho
    {
        public int DespachoID { get; set; }
        public string UsuarioCreacion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public int CajasPrincipales { get; set; }
        public int Cajas { get; set; }
        public int Unidades { get; set; }
        public int CajasPicking { get; set; }
        public int CajasPacking { get; set; }
    }

    /// <summary>Despacho consultado por numero (enviado o no). Fecha como texto: puede venir vacia.</summary>
    public class IM_WMS_MB_LiberarDespachoInfo
    {
        public int DespachoID { get; set; }
        public string UsuarioCreacion { get; set; }
        public string FechaCreacion { get; set; }
        public int CajasPrincipales { get; set; }
        public int Cajas { get; set; }
        public int Unidades { get; set; }
        public int CajasPicking { get; set; }
        public int CajasPacking { get; set; }
        public bool Enviado { get; set; }
    }

    /// <summary>Caja principal de un despacho; las consolidadas se suman a ella.</summary>
    public class IM_WMS_MB_LiberarCaja
    {
        public int ID { get; set; }
        public string Orden { get; set; }
        public string Lote { get; set; }
        public int NumeroCaja { get; set; }
        public string Articulo { get; set; }
        public string Descripcion { get; set; }
        public string Talla { get; set; }
        public string Color { get; set; }
        public string NombreColor { get; set; }
        public int CantidadTotal { get; set; }
        public int CantidadCajas { get; set; }
        public string CajasConsolidadas { get; set; }
        public bool Picking { get; set; }
        public string UserPicking { get; set; }
        public string FechaPicking { get; set; }
        public bool Packing { get; set; }
        public string UserPacking { get; set; }
        public string FechaPacking { get; set; }
        public string Pallet { get; set; }
        public string UbicacionRecepcion { get; set; }
    }

    public class IM_WMS_MB_LiberarCajasRequest
    {
        public int DespachoID { get; set; }
        public List<int> Ids { get; set; }
        public string Usuario { get; set; }
        public string Motivo { get; set; }
    }

    public class IM_WMS_MB_LiberarCajasResultado
    {
        public bool Ok { get; set; }
        public string Mensaje { get; set; }
        public int CajasLiberadas { get; set; }
        public int Unidades { get; set; }
    }

    public class IM_WMS_MB_LiberarHistorial
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public string Usuario { get; set; }
        public string Motivo { get; set; }
        public int DespachoID { get; set; }
        public int CajaID { get; set; }
        public int IDConsolidado { get; set; }
        public string Orden { get; set; }
        public int NumeroCaja { get; set; }
        public string Articulo { get; set; }
        public string Talla { get; set; }
        public string Color { get; set; }
        public int Cantidad { get; set; }
        public bool Picking { get; set; }
        public bool Packing { get; set; }
        public string Pallet { get; set; }
    }
}

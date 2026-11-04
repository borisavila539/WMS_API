using System;
using System.Collections.Generic;

namespace Core.DTOs.AuditoriaInterna
{
    // Respuesta de IM_WMS_AuditoriaInterna_AbrirCaja y _CerrarCaja.
    public class AuditoriaInternaRespuestaDto
    {
        public bool Exito { get; set; }
        public string Resultado { get; set; }
        public string Mensaje { get; set; }
        public int Id { get; set; }
    }

    // Columnas de IM_WMS_AuditoriaInterna_ObtenerCaja.
    public class AuditoriaInternaCajaDto
    {
        public int Id { get; set; }
        public string OP { get; set; }
        public int NumeroCaja { get; set; }
        public int? BoxIdReintegrado { get; set; }
        public string QRCaja { get; set; }
        public string Estado { get; set; }
        public string Resultado { get; set; }
        public string ModoValidacion { get; set; }
        public string Conjunto { get; set; }
        public string Origen { get; set; }
        public int? CompanyId { get; set; }
        public string Contratista { get; set; }
        public string CategoriaInterna { get; set; }
        public string Articulo { get; set; }
        public string NombreArticulo { get; set; }
        public string ProductoEtiqueta { get; set; }
        public string Estilo { get; set; }
        public string DescripcionMB { get; set; }
        public string Lote { get; set; }
        public string ColorId { get; set; }
        public string ColorDescripcion { get; set; }
        public string CategoriaCaja { get; set; }
        public string SubCategoriaCaja { get; set; }
        public string Proveedor { get; set; }
        public string PaisOrigen { get; set; }
        public string Dimensiones { get; set; }
        public string PesoBruto { get; set; }
        public string CodigoBarrasCaja { get; set; }
        public string EmpacadoPor { get; set; }
        public DateTime? FechaCierreEmpaque { get; set; }
        public int? DespachoId { get; set; }
        public string PLDespacho { get; set; }
        public DateTime? FechaDespacho { get; set; }
        public int CantidadEsperada { get; set; }
        public int CantidadEscaneada { get; set; }
        public int Diferencia { get; set; }
        public int Faltantes { get; set; }
        public int Sobrantes { get; set; }
        public int Escaneos { get; set; }
        public int Rechazos { get; set; }
        public string Observaciones { get; set; }
        public string UsuarioInicio { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaUltimoEscaneo { get; set; }
        public string UsuarioCierre { get; set; }
        public DateTime? FechaCierre { get; set; }
    }

    // Columnas de IM_WMS_AuditoriaInterna_ObtenerLineas.
    public class AuditoriaInternaLineaDto
    {
        public int Id { get; set; }
        public string EAN { get; set; }
        public string Talla { get; set; }
        public int CantidadEsperada { get; set; }
        public int CantidadEscaneada { get; set; }
        public int Diferencia { get; set; }
        public int Faltantes { get; set; }
        public int Sobrantes { get; set; }
        public int Escaneos { get; set; }
    }

    // Columnas de IM_WMS_AuditoriaInterna_EscanearPieza.
    public class AuditoriaInternaEscaneoDto
    {
        public bool Exito { get; set; }
        public string Resultado { get; set; }
        public string Mensaje { get; set; }
        public string Talla { get; set; }
        public int Cantidad { get; set; }
        public int LineaEsperada { get; set; }
        public int LineaEscaneada { get; set; }
        public int CajaEsperada { get; set; }
        public int CajaEscaneada { get; set; }
        public string OPSugerida { get; set; }
        public int? CajaSugerida { get; set; }
    }

    // Columnas de IM_WMS_AuditoriaInterna_Pendientes.
    public class AuditoriaInternaPendienteDto
    {
        public int Id { get; set; }
        public string OP { get; set; }
        public int NumeroCaja { get; set; }
        public string Articulo { get; set; }
        public string Estilo { get; set; }
        public string ColorDescripcion { get; set; }
        public string Origen { get; set; }
        public string Contratista { get; set; }
        public string CategoriaInterna { get; set; }
        public string ModoValidacion { get; set; }
        public int CantidadEsperada { get; set; }
        public int CantidadEscaneada { get; set; }
        public int Diferencia { get; set; }
        public string PLDespacho { get; set; }
        public string UsuarioInicio { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaUltimoEscaneo { get; set; }
    }

    // Columnas de IM_WMS_AuditoriaInterna_ObtenerFaltantes.
    public class AuditoriaInternaFaltanteDto
    {
        public long Id { get; set; }
        public string Talla { get; set; }
        public string EAN { get; set; }
        public string Serial { get; set; }
        public int Cantidad { get; set; }
        public string CodigoEmpaque { get; set; }
    }

    // Columnas de IM_WMS_AuditoriaInterna_ObtenerEscaneos (aceptados y rechazados).
    public class AuditoriaInternaHistorialEscaneoDto
    {
        public long Id { get; set; }
        public string Tipo { get; set; }
        public string Talla { get; set; }
        public string EAN { get; set; }
        public string Serial { get; set; }
        public int Cantidad { get; set; }
        public bool EsSobrante { get; set; }
        public string Codigo { get; set; }
        public string Mensaje { get; set; }
        public string Usuario { get; set; }
        public DateTime Fecha { get; set; }
    }

    // Columnas de IM_WMS_AuditoriaInterna_ObtenerHistorial (cierres y reaperturas).
    public class AuditoriaInternaHistorialDto
    {
        public long Id { get; set; }
        public string Accion { get; set; }
        public string Resultado { get; set; }
        public int CantidadEsperada { get; set; }
        public int CantidadEscaneada { get; set; }
        public int Faltantes { get; set; }
        public int Sobrantes { get; set; }
        public string Comentario { get; set; }
        public string Usuario { get; set; }
        public DateTime Fecha { get; set; }
    }

    // Columnas de IM_WMS_AuditoriaInterna_PermisoReabrir.
    public class AuditoriaInternaPermisoDto
    {
        public bool PuedeReabrir { get; set; }
    }

    // Lo que necesita la pantalla de la app despues de abrir la caja.
    public class AuditoriaInternaAbrirCajaResponse
    {
        public AuditoriaInternaRespuestaDto Respuesta { get; set; }
        public AuditoriaInternaCajaDto Caja { get; set; }
        public List<AuditoriaInternaLineaDto> Lineas { get; set; } = new List<AuditoriaInternaLineaDto>();
    }

    // Lo que necesita la pantalla de la app despues de cada escaneo.
    public class AuditoriaInternaEscanearResponse
    {
        public AuditoriaInternaEscaneoDto Escaneo { get; set; }
        public List<AuditoriaInternaLineaDto> Lineas { get; set; } = new List<AuditoriaInternaLineaDto>();
    }
}

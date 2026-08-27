using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.MB
{
    public class IM_WMS_MB_ReimpresionEtiqueta
    {
        public string WorkOrderId { get; set; }
        public string ProductId { get; set; }
        public string Style { get; set; }
        public string ProductName { get; set; }
        public string ProductNameMB { get; set; }         
        public string ProductCategoryDesc { get; set; }   
        public string Correlative { get; set; }
        public string AnoCorrelative { get; set; }
        public string BaseCode { get; set; }
        public string BatchId { get; set; }              
        public string BatchDescription { get; set; }
        public string ColorId { get; set; }
        public string ColorDescription { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string BoxNum { get; set; }                 
        public int BoxCategoryId { get; set; }
        public int BoxSubCategoryId { get; set; }
        public string BoxCategoryDescription { get; set; } 
        public string BoxSubCategory { get; set; }        
        public DateTime DateClosed { get; set; }
        public string Barcode { get; set; }                
        public string UserName { get; set; }
        public string Size { get; set; }     
        public string Qty { get; set; }
        public string SizeXLarge { get; set; }          
        public string SKUMB { get; set; }
        public decimal PESONETO { get; set; }
        public string DescripcionCompleta { get; set; }
        public string UbicacionCompleta { get; set; }
        public string Header { get; set; }
        public string Footer { get; set; }
        public string DimensionBox { get; set; }
        public string WeightBox { get; set; }
        public int PrintQuantity { get; set; }
        //public bool EsEtiquetaMB =>
        //    !string.IsNullOrWhiteSpace(Header) && Header != "--";
        //public string CategoriaCompleta =>
        //    $"{BoxCategoryDescription} {BoxSubCategory}".Trim();
        //public int Copias => PrintQuantity > 0 ? PrintQuantity : 1;

    }
}

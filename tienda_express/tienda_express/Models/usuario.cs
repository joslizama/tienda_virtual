//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace tienda_express.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class usuario
    {
        public usuario()
        {
            this.venta = new HashSet<venta>();
        }
    
        public string rut { get; set; }
        public string nombre { get; set; }
        public string apellido { get; set; }
        public string direccion { get; set; }
        public string clave { get; set; }
        public Nullable<int> tipo_id { get; set; }
    
        public virtual tipo tipo { get; set; }
        public virtual ICollection<venta> venta { get; set; }
    }
}

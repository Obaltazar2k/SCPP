//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SCPP.DataAcces
{
    using System;
    using System.Collections.ObjectModel;
    
    public partial class Responsableproyecto
    {
        public string Apellidomaterno { get; set; }
        public string Apellidopaterno { get; set; }
        public string Correopersonal { get; set; }
        public string Nombre { get; set; }
        public Nullable<int> OrganizaciónID { get; set; }
        public string Telefono { get; set; }
        public Nullable<int> Activo { get; set; }
        public int ResponsableproyectoID { get; set; }
    
        public virtual Organización Organización { get; set; }
    }
}

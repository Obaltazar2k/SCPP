//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SCPP
{
    using System;
    using System.Collections.Generic;
    
    public partial class Estudiante
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Estudiante()
        {
            this.Inscripción = new HashSet<Inscripción>();
            this.Selecciónproyecto = new HashSet<Selecciónproyecto>();
        }
    
        public string Apellidomaterno { get; set; }
        public string Apellidopaterno { get; set; }
        public string Correopersonal { get; set; }
        public string Estado { get; set; }
        public string Matricula { get; set; }
        public string Nombre { get; set; }
        public Nullable<double> Promedio { get; set; }
        public string Telefono { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Inscripción> Inscripción { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Selecciónproyecto> Selecciónproyecto { get; set; }
    }
}

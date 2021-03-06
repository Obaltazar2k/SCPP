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
    
    public partial class Expediente
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Expediente()
        {
            this.Archivo = new ObservableCollection<Archivo>();
        }
    
        public Nullable<System.DateTime> Fechafinpp { get; set; }
        public Nullable<System.DateTime> Fechainiciopp { get; set; }
        public Nullable<double> Horasacumuladas { get; set; }
        public Nullable<int> Numreportesentregados { get; set; }
        public int ExpedienteID { get; set; }
        public int InscripciónID { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ObservableCollection<Archivo> Archivo { get; set; }
        public virtual Inscripción Inscripción { get; set; }
    }
}

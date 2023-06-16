using System.ComponentModel.DataAnnotations;

namespace Feriados.Models
{
    public enum Tipo : byte
    {
        [Display(Name = "feriado_nacional")]
        Nacional = 0,

        [Display(Name = "feriado_estadual")]
        Estadual = 1,

        [Display(Name = "feriado_municipal")]
        Municipal = 2,

        [Display(Name = "facultativo")]
        Facultativo = 3,

        [Display(Name = "multiplos")]
        Multiplos = 4,

        [Display(Name = "dia_comum")]
        DiaComum = 5,
    }
}

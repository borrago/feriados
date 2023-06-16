namespace Feriados.Models
{
    public class RegiaoImediata
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public RegiaoIntermediaria Regiaointermediaria { get; set; }
    }
}

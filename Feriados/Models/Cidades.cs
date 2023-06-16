namespace Feriados.Models
{
    public class Cidades
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public Microrregiao Microrregiao { get; set; }
        public RegiaoImediata Regiaoimediata { get; set; }
    }
}

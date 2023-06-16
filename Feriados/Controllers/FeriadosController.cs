using Feriados.Models;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Feriados.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FeriadosController : ControllerBase
    {
        [HttpGet("estados")]
        [ProducesResponseType(typeof(IEnumerable<Estados>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetEstadosAsync(CancellationToken cancellationToken)
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri("https://servicodados.ibge.gov.br/api/v1/localidades/estados");
            var result = await client.GetAsync("", cancellationToken);
            var json = await result.Content.ReadAsStringAsync(cancellationToken);

            return new OkObjectResult(JsonConvert.DeserializeObject<IEnumerable<Estados>>(json));
        }

        [HttpGet("cidades")]
        [ProducesResponseType(typeof(IEnumerable<Cidades>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCidadesByEstadoAsync([FromQuery] string uf, CancellationToken cancellationToken)
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri($"https://servicodados.ibge.gov.br/api/v1/localidades/estados/{uf}/municipios");
            var result = await client.GetAsync("", cancellationToken);
            var json = await result.Content.ReadAsStringAsync(cancellationToken);

            return new OkObjectResult(JsonConvert.DeserializeObject<List<Cidades>>(json));
        }

        [HttpGet()]
        [ProducesResponseType(typeof(IEnumerable<Feriado>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFeriadosByCidadesEstadoAsync([FromQuery] GetFeriadosByCidadesEstadoModel request, CancellationToken cancellationToken)
        {
            var url = $"https://www.calendario.com.br/mycalendar/calendar_XxY.php?x=3&y=4&show_today=1&ano={request.Ano}&estado={request.Uf}&cidade={Helpers.Helpers.RemoveAccents(Helpers.Helpers.NormalizarCidades(request.Cidade))}";

            using var client = new HttpClient();
            var html = await client.GetStringAsync(url, cancellationToken);

            var feriados = new List<Feriado>();

            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            AddFeriados(feriados, doc, Tipo.Nacional);
            AddFeriados(feriados, doc, Tipo.Estadual);
            AddFeriados(feriados, doc, Tipo.Municipal);
            AddFeriados(feriados, doc, Tipo.Facultativo);
            AddFeriados(feriados, doc, Tipo.Multiplos);
            AddFeriados(feriados, doc, Tipo.DiaComum);

            return new OkObjectResult(feriados);
        }

        private static void AddFeriados(List<Feriado> feriados, HtmlDocument doc, Tipo tipo)
        {
            var tipoNome = Helpers.Helpers.GetAttributeName(tipo);

            var rows = doc.DocumentNode.SelectNodes($"//td[contains(@class, '{tipoNome}')]");

            if (rows == null) return;

            foreach (var row in rows)
            {
                var cells = row.SelectNodes("div");
                var dataFeriado = Convert.ToDateTime(cells.FirstOrDefault().Id.Replace("div_", ""));
                var outerHtml = cells.FirstOrDefault().OuterHtml.ToString();
                var tag = "<b>";
                var segundaPosicao = outerHtml.IndexOf(tag, outerHtml.IndexOf(tag) + tag.Length + 1) + tag.Length;
                var nomeFeriado = outerHtml.Substring(segundaPosicao, outerHtml.IndexOf("</b>", segundaPosicao) - segundaPosicao);

                if (!feriados.Any(a => a.Data.Equals(dataFeriado)))
                {
                    feriados.Add(new Feriado
                    {
                        Tipo = tipoNome,
                        Data = dataFeriado,
                        Descricao = nomeFeriado,
                    });
                }
            }
        }
    }
}
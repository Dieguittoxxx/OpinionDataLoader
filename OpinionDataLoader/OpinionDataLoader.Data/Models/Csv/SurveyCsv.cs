using CsvHelper.Configuration.Attributes;

namespace OpinionDataLoader.Data.Models.Csv;

public class SurveyCsv
{
    public int IdOpinion { get; set; }
    public int IdCliente { get; set; }
    public int IdProducto { get; set; }
    public string Fecha { get; set; } = null!;
    public string Comentario { get; set; } = null!;

    [Name("Clasificación")]
    public string Clasificacion { get; set; } = null!;

    [Name("PuntajeSatisfacción")]
    public decimal PuntajeSatisfaccion { get; set; }

    public string Fuente { get; set; } = null!;
}
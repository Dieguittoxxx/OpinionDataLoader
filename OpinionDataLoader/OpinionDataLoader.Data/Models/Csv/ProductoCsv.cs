using CsvHelper.Configuration.Attributes;

namespace OpinionDataLoader.Data.Models.Csv;

public class ProductoCsv
{
    public int IdProducto { get; set; }
    public string Nombre { get; set; } = null!;

    [Name("Categoría")]
    public string Categoria { get; set; } = null!;
}
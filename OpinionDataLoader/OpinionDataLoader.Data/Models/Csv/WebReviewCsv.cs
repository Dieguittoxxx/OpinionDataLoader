namespace OpinionDataLoader.Data.Models.Csv;

public class WebReviewCsv
{
    public string IdReview { get; set; } = null!;
    public string IdCliente { get; set; } = null!;
    public string IdProducto { get; set; } = null!;
    public string Fecha { get; set; } = null!;
    public string Comentario { get; set; } = null!;
    public decimal Rating { get; set; }
}
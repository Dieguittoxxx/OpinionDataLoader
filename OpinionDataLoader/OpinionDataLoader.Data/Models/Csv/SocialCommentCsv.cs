namespace OpinionDataLoader.Data.Models.Csv;

public class SocialCommentCsv
{
    public string IdComment { get; set; } = null!;
    public string IdCliente { get; set; } = null!;
    public string IdProducto { get; set; } = null!;
    public string Fuente { get; set; } = null!;
    public string Fecha { get; set; } = null!;
    public string Comentario { get; set; } = null!;
}
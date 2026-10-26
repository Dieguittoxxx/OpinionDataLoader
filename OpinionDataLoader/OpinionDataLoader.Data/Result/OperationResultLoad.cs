#nullable disable
namespace OpinionDataLoader.Data.Result
{
    public class OperationResultLoad : OperationResult
    {
        public int TotalLeidos { get; set; }
        public int TotalRechazadosPreSp { get; set; }
        public int TotalEnviados { get; set; }
        public int TotalInsertados { get; set; }
        public int TotalYaExistian { get; set; }
        public int TotalRechazados { get; set; }
    }
}
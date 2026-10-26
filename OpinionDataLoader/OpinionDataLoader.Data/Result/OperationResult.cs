#nullable disable
namespace OpinionDataLoader.Data.Result
{
    public class OperationResult
    {
        public OperationResult()
        {
            this.IsSuccess = true;
        }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public CodigoResultado Codigo { get; set; }
    }

    public class OperationResult<T> : OperationResult
    {
        public T Data { get; set; }
    }
}
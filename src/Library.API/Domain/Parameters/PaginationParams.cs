namespace Library.API.Domain.Parameters
{
    public class PaginationParams : BasePaginationParams
    {
        public string Term { get; set; }

        public PaginationParams() : base()
        {
            this.Term = string.Empty;
        }
    }
}

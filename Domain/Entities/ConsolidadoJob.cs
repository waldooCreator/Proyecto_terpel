namespace Domain.Entities
{
    public class ConsolidadoJob
    {
        public int Id { get; set; }
        public int EdsId { get; set; }
        public int TotalArchivos { get; set; }
        public int UrlsGeneradas { get; set; }
        public int ExpiracionMinutos { get; set; }
        public int Status { get; set; }
        public string? ErrorMessage { get; set; }
    }
}

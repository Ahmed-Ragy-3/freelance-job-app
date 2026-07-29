namespace backend.DTOs {
    public class PaginatedResponse<T> {
        public List<T> Items { get; set; } = new List<T>();
        public int TotalCount { get; set; }
    }
}
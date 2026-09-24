namespace blogapi.models.DTOs
{
    public class UpdateBlogpost
    {
        public string? Title { get; set; }
        public string? Content { get; set; }
        public int blogId { get; set; }
    }
}

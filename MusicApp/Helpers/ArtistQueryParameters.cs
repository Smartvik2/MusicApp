namespace MusicApp.Helpers
{
    public class ArtistQueryParameters
    {
        public string? SearchTerm { get; set; }
        public string? OrderBy { get; set; } = "fullname"; // default sort
        public bool Descending { get; set; } = false;
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}

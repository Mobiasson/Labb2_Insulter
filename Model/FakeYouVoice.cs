namespace Insulter.Model // Use the same namespace as your VoiceService, or add a using statement
{
    public class FakeYouVoice
    {
        public string ModelToken { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string CreatorUsername { get; set; } = string.Empty;

        public override string ToString() => $"{Title} (by {CreatorUsername})";
    }
}
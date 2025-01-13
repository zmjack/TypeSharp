namespace TypeSharp
{
    public class ApiBuilderOptions
    {
        public static readonly ApiBuilderOptions Default = new();

        public string DefaultPattern { get; set; } = "{controller=Home}/{action=Index}";
        public bool Verbose { get; set; }
    }
}

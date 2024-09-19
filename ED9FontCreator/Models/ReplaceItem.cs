namespace ED9FontCreator.Models
{
    internal class ReplaceItem(string? old, string? @new)
    {
        public string? Old = old;
        public string? New = @new;
    }
}
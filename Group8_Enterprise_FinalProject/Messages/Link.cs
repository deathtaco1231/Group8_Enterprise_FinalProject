namespace Group8_Enterprise_FinalProject.Messages
{
    /// <summary> 
    /// Borrowed from Task V3.1 example since it serves the same purpose here and functionally works identically
    /// </summary>
    public class Link
    {
        public Link()
        {
        }

        /// <summary>
        /// Overridden constructor just for easy creation (Not currently used but, again, same as Task example I have it just in case)
        /// </summary>
        /// <param name="href"></param>
        /// <param name="rel"></param>
        /// <param name="method"></param>
        public Link(string href, string rel, string method)
        {
            Href = href;
            Rel = rel;
            Method = method;
        }
        public string? Href { get; set; }
        public string? Rel { get; set; }
        public string? Method { get; set; }

    }
}

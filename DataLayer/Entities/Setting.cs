using System.Diagnostics.CodeAnalysis;

namespace DataLayer.Entities
{
    public class Setting
    {
        public int Id { get; set; }
        public string Title { get; set; }
        [AllowNull]
        public string? Description { get; set; }
        [AllowNull]
        public string? DataString { get; set; }
        [AllowNull]
        public bool? DataBool { get; set; }
        [AllowNull]
        public int? DataInt { get; set; }
        public bool Changeable { get; set; }
    }
}

using System;
using System.Text.Json.Serialization;

namespace Lms.Core.Entities
{
    public class Module
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
        public int CourseId { get; set; }
        [JsonIgnore]
        public Course Course { get; set; }
    }
}
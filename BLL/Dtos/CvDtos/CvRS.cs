namespace BLL.Dtos.CvDtos
{
    public class CvRS
    {
        public int CVId { get; set; }
        public string FileName { get; set; }
        public string FileUrl { get; set; }
        public DateTime UploadedAt { get; set; }
        public bool IsPrimary { get; set; }
        public string ParsedContent { get; set; }
        public List<string> ExtractedSkills { get; set; }
        public List<string>? CVIssues { get; set; }
        public int CVScore { get; set; }
        public List<string>? SuggestedJobTitles { get; set; }
        public List<string>? RecommendedSkills { get; set; }
    }
}

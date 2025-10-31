using DiffPlex.DiffBuilder.Model;

namespace CodeDiffLLM.Models
{
    public class DiffLine
    {
        public DiffLine(ChangeType changeType, string lineContent)
        {
            this.ChangeType = changeType;
            this.LineContent = lineContent;
        }

        public ChangeType ChangeType { get; set; }

        public string LineContent { get; set; }
    }
}

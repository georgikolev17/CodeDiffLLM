using CodeDiffLLM.Models;

namespace CodeDiffLLM.Services
{
    public interface ICodeService
    {
        IEnumerable<DiffLine> GetCodeDiffLines(string originalCode, string modifiedCode);

        string BuildPrompt(IEnumerable<DiffLine> diffLines);
    }
}

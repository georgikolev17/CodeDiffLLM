using CodeDiffLLM.Models;
using DiffPlex;
using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;
using System.Text;

namespace CodeDiffLLM.Services
{
    public class CodeService : ICodeService
    {
        /// <summary>
        /// Builds a prompt for a code review task based on a collection of unified diff lines.
        /// </summary>
        /// <param name="diffLines">A collection of <see cref="DiffLine"/> objects representing the unified diff between the original modified code.</param>
        /// <returns>A formatted string containing instructions and context for a code review, including the diff content  specific tasks for the reviewer.</returns>
        public string BuildPrompt(IEnumerable<DiffLine> diffLines)
        {
            var promptBuilder = new StringBuilder();

            // Assign role
            promptBuilder.AppendLine("You are a senior C#/.NET engineer and code reviewer. Analyze the following unified diff carefully.");
            promptBuilder.AppendLine("Focus on correctness, security, performance, reliability, and maintainability. Provide concise, actionable feedback.");
            promptBuilder.AppendLine();

            // Provide context
            promptBuilder.AppendLine("## Context");
            promptBuilder.AppendLine("- Project: ASP.NET Core MVC application");
            promptBuilder.AppendLine("- Language: C# (.NET 9)");
            promptBuilder.AppendLine("- Coding standards: Microsoft defaults, nullable enabled, async/await best practices");
            promptBuilder.AppendLine();
            promptBuilder.AppendLine("## Artifact");
            promptBuilder.AppendLine("Below is the unified diff between the original and modified code:");
            promptBuilder.AppendLine();

            // Include the diff in a separate code block
            promptBuilder.AppendLine("```diff");
            foreach (var line in diffLines)
            {
                switch (line.ChangeType)
                {
                    case ChangeType.Inserted:
                        promptBuilder.AppendLine($"+ {line.LineContent}");
                        break;

                    case ChangeType.Deleted:
                        promptBuilder.AppendLine($"- {line.LineContent}");
                        break;

                    case ChangeType.Unchanged:
                        promptBuilder.AppendLine($"  {line.LineContent}");
                        break;
                }
            }

            promptBuilder.AppendLine("```");
            promptBuilder.AppendLine();

            // Define the task
            promptBuilder.AppendLine("## Task");
            promptBuilder.AppendLine("1. Summarize the intent of the change in 2–4 short bullets.");
            promptBuilder.AppendLine("2. Identify any potential issues under these categories:");
            promptBuilder.AppendLine("   - Correctness & Behavior");
            promptBuilder.AppendLine("   - Security (ASP.NET Core specifics like model binding, validation, XSS, authorization)");
            promptBuilder.AppendLine("   - Performance (allocations, async, LINQ, caching)");
            promptBuilder.AppendLine("   - Reliability & Concurrency");
            promptBuilder.AppendLine("   - Maintainability & Style");
            promptBuilder.AppendLine("3. Suggest improvements with exact code edits if possible.");
            promptBuilder.AppendLine();

            // Specify format
            promptBuilder.AppendLine("## Output Format (strict)");
            promptBuilder.AppendLine("```markdown");
            promptBuilder.AppendLine("## Summary");
            promptBuilder.AppendLine("- <3 concise bullets>");
            promptBuilder.AppendLine();
            promptBuilder.AppendLine("## Findings");
            promptBuilder.AppendLine("### Critical");
            promptBuilder.AppendLine("1. [File.cs:@@ -X,Y +A,B @@] <issue>");
            promptBuilder.AppendLine("   **Why:** <risk/impact>");
            promptBuilder.AppendLine("   **Fix:** <short actionable fix>");
            promptBuilder.AppendLine();
            promptBuilder.AppendLine("### Major");
            promptBuilder.AppendLine("1. [File.cs:@@ ... @@] <issue>");
            promptBuilder.AppendLine();
            promptBuilder.AppendLine("### Minor");
            promptBuilder.AppendLine("- [File.cs:@@ ... @@] <nit or style note>");
            promptBuilder.AppendLine();
            promptBuilder.AppendLine("## Tests To Add");
            promptBuilder.AppendLine("- `<TestName>`: verifies <condition>");
            promptBuilder.AppendLine("```");
            promptBuilder.AppendLine();
            promptBuilder.AppendLine("If the diff appears incomplete or lacks context, note which parts are missing before analyzing.");

            return promptBuilder.ToString();

        }

        /// <summary>
        /// Generates a collection of lines representing the differences between two versions of code.
        /// </summary>
        /// <remarks>The idea of the method is to make the diff as close as possible to a Git Diff. Hence, modified lines are treated as inserted</remarks>
        /// <param name="originalCode">The original version of the code to compare.</param>
        /// <param name="modifiedCode">The modified version of the code to compare against the original.</param>
        /// <returns>An enumerable collection of <see cref="DiffLine"/> objects, each representing a line of code that has been inserted, deleted, or remains unchanged.</returns>
        public IEnumerable<DiffLine> GetCodeDiffLines(string originalCode, string modifiedCode)
        {
            var differ = new Differ();
            var builder = new InlineDiffBuilder(differ);
            var diff = builder.BuildDiffModel(originalCode, modifiedCode);

            var diffLines = new List<DiffLine>();

            foreach (var line in diff.Lines)
            {
                switch (line.Type)
                {
                    case ChangeType.Inserted:
                        diffLines.Add(new DiffLine(ChangeType.Inserted, line.Text));
                        break;
                    case ChangeType.Deleted:
                        diffLines.Add(new DiffLine(ChangeType.Deleted, line.Text));
                        break;
                    case ChangeType.Modified:
                        // Modified lines are treated as inserted for LLM purposes.
                        diffLines.Add(new DiffLine(ChangeType.Inserted, line.Text));
                        break;
                    case ChangeType.Imaginary:
                        // This line is placed for alignment purposes and is irrelevant for the LLM prompt.
                        break;
                    default:
                        // Unchanged line (Generalized)
                        diffLines.Add(new DiffLine(ChangeType.Unchanged, line.Text));
                        break;
                }
            }

            return diffLines;
        }
    }
}

# 🧠 LLM Code-Diff Prompt Builder

This is a simple **ASP.NET Core MVC** application that demonstrates how to generate a Git-style code diff between two versions of a file and convert it into a clear, structured **prompt** for LLM-based code analysis.

---

## 💡 Thought Process

I started by thinking about how LLMs understand code — most of them are trained on **Git-style diffs** from GitHub.  
That’s why I decided to use the **DiffPlex** library to generate diffs that look exactly like real Git output (`+` for added lines, `-` for removed ones, and context lines).  
This makes the generated diff easy for both humans and AI models to interpret.

Then I focused on turning that diff into a **structured prompt** that clearly tells the LLM what to do — for example, to review the changes for correctness, security, and performance.  
I wanted the format to look like something a **senior developer** or a **code review tool** would write, not just a block of text.

---

## ⚙️ How It Works

1. The main page contains two text boxes:
   - **Original Code** – readonly and pre-filled with sample C#.
   - **Modified Code** – editable so users can make changes.
2. When the user clicks **Generate Diff**, the backend:
   - Uses **DiffPlex** to compute a unified, Git-style diff.  
   - Passes that diff to a **prompt builder** service.
3. The service builds a full **LLM review prompt** using Markdown formatting.
4. The prompt is displayed on a new page.

---

## 🧩 Prompt Structure

The generated prompt follows a strict layout designed to help large language models return consistent responses:
1. Assign a role
2. Provide Context
3. Give data (the generated diff)
4. Set task
5. Set output format

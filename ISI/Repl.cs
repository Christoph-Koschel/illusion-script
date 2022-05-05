using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace IllusionScript.ISI
{
    internal abstract partial class Repl
    {
        private bool done;
        private readonly List<MetaCommand> metaCommands;
        private readonly List<string> submissionHistory;
        private int submissionHistoryIndex;

        protected Repl()
        {
            metaCommands = new List<MetaCommand>();
            submissionHistory = new List<string>();
            InitMetaCommands();
        }

        private void InitMetaCommands()
        {
            MethodInfo[] methods = GetType()
                .GetMethods(BindingFlags.NonPublic |
                            BindingFlags.Instance |
                            BindingFlags.FlattenHierarchy
                );
            foreach (MethodInfo method in methods)
            {
                CustomAttributeData? attribute = method.GetCustomAttributesData()
                    .SingleOrDefault(cd => cd.AttributeType == typeof(MetaCommandAttribute));

                if (attribute == null)
                {
                    continue;
                }

                string name = attribute.ConstructorArguments.FirstOrDefault().Value as string;
                if (string.IsNullOrEmpty(name))
                {
                    continue;
                }

                string description = attribute.ConstructorArguments.LastOrDefault().Value as string;

                if (string.IsNullOrEmpty(description))
                {
                    continue;
                }

                MetaCommand metaCommand = new MetaCommand(name, description, method);
                metaCommands.Add(metaCommand);
            }
        }

        public void Run()
        {
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Green;

                string text = EditSubmission();
                if (string.IsNullOrEmpty(text))
                {
                    return;
                }

                if (!text.Contains("\n") && text.StartsWith("#"))
                {
                    InvokeMetaCommand(text);
                }
                else
                {
                    Invoke(text);
                }

                submissionHistory.Add(text);
                submissionHistoryIndex = 0;
            }
        }

        private string EditSubmission()
        {
            done = false;
            ObservableCollection<string> document = new ObservableCollection<string>() { "" };
            Repl.SubmissionView view = new Repl.SubmissionView(Renderer, document);
            while (!done)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                HandleKey(key, document, view);
            }

            view.CurrentLineIndex = document.Count - 1;
            view.CurrentCharacter = document[view.CurrentLineIndex].Length;

            Console.Write("\n");

            return string.Join("\n", document);
        }

        private void HandleKey(ConsoleKeyInfo key, ObservableCollection<string> document, SubmissionView view)
        {
            if (key.Modifiers == ConsoleModifiers.Shift)
            {
                switch (key.Key)
                {
                    case ConsoleKey.Tab:
                        HandleTabulator(document, view, true);
                        break;
                    case ConsoleKey.Enter:
                        HandleShiftEnter(document, view);
                        break;
                }
            }
            else
            {
                switch (key.Key)
                {
                    case ConsoleKey.Enter:
                        HandleEnter(document, view);
                        break;
                    case ConsoleKey.LeftArrow:
                        HandleLeftArrow(document, view);
                        break;
                    case ConsoleKey.RightArrow:
                        HandleRightArrow(document, view);
                        break;
                    case ConsoleKey.UpArrow:
                        HandleUpArrow(document, view);
                        break;
                    case ConsoleKey.DownArrow:
                        HandleDownArrow(document, view);
                        break;
                    case ConsoleKey.Backspace:
                        HandleBackspace(document, view);
                        break;
                    case ConsoleKey.Delete:
                        HandleDelete(document, view);
                        break;
                    case ConsoleKey.Tab:
                        HandleTabulator(document, view);
                        break;
                    case ConsoleKey.PageUp:
                        HandlePageUp(document, view);
                        break;
                    case ConsoleKey.PageDown:
                        HandlePageDown(document, view);
                        break;
                }
            }

            if (key.KeyChar >= ' ')
            {
                HandleTyping(document, view, key.KeyChar);
            }
        }

        private void HandleShiftEnter(ObservableCollection<string> document, SubmissionView view)
        {
            int start = view.CurrentCharacter;
            string before = document[view.CurrentLineIndex].Substring(0, start);
            string after = document[view.CurrentLineIndex].Substring(start);

            document[view.CurrentLineIndex] = before;
            document.Insert(view.CurrentLineIndex + 1, after);
            view.CurrentCharacter = 0;
            view.CurrentLineIndex++;
        }

        private void HandleEnter(ObservableCollection<string> document, SubmissionView view)
        {
            done = true;
        }

        private void HandleLeftArrow(ObservableCollection<string> document, SubmissionView view)
        {
            if (view.CurrentCharacter > 0)
            {
                view.CurrentCharacter--;
            }
        }

        private void HandleRightArrow(ObservableCollection<string> document, SubmissionView view)
        {
            string line = document[view.CurrentLineIndex];
            if (view.CurrentCharacter < line.Length)
            {
                view.CurrentCharacter++;
            }
        }

        private void HandleUpArrow(ObservableCollection<string> document, SubmissionView view)
        {
            if (view.CurrentLineIndex > 0)
            {
                view.CurrentLineIndex--;
                if (document[view.CurrentLineIndex].Length - 1 < view.CurrentCharacter)
                {
                    view.CurrentCharacter = document[view.CurrentLineIndex].Length;
                }
            }
        }

        private void HandleDownArrow(ObservableCollection<string> document, SubmissionView view)
        {
            if (view.CurrentLineIndex < document.Count - 1)
            {
                view.CurrentLineIndex++;
                if (document[view.CurrentLineIndex].Length - 1 < view.CurrentCharacter)
                {
                    view.CurrentCharacter = document[view.CurrentLineIndex].Length;
                }
            }
        }

        private void HandleTabulator(ObservableCollection<string> document, SubmissionView view, bool back = false)
        {
            if (back)
            {
                HandleBackspace(document, view);
                HandleBackspace(document, view);
                HandleBackspace(document, view);
                HandleBackspace(document, view);
            }
            else
            {
                HandleTyping(document, view, ' ');
                HandleTyping(document, view, ' ');
                HandleTyping(document, view, ' ');
                HandleTyping(document, view, ' ');
            }
        }

        private void HandleTyping(ObservableCollection<string> document, SubmissionView view, char c)
        {
            int lineIndex = view.CurrentLineIndex;
            int start = view.CurrentCharacter;

            document[lineIndex] = document[lineIndex].Insert(start, c.ToString());
            view.CurrentCharacter++;
        }

        private void HandleDelete(ObservableCollection<string> document, SubmissionView view)
        {
            int lineIndex = view.CurrentLineIndex;
            string line = document[lineIndex];
            int start = view.CurrentCharacter;
            if (start >= line.Length)
            {
                return;
            }

            string before = line.Substring(0, start);
            string after = line.Substring(start + 1);

            document[lineIndex] = before + after;
        }

        private void HandleBackspace(ObservableCollection<string> document, SubmissionView view)
        {
            int start = view.CurrentCharacter;
            if (start == 0)
            {
                if (view.CurrentLineIndex == 0)
                {
                    return;
                }

                string currentLine = document[view.CurrentLineIndex];
                string previousLine = document[view.CurrentLineIndex - 1];

                document.RemoveAt(view.CurrentLineIndex);
                view.CurrentLineIndex--;
                document[view.CurrentLineIndex] = previousLine + currentLine;
                view.CurrentCharacter = previousLine.Length;
            }
            else
            {
                int lineIndex = view.CurrentLineIndex;
                string line = document[lineIndex];
                string before = line.Substring(0, start - 1);
                string after = line.Substring(start);

                document[lineIndex] = before + after;
                view.CurrentCharacter--;
            }
        }

        private void HandlePageDown(ObservableCollection<string> document, SubmissionView view)
        {
            submissionHistoryIndex--;
            if (submissionHistoryIndex < 0)
            {
                submissionHistoryIndex = submissionHistory.Count - 1;
            }

            UpdateDocumentFromHistory(document, view);
        }

        private void HandlePageUp(ObservableCollection<string> document, SubmissionView view)
        {
            submissionHistoryIndex++;
            if (submissionHistoryIndex > submissionHistory.Count - 1)
            {
                submissionHistoryIndex = 0;
            }

            UpdateDocumentFromHistory(document, view);
        }

        private void UpdateDocumentFromHistory(ObservableCollection<string> document, SubmissionView view)
        {
            if (submissionHistory.Count == 0)
            {
                return;
            }

            document.Clear();

            string history = submissionHistory[submissionHistoryIndex];
            string[] lines = history.Split('\n');
            foreach (string line in lines)
            {
                document.Add(line);
            }

            view.CurrentLineIndex = document.Count - 1;
            view.CurrentCharacter = document[view.CurrentLineIndex].Length;
        }

        protected void ClearHistory()
        {
            submissionHistory.Clear();
        }

        protected void InvokeMetaCommand(string input)
        {
            List<string> args = new List<string>();
            int position = 1;
            bool inQuotes = false;
            StringBuilder sb = new StringBuilder();
            while (position < input.Length)
            {
                char c = input[position];
                char l = position + 1 >= input.Length ? '\0' : input[position + 1];

                if (c == '\\')
                {
                    if (l == '"')
                    {
                        position++;
                        sb.Append('"');
                    }
                    else if (c == '\\')
                    {
                        position++;
                        sb.Append('\\');
                    }
                }
                else if (c == '"')
                {
                    if (inQuotes)
                    {
                        inQuotes = false;
                        args.Add(sb.ToString());
                        sb.Clear();
                    }
                    else
                    {
                        inQuotes = true;
                    }
                }
                else if (char.IsWhiteSpace(c) && !inQuotes)
                {
                    args.Add(sb.ToString());
                    sb.Clear();
                }
                else
                {
                    sb.Append(c);
                }

                position++;
            }

            if (sb.Length != 0 && !string.IsNullOrWhiteSpace(sb.ToString()))
            {
                args.Add(sb.ToString());
            }


            if (args.Count != 0)
            {
                string commandName = args[0];
                args.RemoveAt(0);

                MetaCommand? command = metaCommands.SingleOrDefault(mc => mc.name == commandName);
                if (command != null)
                {
                    ParameterInfo[] parameters = command.method.GetParameters();


                    if (args.Count != parameters.Length)
                    {
                        string parameterNames = string.Join(", ", parameters
                            .Select(p => $"<{p.Name}>"));

                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine(
                            $"ERROR: Invalid number of arguments (given: {args.Count}, expected: {parameters.Length})");
                        Console.WriteLine($"Usage: #{commandName} {parameterNames}");
                        Console.ResetColor();
                        return;
                    }

                    object[] arguments = (object[])args.ToArray();
                    command.method.Invoke(this, arguments);
                    return;
                }
            }

            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine($"Invalid command {input}");
            Console.ResetColor();
        }

        protected virtual void Renderer(string line)
        {
            Console.Write(line);
        }

        protected abstract void Invoke(string input);

        [MetaCommand("help", "Shows help")]
        protected void InvokeHelp()
        {
            foreach (MetaCommand metaCommand in metaCommands)
            {
                Console.WriteLine($"#{metaCommand.name,-15} {metaCommand.description,-20}");
            }
        }
    }
}
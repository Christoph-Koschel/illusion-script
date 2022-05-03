using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace IllusionScript.ISI
{
    internal abstract partial class Repl
    {
        private sealed class SubmissionView
        {
            private readonly Action<string> lineRenderer;
            private readonly ObservableCollection<string> document;
            private readonly int top;
            private int renderedLineCount;
            private int currentCharacter;
            private int currentLineIndex;

            public SubmissionView(Action<string> lineRenderer,ObservableCollection<string> document)
            {
                this.lineRenderer = lineRenderer;
                this.document = document;
                this.document.CollectionChanged += SubmissionDocumentChanged;
                top = Console.CursorTop;
                Render();
            }

            private void SubmissionDocumentChanged(object? sender, NotifyCollectionChangedEventArgs e)
            {
                Render();
            }

            private void Render()
            {
                Console.CursorVisible = false;
                int lineCount = 0;

                foreach (string line in document)
                {
                    Console.SetCursorPosition(0, top + lineCount);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(lineCount == 0 ? "» " : "· ");
                    
                    Console.ResetColor();
                    lineRenderer(line);
                    Console.WriteLine(new string(' ', Console.WindowWidth - line.Length));
                    lineCount++;
                }

                int numberOfBlankLines = renderedLineCount - lineCount;
                if (numberOfBlankLines > 0)
                {
                    string blankLine = new string(' ', Console.WindowWidth);
                    for (int i = 0; i < numberOfBlankLines; i++)
                    {
                        Console.SetCursorPosition(0, top + lineCount + i);
                        Console.WriteLine(blankLine);
                    }
                }

                renderedLineCount = lineCount;
                Console.CursorVisible = true;
                UpdateCursorPosition();
            }

            private void UpdateCursorPosition()
            {
                Console.CursorTop = top + currentLineIndex;
                Console.CursorLeft = 2 + currentCharacter;
            }

            public int CurrentLineIndex
            {
                get => currentLineIndex;
                set
                {
                    if (currentLineIndex != value)
                    {
                        currentLineIndex = value;
                        UpdateCursorPosition();
                    }
                }
            }

            public int CurrentCharacter
            {
                get => currentCharacter;
                set
                {
                    if (currentCharacter != value)
                    {
                        currentCharacter = value;
                        UpdateCursorPosition();
                    }
                }
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using IllusionScript.Runtime.Binding.Nodes.Expressions;
using IllusionScript.Runtime.Binding.Nodes.Statements;

namespace IllusionScript.Runtime.Binding.Nodes
{
    internal abstract class BoundNode
    {
        public abstract BoundNodeType boundType { get; }

        public IEnumerable<BoundNode> GetChildren()
        {
            FieldInfo[] fields = GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (FieldInfo field in fields)
            {
                if (typeof(BoundNode).IsAssignableFrom(field.FieldType))
                {
                    BoundNode child = (BoundNode)field.GetValue(this);
                    if (child != null)
                    {
                        yield return child;
                    }
                }
                else if (typeof(IEnumerable<BoundNode>).IsAssignableFrom(field.FieldType))
                {
                    IEnumerable<BoundNode> children = (IEnumerable<BoundNode>)field.GetValue(this);
                    foreach (BoundNode child in children)
                    {
                        if (child != null)
                        {
                            yield return child;
                        }
                    }
                }
            }
        }

        private IEnumerable<(string name, object value)> GetProperties()
        {
            FieldInfo[] fields = GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (FieldInfo field in fields)
            {
                if (field.Name == nameof(boundType) ||
                    field.Name == nameof(BoundBinaryExpression.binaryOperator))
                {
                    continue;
                }

                if (typeof(BoundNode).IsAssignableFrom(field.FieldType) ||
                    typeof(IEnumerable<BoundNode>).IsAssignableFrom(field.FieldType))
                {
                    continue;
                }

                object value = field.GetValue(this);
                if (value != null)
                {
                    yield return (field.Name, value);
                }
            }
        }

        public void WriteTo(TextWriter writer)
        {
            PrettyPrint(writer, this);
        }

        private static void PrettyPrint(TextWriter writer, BoundNode node, string indent = "", bool isLast = true)
        {
            bool isConsole = writer == Console.Out;

            string marker = isLast ? "└──" : "├──";

            if (isConsole)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
            }

            writer.Write(indent);
            writer.Write(marker);

            if (isConsole)
            {
                Console.ForegroundColor = GetColor(node);
            }

            string text = GetText(node);
            writer.Write(text);

            bool isFirstProperty = true;

            foreach ((string name, object value) property in node.GetProperties())
            {
                if (isFirstProperty)
                {
                    isFirstProperty = false;
                }
                else
                {
                    if (isConsole)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                    }

                    writer.Write(",");
                }

                writer.Write(" ");

                if (isConsole)
                {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                }

                writer.Write(property.name);

                if (isConsole)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                }

                writer.Write(" = ");

                if (isConsole)
                {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                }

                writer.Write(property.value);
            }

            if (isConsole)
            {
                Console.ResetColor();
            }

            writer.Write("\n");


            indent += isLast ? "   " : "│  ";

            BoundNode lastChild = node.GetChildren().LastOrDefault();

            foreach (BoundNode child in node.GetChildren())
            {
                PrettyPrint(writer, child, indent, child == lastChild);
            }
        }

        private static string GetText(BoundNode node)
        {
            if (node is BoundBinaryExpression b)
            {
                return b.binaryOperator.operatorType + "Expression";
            }

            if (node is BoundUnaryExpression u)
            {
                return u.unaryOperator.operatorType + "Expression";
            }

            return node.boundType.ToString();
        }

        private static ConsoleColor GetColor(BoundNode node)
        {
            if (node is BoundExpression)
            {
                return ConsoleColor.DarkBlue;
            }

            if (node is BoundStatement)
            {
                return ConsoleColor.DarkCyan;
            }

            return ConsoleColor.DarkYellow;
        }

        public override string ToString()
        {
            using StringWriter writer = new StringWriter();
            WriteTo(writer);
            return writer.ToString();
        }
    }
}
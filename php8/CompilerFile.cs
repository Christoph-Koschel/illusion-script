using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Text;
using IllusionScript.Runtime.Binding;
using IllusionScript.Runtime.Binding.Nodes.Expressions;
using IllusionScript.Runtime.Binding.Nodes.Statements;
using IllusionScript.Runtime.Interpreting.Memory.Symbols;

namespace IllusionScript.Compiler.PHP8
{
    public sealed class CompilerFile
    {
        private readonly StreamWriter writer;
        private readonly string name;

        public CompilerFile(FileStream writer)
        {
            this.writer = new StreamWriter(writer, Encoding.UTF8);
            name = Path.GetFileName(writer.Name);
        }

        public void Close()
        {
            writer.Close();
        }

        public void WriteHeader(Dictionary<string, List<FunctionSymbol>>.KeyCollection keyCollection)
        {
            writer.Write("<?php\n");

            writer.Write($"include_once \"./syscall.php\";\n");

            foreach (string s in keyCollection)
            {
                string file = Path.GetFileName(s);
                if (file == name)
                {
                    continue;
                }

                writer.Write($"include_once \"./{file}\";\n");
            }
        }

        public void Write(FunctionSymbol function,
            ImmutableDictionary<FunctionSymbol, BoundBlockStatement> functionBodies)
        {
            if (function.name == "$eval")
            {
                writer.Write("(");
            }

            WriteFunctionHead(function);
            writer.Write("\n{\n");
            WriteStatement(functionBodies[function]);

            writer.Write(function.name == "$eval" ? "})()\n" : "}\n");
        }

        private void WriteBlockStatement(BoundBlockStatement body)
        {
            foreach (BoundStatement statement in body.statements)
            {
                writer.Write("    ");
                WriteStatement(statement);
            }
        }

        private void WriteStatement(BoundStatement statement)
        {
            switch (statement.boundType)
            {
                case BoundNodeType.BlockStatement:
                    WriteBlockStatement((BoundBlockStatement)statement);
                    break;
                case BoundNodeType.ExpressionStatement:
                    WriteExpressionStatement((BoundExpressionStatement)statement);
                    writer.Write(";\n");
                    break;
                case BoundNodeType.VariableDeclarationStatement:
                    WriteVariableDeclarationStatement((BoundVariableDeclarationStatement)statement);
                    writer.Write(";\n");
                    break;
                case BoundNodeType.ReturnStatement:
                    WriteReturnStatement((BoundReturnStatement)statement);
                    writer.Write(";\n");
                    break;
                default:
                    writer.Close(); // TODO delete file
                    throw new Exception($"Undefined statement {statement.boundType}");
            }
        }

        private void WriteReturnStatement(BoundReturnStatement statement)
        {
            writer.Write("return");
            if (statement.expression != null)
            {
                writer.Write(" ");
                WriteExpression(statement.expression);
            }
        }

        private void WriteExpressionStatement(BoundExpressionStatement statement)
        {
            WriteExpression(statement.expression);
        }

        private void WriteVariableDeclarationStatement(BoundVariableDeclarationStatement statement)
        {
            writer.Write("$");
            writer.Write(statement.variable.name);
            writer.Write(" = ");

            WriteExpression(statement.initializer);
        }

        private void WriteExpression(BoundExpression expression)
        {
            switch (expression.boundType)
            {
                case BoundNodeType.ConversionExpression:
                    WriteConversionExpression((BoundConversionExpression)expression);
                    break;
                case BoundNodeType.CallExpression:
                    WriteCallExpression((BoundCallExpression)expression);
                    break;
                case BoundNodeType.VariableExpression:
                    WriteVariableExpression((BoundVariableExpression)expression);
                    break;
                case BoundNodeType.LiteralExpression:
                    WriteLiteralExpression((BoundLiteralExpression)expression);
                    break;
                default:
                    writer.Close(); // TODO delete file
                    throw new Exception($"Undefined expression {expression.boundType}");
            }
        }

        private void WriteLiteralExpression(BoundLiteralExpression expression)
        {
            if (expression.type == TypeSymbol.Bool)
            {
                writer.Write((bool)expression.value ? "true" : "false");
            }
            else if (expression.type == TypeSymbol.Int)
            {
                writer.Write(expression.value);
            }
            else if (expression.type == TypeSymbol.String)
            {
                writer.Write("\"");
                writer.Write(((string)expression.value).Replace("\"", "\\\""));
                writer.Write("\"");
            }
            else
            {
                throw new Exception($"Undefined type {expression.type}");
            }
        }

        private void WriteVariableExpression(BoundVariableExpression expression)
        {
            writer.Write("$");
            writer.Write(expression.variableSymbol.name);
        }

        private void WriteCallExpression(BoundCallExpression expression)
        {
            writer.Write(expression.function.name);
            writer.Write("(");

            bool first = true;
            foreach (BoundExpression argument in expression.arguments)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    writer.Write(", ");
                }

                WriteExpression(argument);
            }

            writer.Write(")");
        }

        private void WriteConversionExpression(BoundConversionExpression expression)
        {
            writer.Write("(");
            writer.Write(expression.type.name.ToLower());
            writer.Write(")");
            WriteExpression(expression.expression);
        }

        private void WriteFunctionHead(FunctionSymbol function)
        {
            writer.Write("function ");
            writer.Write(function.name);
            writer.Write("(");

            bool first = true;

            foreach (ParameterSymbol parameter in function.parameters)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    writer.Write(", ");
                }

                writer.Write("$");
                writer.Write(parameter.name);
            }

            writer.Write(")");
        }
    }
}
﻿using System;
using System.Collections.Generic;
using IllusionScript.Runtime.Diagnostics;
using IllusionScript.Runtime.Interpreting;
using IllusionScript.Runtime.Interpreting.Memory;
using IllusionScript.Runtime.Parsing;
using Xunit;

namespace IllusionScript.Runtime.Test.Interpreting
{
    public class InterpreterTest
    {
        [Theory]
        [InlineData("1", 1)]
        [InlineData("+1", 1)]
        [InlineData("-1", -1)]
        [InlineData("~1", -2)]
        [InlineData("14 + 12", 26)]
        [InlineData("12 - 3", 9)]
        [InlineData("4 * 2", 8)]
        [InlineData("9 / 3", 3)]
        [InlineData("(10)", 10)]
        [InlineData("12 == 3", false)]
        [InlineData("3 == 3", true)]
        [InlineData("12 != 3", true)]
        [InlineData("3 != 3", false)]
        [InlineData("3 < 4", true)]
        [InlineData("5 < 4", false)]
        [InlineData("50 << 2", 200)]
        [InlineData("2 << 1", 4)]
        [InlineData("4 <= 4", true)]
        [InlineData("4 <= 5", true)]
        [InlineData("5 <= 4", false)]
        [InlineData("4 > 3", true)]
        [InlineData("4 > 5", false)]
        [InlineData("1 >> 45", 0)]
        [InlineData("50 >> 2", 12)]
        [InlineData("4 >= 4", true)]
        [InlineData("5 >= 4", true)]
        [InlineData("4 >= 5", false)]
        [InlineData("1 | 2", 3)]
        [InlineData("1 | 0", 1)]
        [InlineData("1 & 3", 1)]
        [InlineData("1 & 0", 0)]
        [InlineData("1 ^ 0", 1)]
        [InlineData("0 ^ 1", 1)]
        [InlineData("1 ^ 3", 2)]
        [InlineData("false == false", true)]
        [InlineData("true == false", false)]
        [InlineData("false != false", false)]
        [InlineData("true != false", true)]
        [InlineData("true && true", true)]
        [InlineData("false || false", false)]
        [InlineData("false | false", false)]
        [InlineData("false | true", true)]
        [InlineData("true | false", true)]
        [InlineData("true | true", true)]
        [InlineData("false & false", false)]
        [InlineData("false & true", false)]
        [InlineData("true & false", false)]
        [InlineData("true & true", true)]
        [InlineData("false ^ false", false)]
        [InlineData("true ^ false", true)]
        [InlineData("false ^ true", true)]
        [InlineData("true ^ true", false)]
        [InlineData("true", true)]
        [InlineData("false", false)]
        [InlineData("!true", false)]
        [InlineData("!false", true)]
        [InlineData("{ let a = 10 a = 10 * a }", 100)]
        [InlineData("{ let a = 0 if (a == 0) a = 10 }", 10)]
        [InlineData("{ let a = 0 if (a == 4) a = 10 }", 0)]
        [InlineData("{ let a = 0 if (a == 0) a = 10 else a = 5 }", 10)]
        [InlineData("{ let a = 0 if (a == 4) a = 10 else a = 5 }", 5)]
        [InlineData("{ let a = 0 while (a != 4) a = a + 1 }", 4)]
        [InlineData("{ let a = 0 for (i = 0 to 10) a = a + i}", 45)]
        public void InterpreterComputesCorrectValues(string text, object expectedValue)
        {
            AssertValue(text, expectedValue);
        }

        private static void AssertValue(string text, object expectedValue)
        {
            SyntaxTree syntaxThree = SyntaxTree.Parse(text);
            Compilation compilation = new Compilation(syntaxThree);
            Dictionary<VariableSymbol, object> variables = new Dictionary<VariableSymbol, object>();
            InterpreterResult result = compilation.Interpret(variables);

            Assert.Empty(result.diagnostics);
            Assert.Equal(expectedValue, result.value);
        }

        [Fact]
        public void InterpreterVariableDeclaration()
        {
            string text = @"
                {
                    let x = 10
                    let y = 100
                    {
                        let x = 10
                    }
                    let [x] = 5
                }
            ";

            string diagnostics = @"
                ERROR: Variable 'x' is already declared
            ";

            AssertDiagnostics(text, diagnostics);
        }

        [Fact]
        public void InterpreterVariableNameExpression()
        {
            string text = @"[x] * 10";

            string diagnostics = @"
                ERROR: Variable 'x' doesnt exist
            ";

            AssertDiagnostics(text, diagnostics);
        }

        [Fact]
        public void InterpreterVariableAssignmentExpression()
        {
            string text = @"[x] = 10";

            string diagnostics = @"
                ERROR: Variable 'x' doesnt exist
            ";

            AssertDiagnostics(text, diagnostics);
        }

        [Fact]
        public void InterpreterVariableConstAssignmentExpression()
        {
            string text = @"
                {
                    const x = 10
                    [x] = 0
                }
            ";

            string diagnostics = @"
                ERROR: Variable 'x' is read-ony and cannot be assigned to
            ";

            AssertDiagnostics(text, diagnostics);
        }


        [Fact]
        public void InterpreterVariableConvert()
        {
            string text = @"
                {
                    let x = 10
                    x = [true]
                }
            ";

            string diagnostics = @"
                ERROR: Cannot convert type 'System.Boolean' to 'System.Int32'
            ";

            AssertDiagnostics(text, diagnostics);
        }

        [Fact]
        public void InterpreterUnaryUndefinedOperator()
        {
            string text = @"[+]true";

            string diagnostics = @"
                ERROR: Unary operator '+' is not defined for type System.Boolean
            ";

            AssertDiagnostics(text, diagnostics);
        }

        [Fact]
        public void InterpreterBinaryUndefinedOperator()
        {
            string text = @"10 [+] true";

            string diagnostics = @"
                ERROR: Binary operator '+' is not defined for type System.Int32 and System.Boolean
            ";

            AssertDiagnostics(text, diagnostics);
        }

        private void AssertDiagnostics(string text, string diagnosticsText)
        {
            AnnotatedText annotatedText = AnnotatedText.Parse(text);
            SyntaxTree syntaxTree = SyntaxTree.Parse(annotatedText.text);
            Compilation compilation = new Compilation(syntaxTree);
            InterpreterResult result = compilation.Interpret(new Dictionary<VariableSymbol, object>());

            string[] diagnostics = AnnotatedText.UnindentLines(diagnosticsText);
            if (annotatedText.spans.Length != diagnostics.Length)
            {
                throw new Exception("ERROR: Must mark as many spans as there are expected diagnostics");
            }

            Assert.Equal(diagnostics.Length, result.diagnostics.Length);

            for (int i = 0; i < diagnostics.Length; i++)
            {
                string expectedMessage = diagnostics[i];
                string actualMessage = result.diagnostics[i].message;

                Assert.Equal(expectedMessage, actualMessage);

                TextSpan expectedSpan = annotatedText.spans[i];
                TextSpan actualSpan = result.diagnostics[i].span;

                Assert.Equal(expectedSpan, actualSpan);
            }
        }
    }
}
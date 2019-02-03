using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace DopeDb.Shared.Cli
{
    public class Util
    {

        static Dictionary<string, FormattingStyle> knownStyles = new Dictionary<string, FormattingStyle>(){
            { "error", new FormattingStyle(ConsoleColor.DarkRed) },
            { "info", new FormattingStyle(ConsoleColor.DarkGreen) },
            { "notice", new FormattingStyle(ConsoleColor.Cyan) },
        };

        /// <summary>
        /// Print a string containing formatting-tags like Symfony uses.
        /// Builtin styles are <i>error</i>, <i>info</i> and <i>notice</i>.
        /// Custom styles can be applied with setting <i>fg=...</i> or <i>bg=...</i> respectively to names
        /// of ConsoleColor values. fg and bg are separated with a semicolon (;).
        /// Examples:
        /// <pre>
        /// "Something <error>failed</error>"
        /// "Some <bg=Cyan>cyan text</>"
        /// "<fg=White;bg=Green>White text on green background</>"
        /// </pre>
        /// </summary>
        /// <param name="text"></param>
        /// <param name="indent">Optionally indent the text</param>
        public static void Write(string text, int indent = 0)
        {
            for (int i = 0; i < indent; i++)
            {
                Console.Write(' ');
            }
            if (text.Split("<").Length < 3)
            {
                Console.Write(text);
                return;
            }
            var formattingStack = new Stack<FormattingStyle>();
            formattingStack.Push(new FormattingStyle(Console.ForegroundColor, Console.BackgroundColor));
            var tagStack = new Stack<string>();
            for (int i = 0; i < text.Length; i++)
            {
                var c = text[i];
                if (c == '<')
                {
                    var tagEnd = text.IndexOf('>', i);
                    if (tagEnd < 0)
                    {
                        continue;
                    }
                    var tagContent = text.Substring(i + 1, tagEnd - i - 1);
                    if (tagContent[0] == '/')
                    {
                        if (tagStack.Count == 0)
                        {
                            Console.Write("<");
                            continue;
                        }
                        var closingTag = $"<{tagContent}>";
                        if (closingTag == tagStack.Peek())
                        {
                            tagStack.Pop();
                            formattingStack.Pop();
                            var currentStyle = formattingStack.Peek();
                            Console.ForegroundColor = currentStyle.Foreground;
                            Console.BackgroundColor = currentStyle.Background;
                            i += tagContent.Length + 1;
                            continue;
                        }
                    }
                    if (knownStyles.ContainsKey(tagContent))
                    {
                        var endTag = $"</{tagContent}>";
                        var hasEndTag = text.IndexOf(endTag, i + 1) > 0;
                        if (hasEndTag)
                        {
                            var nextStyle = knownStyles[tagContent];
                            formattingStack.Push(nextStyle);
                            Console.ForegroundColor = nextStyle.Foreground;
                            Console.BackgroundColor = nextStyle.Background;
                            i += tagContent.Length + 1;
                            tagStack.Push(endTag);
                            continue;
                        }
                    }
                    else if (tagContent.Contains("="))
                    {
                        var endTag = "</>";
                        var hasEndTag = text.IndexOf(endTag, i + 1) > 0;
                        if (hasEndTag)
                        {
                            var assignments = tagContent.Split(";");
                            var foreground = formattingStack.Peek().Foreground;
                            var background = formattingStack.Peek().Background;
                            foreach (var assignment in assignments)
                            {
                                var parts = assignment.Split("=");
                                if (parts[0] == "fg")
                                {
                                    foreground = Util.StringToConsoleColor(parts[1]);
                                }
                                else if (parts[0] == "bg")
                                {
                                    background = Util.StringToConsoleColor(parts[1]);
                                }
                            }
                            var nextStyle = new FormattingStyle(foreground, background);
                            formattingStack.Push(nextStyle);
                            Console.ForegroundColor = nextStyle.Foreground;
                            Console.BackgroundColor = nextStyle.Background;
                            i += tagContent.Length + 1;
                            tagStack.Push(endTag);
                            continue;
                        }
                    }
                }
                Console.Write(c);
            }
            Console.ResetColor();
        }

        public static void Write(object obj)
        {
            printObject(obj);
        }

        /// <see cref="Util.Write"/>
        /// <param name="text"></param>
        /// <param name="indent">Optionally indent the text</param>
        public static void WriteLine(string text = "", int indent = 0)
        {
            Util.Write(text + '\n', indent);
            Console.ResetColor();
        }

        public static void WriteLine(object obj)
        {
            printObject(obj);
            WriteLine();
        }

        public static void WriteException(System.Exception exception)
        {
            Util.WriteLine($"<error>[{exception.GetType()}]</error> {exception.Message}");
            if (exception.InnerException != null)
            {
                Util.WriteLine();
                Util.WriteException(exception.InnerException);
            }
        }

        public static ConsoleColor StringToConsoleColor(string color, ConsoleColor fallback = ConsoleColor.White)
        {
            try
            {
                return (ConsoleColor)Enum.Parse(typeof(ConsoleColor), color);
            }
            catch (System.Exception)
            {
                return fallback;
            }
        }

        protected static void printObject(object obj, int depth = 0)
        {
            var indentationWidth = 2;
            var maxDepth = 3;
            if (obj == null)
            {
                WriteLine(" <fg=Blue>null</>");
            }
            else if (obj is bool)
            {
                var lbl = (bool)obj ? "true" : "false";
                WriteLine($" <fg=Blue>{lbl}</>");
            }
            else if (obj is string)
            {
                WriteLine($" <fg=DarkYellow>\"{obj}\" ({((string)obj).Length})</>");
            }
            else if (obj is int || obj is double)
            {
                WriteLine($" <fg=DarkMagenta>{obj}</>");
            }
            else if (obj.GetType().IsGenericType && obj.GetType().GetGenericTypeDefinition() == typeof(Dictionary<,>))
            {
                WriteLine($" <fg=Yellow>{obj.GetType()}</>");
                foreach (var x in (dynamic)obj)
                {
                    var item = (dynamic)x;
                    Write($"{item.Key} =>", (depth + 1) * indentationWidth);
                    printObject(item.Value, depth + 2);
                }
            }
            else if (obj is IEnumerable<object>)
            {
                WriteLine($" <fg=Yellow>{obj.GetType()}</>");
                var n = 0;
                foreach (var x in (IEnumerable<object>)obj)
                {
                    Write($"{n++} =>", (depth + 1) * indentationWidth);
                    printObject(x, depth + 2);
                }
            }
            else if (obj.GetType().IsClass)
            {
                WriteLine($" <fg=Yellow>{obj.GetType()}</>");
                if (depth < maxDepth)
                {
                    foreach (var property in obj.GetType().GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance))
                    {
                        Write($"{property.Name}:", (depth + 1) * indentationWidth);
                        printObject(property.GetValue(obj), depth + 1);
                    }
                    foreach (var member in obj.GetType().GetMembers(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance))
                    {
                        if (member.MemberType != MemberTypes.Field && member.MemberType != MemberTypes.Property)
                            continue;
                        Write($"{member.Name}:", (depth + 1) * indentationWidth);
                        if (member is FieldInfo)
                        {
                            printObject(((FieldInfo)member).GetValue(obj), depth + 1);
                        }
                        else
                        {
                            WriteLine($" <fg=Yellow>{obj.GetType()}</>");
                        }
                    }
                }
            }
            else
            {
                WriteLine($" <fg=Yellow>{obj.GetType()}</>");
            }
        }

        struct FormattingStyle
        {
            public FormattingStyle(ConsoleColor foreground = ConsoleColor.White, ConsoleColor background = ConsoleColor.Black)
            {
                this.Foreground = foreground;
                this.Background = background;
            }

            public ConsoleColor Foreground;

            public ConsoleColor Background;

            override public string ToString()
            {
                return $"({Foreground}, {Background})";
            }
        }
    }
}
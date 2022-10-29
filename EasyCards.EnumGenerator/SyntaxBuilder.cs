using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Linq;
using System.Text;

namespace EasyCards.EnumGenerator
{
    public sealed class SyntaxBuilder
    {
        private int _blockDepth = 0;
        private readonly StringBuilder _builder = new StringBuilder();
        public string ClassName { get; set; }

        public int Depth => _blockDepth;
        public static SyntaxBuilder FromString(string text)
        {
            var builder = new SyntaxBuilder();
            foreach (var line in text.Split('\r', '\n'))
            {
                switch (line)
                {
                    case "{":
                        builder.BeginBlock();
                        break;
                    case "}":
                        builder.EndBlock();
                        break;
                    default:
                        builder.AppendLine(line);
                        break;
                }
            }

            return builder;
        }

        public void Reset()
        {
            _builder.Clear();
            _blockDepth = 0;
        }

        public SyntaxBuilder BeginBlock(string statement)
        {
            _builder.AppendLine(statement);
            return BeginBlock();
        }
        public SyntaxBuilder BeginBlock()
        {
            _builder.AppendLine("{");
            _blockDepth++;
            return this;
        }

        public SyntaxBuilder EndBlock(string statement)
        {
            _builder.AppendLine(statement);
            return EndBlock();
        }
        public SyntaxBuilder EndBlock()
        {
            if (_blockDepth == 0)
                throw new InvalidOperationException("No blocks are in progress");
            _builder.AppendLine("}");
            _blockDepth--;
            return this;
        }

        public SyntaxBuilder EndAllBlocks()
        {
            while (_blockDepth > 0)
            {
                EndBlock();
            }

            return this;
        }

        public SyntaxBuilder Append(string statement)
        {
            _builder.Append(statement);
            return this;
        }

        public SyntaxBuilder AppendLine(string statement)
        {
            Append(statement);
            return AppendLine();
        }

        public SyntaxBuilder AppendLine()
        {
            _builder.AppendLine();
            return this;
        }

        public override string ToString()
        {
            return _builder.ToString();
        }

        private void FormatCode()
        {
            var builder = new StringBuilder();
            using (var stringWriter = new StringWriter(builder))
            using (var indentedTextWriter = new IndentedTextWriter(stringWriter, tabString: "    "))
            {
                foreach (var line in _builder.ToString().Split('\n').Select(i => i.Trim('\r', ' ')))
                {
                    if (line == "}")
                    {
                        indentedTextWriter.Indent -= 1;
                    }

                    indentedTextWriter.WriteLine(line);

                    if (line == "{")
                    {
                        indentedTextWriter.Indent += 1;
                    }
                }

                indentedTextWriter.Flush();
            }

            _builder.Clear();
            _builder.Append(builder);
        }
        public SyntaxBuilder Format()
        {
            FormatCode();
            return this;
        }

        public SyntaxBuilder PrependBlock()
        {
            _blockDepth++;
            PrependLine();
            _builder.Insert(0, "{");
            return this;
        }

        public SyntaxBuilder PrependLine(string s)
        {
            PrependLine();
            _builder.Insert(0, s);
            return this;
        }
        public SyntaxBuilder PrependLine()
        {
            _builder.Insert(0, "\r\n");
            return this;
        }
        public SyntaxBuilder PrependBlock(string s)
        {
            PrependBlock();
            PrependLine();
            _builder.Insert(0, s);
            return this;
        }
    }
}

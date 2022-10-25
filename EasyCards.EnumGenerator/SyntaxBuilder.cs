using System;
using System.Text;

namespace EasyCards.EnumGenerator
{
    public class SyntaxBuilder
    {
        private int _blockDepth = 0;
        private readonly StringBuilder _builder = new StringBuilder();

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
            if (_blockDepth == 0) throw new InvalidOperationException("No blocks are in progress");
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
    }
}
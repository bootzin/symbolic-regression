using System;
using System.Collections.Generic;
using System.Text;

namespace tp1.Operands
{
    public class Addition : IOperand
    {
        private IOperand Op1 { get; }
        private IOperand Op2 { get; }

        public Addition(IOperand op1, IOperand op2)
        {
            this.Op1 = op1;
            this.Op2 = op2;
        }

        public double Compute(params double[] value)
        {
            return Op1.Compute(value) + Op2.Compute(value);
        }
    }
}

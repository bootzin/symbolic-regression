using System;

namespace tp1.Operands
{
    public class Pow : IOperand
    {
        public IOperand Op1 { get; set; }
        public IOperand Op2 { get; set; }

        public Pow(IOperand op1, IOperand op2)
        {
            Op1 = op1;
            Op2 = op2;
        }

        public double Compute(params double[] value)
        {
            if (Op1.Compute(value) == 0 || (Op1.Compute(value) < 0 && Math.Abs(Op2.Compute(value) % 1) > (double.Epsilon * 100)))
            {
                return 1;
            }
            else
            {
                return Math.Pow(Op1.Compute(value), Op2.Compute(value));
            }
        }
    }
}

namespace tp1.Operands
{
    public class Subtraction : IOperand
    {
        private IOperand Op1 { get; }
        private IOperand Op2 { get; }

        public Subtraction(IOperand op1, IOperand op2)
        {
            this.Op1 = op1;
            this.Op2 = op2;
        }

        public double Compute(params double[] value)
        {
            return Op1.Compute(value) - Op2.Compute(value);
        }
    }
}

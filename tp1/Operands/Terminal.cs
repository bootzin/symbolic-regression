namespace tp1.Operands
{
    public class Terminal : IOperand
    {
        private double Value { get; }
        public string Label { get; }
        public bool IsVariable { get; } = true;

        public Terminal(string op1)
        {
            this.Label = op1;
            if (double.TryParse(op1, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out double db1))
            {
                this.Value = db1;
                IsVariable = false;
            }
            else
            {
                this.Value = double.MinValue;
            }
        }

        public double Compute(params double[] value)
        {
            if (IsVariable)
                return value[int.Parse(Label[1].ToString())];
            return Value;
        }
    }
}

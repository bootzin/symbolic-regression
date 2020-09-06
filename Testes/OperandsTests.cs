using tp1.Operands;
using Xunit;

namespace Testes
{
    public class OperandsTests
    {
        [Theory]
        [InlineData("1","2", 1, 2)]
        [InlineData("0.1","0.2", 0.1, 0.2)]
        [InlineData("0.000000001", "0.000000002", 0.000000001, 0.000000002)]
        [InlineData("0","1", 0, 1)]
        public void Constants(string x1, string x2, double r1, double r2)
        {
            Terminal op1 = new Terminal(x1);
            Terminal op2 = new Terminal(x2);
            Mult m = new Mult(op1, op2);
            Div d = new Div(op1, op2);
            Addition a = new Addition(op1, op2);
            Subtraction s = new Subtraction(op1, op2);

            Assert.True(m.Compute(0) == r1 * r2);
            Assert.True(a.Compute(0) == r1 + r2);
            Assert.True(s.Compute(0) == r1 - r2);
            Assert.True(d.Compute(0) == r1 / r2);
        }

        [Fact]
        public void ProtectedDivision()
        {
            Terminal op1 = new Terminal("1");
            Terminal op2 = new Terminal("0");
            Div d = new Div(op1, op2);
            Assert.True(d.Compute(0) == 1);
        }

        [Theory]
        [InlineData(2,1)]
        [InlineData(0.003,2)]
        public void Variables(double x, double y)
        {
            Terminal op1 = new Terminal("x0");
            Terminal op2 = new Terminal("x1");
            Mult m = new Mult(op1, op2);
            Div d = new Div(op1, op2);
            Addition a = new Addition(op1, op2);
            Subtraction s = new Subtraction(op1, op2);

            Assert.True(m.Compute(x, y) == x * y);
            Assert.True(d.Compute(x, y) == x / y);
            Assert.True(a.Compute(x, y) == x + y);
            Assert.True(s.Compute(x, y) == x - y);
        }

        [Theory]
        [InlineData(2)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(227)]
        [InlineData(0.31)]
        [InlineData(0.0004)]
        [InlineData(-1)]
        [InlineData(-0.13)]
        public void Composed(double value)
        {
            Terminal x = new Terminal("x0");
            Terminal one = new Terminal("1");

            Addition a2 = new Addition(one, one); //y
            Mult aa = new Mult(a2, a2); //y^2
            Mult xx = new Mult(x, x); //x^2

            Subtraction s = new Subtraction(xx, aa); // x^2 - y^2

            Addition xay = new Addition(x, a2); // x + y
            Subtraction xsy = new Subtraction(x, a2); // x - y
            Mult f = new Mult(xay, xsy); // (x + y)(x - y)

            Assert.True(f.Compute(value) == (value + 2)*(value - 2));
            Assert.True(s.Compute(value) == (value * value) - 4);
            Assert.True(s.Compute(value) == f.Compute(value)); //(x + y)(x - y) == (x^2 - y^2)

        }
    }
}

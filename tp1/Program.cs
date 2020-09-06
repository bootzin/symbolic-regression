using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using tp1.Operands;

namespace tp1
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            const int maxDepth = 7;
            const int minDepth = 1;
            const int popSize = 250;
            const uint maxGenerations = 250;
            const uint elitism = 5;
            const uint tournamentSize = 50;
            const float growTerminalChance = .5f;
            const float pCrossover = .3f;
            const float pMutation = .5f;

            string[] terminals = new string[] { "1", "0.5" };
            string[] functions = new string[] { "add", "mul", "sub", "div", "pow" };

            List<Tuple<List<float>, float>> Data = new List<Tuple<List<float>, float>>();
            using (StreamReader sr = new StreamReader("concrete.txt"))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    var strData = line.Replace("\t","").Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    List<float> inputs = new List<float>();
                    for (int i = 0; i < strData.Length - 1; i++)
                    {
                        terminals = terminals.Append("x" + i).ToArray();
                        inputs.Add(float.Parse(strData[i], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture));
                    }
                    Data.Add(new Tuple<List<float>, float>(inputs,
                        (float.Parse(strData[strData.Length - 1], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture))));
                }
            }

            var plt = new ScottPlot.Plot();
            for (int i = 0; i < Data.Count; i++)
            {
                for (int j = 0; j < Data[0].Item1.Count; j++)
                {
                    plt.PlotPoint(Data[i].Item1[j], Data[i].Item2, Color.Red, label: "Original Data");
                }
            }

            Population<string> pop = new Population<string>(popSize, tournamentSize, pCrossover, pMutation, elitism, maxDepth, minDepth, functions, terminals, growTerminalChance);
            pop.Init();
            pop.Individuals = CalculateFitness(Data, pop.Individuals, terminals);
            for (int i = 0; i < maxGenerations; i++)
            {
                pop.Evolve();
                pop.Individuals = CalculateFitness(Data, pop.Individuals, terminals);
                if (pop.Individuals.Any(i => i.Fitness <= 0))
                {
                    Console.WriteLine("Solution reached in generation: " + i);
                    break;
                }
            }
            Console.WriteLine("Fitness: " + pop.Individuals[0].Fitness);
            pop.Individuals[0].PrintTree();
            var op = Evaluate(pop.Individuals[0], terminals);

            for (int i = 0; i < Data.Count; i++)
            {
                for (int j = 0; j < Data[0].Item1.Count; j++)
                    plt.Add(plt.PlotPoint(Data[i].Item1[j], op.Compute(Data[i].Item1.Select(d => (double)d).ToArray()), Color.Blue, label: "Estimate"));
            }
            plt.SaveFig("output.png");
        }

        private static List<Tree<string>> CalculateFitness(List<Tuple<List<float>, float>> data, List<Tree<string>> population, string[] terminals)
        {
            foreach (Tree<string> individual in population)
            {
                IOperand op = Evaluate(individual, terminals);
                double totalError = 0, err = 0;
                foreach (Tuple<List<float>, float> tuple in data)
                {
                    double estimate = op.Compute(tuple.Item1.Select(d => (double)d).ToArray());
                    err = Math.Abs(estimate - tuple.Item2);
                    totalError += err;
                }
                if (double.IsNaN(totalError) || double.IsInfinity(totalError))
                {
                    individual.Fitness = double.MaxValue;
                }
                else
                {
                    individual.Fitness = totalError / data.Count;
                }
                if (totalError <= 0.1)
                {

                }
            }
            return population.OrderBy(ind => ind.Fitness).ToList();
        }

        private static IOperand Evaluate(Tree<string> individual, string[] terminals)
        {
            Stack<IOperand> stack = new Stack<IOperand>();
            foreach (var node in individual.PostOrder())
            {
                var item = node.Value;
                if (terminals.Contains(item))
                {
                    stack.Push(new Terminal(item));
                }
                else
                {
                    IOperand op2 = stack.Pop();
                    IOperand op1 = stack.Pop();

                    switch (item)
                    {
                        case "add":
                            stack.Push(new Addition(op1, op2));
                            break;
                        case "sub":
                            stack.Push(new Subtraction(op1, op2));
                            break;
                        case "mul":
                            stack.Push(new Mult(op1, op2));
                            break;
                        case "div":
                            stack.Push(new Div(op1, op2));
                            break;
                        case "pow":
                            stack.Push(new Pow(op1, op2));
                            break;
                    }
                }
            }
            return stack.Peek();
        }
    }
}

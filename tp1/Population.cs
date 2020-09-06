using System;
using System.Collections.Generic;

namespace tp1
{
    public class Population<T>
    {
        public List<Tree<T>> Individuals { get; set; }
        public int MaxPop { get; set; }
        public int MaxDepth { get; set; }
        public int MinDepth { get; set; }
        public uint TournamentSize { get; set; }
        public float pCrossover { get; set; }
        public float pMutation { get; set; }
        public uint ElitismCount { get; set; }
        public T[] Terminals { get; }
        public T[] Functions { get; }
        public float GrowTerminalChance { get; }

        public Population(
            int maxPop,
            uint tournamentSize,
            float pCrossover,
            float pMutation,
            uint elitism,
            int maxDepth,
            int minDepth,
            T[] functions,
            T[] terminals,
            float growTerminalChance)
        {
            MaxPop = maxPop;
            MaxDepth = maxDepth;
            MinDepth = minDepth;
            TournamentSize = tournamentSize;
            this.pCrossover = pCrossover;
            this.pMutation = pMutation;
            ElitismCount = elitism;
            Terminals = terminals;
            Functions = functions;
            GrowTerminalChance = growTerminalChance;
        }

        public void Init()
        {
            Individuals = RampedHalfNHalf(MaxPop, MaxDepth, Terminals, Functions, GrowTerminalChance);
        }

        public void Evolve()
        {
            var nextGen = new List<Tree<T>>(MaxPop);
            var selection = Tournament();
            nextGen.AddRange(Crossover(selection));
            nextGen.AddRange(Mutate(selection));
            nextGen.AddRange(Elitism());
            nextGen.AddRange(RampedHalfNHalf(MaxPop - nextGen.Count, MaxDepth, Terminals, Functions, GrowTerminalChance));
            Individuals.Clear();
            Individuals = nextGen;
        }

        private List<Tree<T>> Tournament()
        {
            var retorno = new List<Tree<T>>();
            for (int i = 0; i < Individuals.Count; i++)
            {
                retorno.Add(PlayTournament(Individuals, TournamentSize));
            }
            return retorno;
        }

        private Tree<T> PlayTournament(List<Tree<T>> popList, uint tourSize)
        {
            Random rand = new Random();
            var indexes = new HashSet<int>();
            var best = popList[0];
            for (var i = 0; i < tourSize; i++)
            {
                // selects individual at random from pop (no repetition)
                int index;
                do
                {
                    index = rand.Next(popList.Count);
                } while (indexes.Contains(index));

                indexes.Add(index);

                // checks max fitness
                var individual = popList[index];
                double diff = individual.Fitness.CompareTo(best.Fitness);
                if (diff == 0 ? -individual.NodeCount.CompareTo(best.NodeCount) > 0 : (diff > 0))
                    best = individual;
            }

            return best;
        }

        private List<Tree<T>> Elitism()
        {
            var retorno = new List<Tree<T>>();
            for (int i = 0; i < ElitismCount; i++)
            {
                retorno.Add(Individuals[i]);
            }
            return retorno;
        }

        //SubTree Crossover
        private List<Tree<T>> Crossover(List<Tree<T>> selection)
        {
            Random rand = new Random();
            int crossoverAmount = (int)(MaxPop * pCrossover);
            List<Tree<T>> retorno = new List<Tree<T>>(crossoverAmount);
            for (int i = 0; i < crossoverAmount; i++)
            {
                var parent1 = selection[rand.Next(selection.Count)];
                var parent2 = selection[rand.Next(selection.Count)];
                var child = GetCrossover(parent1, parent2);
                if (child.Level <= MaxDepth)
                    retorno.Add(child);
            }
            return retorno;
        }

        private Tree<T> GetCrossover(Tree<T> parent1, Tree<T> parent2)
        {
            Random rand = new Random();
            int commonLevel = rand.Next(Math.Min(parent1.Level, parent2.Level));
            int swapIndex = rand.Next((int)Math.Pow(2, commonLevel), (int)Math.Pow(2, commonLevel + 1) - 1);

            var child1 = new Tree<T>();
            child1.SetRoot(parent1.Root, parent1.Level, parent1.NodeCount);
            var child2 = new Tree<T>();
            child2.SetRoot(parent2.Root, parent2.Level, parent2.NodeCount);

            Node<T> node1 = child1.GetNode(swapIndex);
            var temp = new Tree<T>();
            temp.SetRoot(node1, 0, 0);
            Node<T> node2 = child2.GetNode(swapIndex);
            node1.Replace(node2);
            node2.Replace(temp.Root);
            child1.RecalculateCountAndDepth();
            return child1;
        }

        // Point Mutation
        private List<Tree<T>> Mutate(List<Tree<T>> selection)
        {
            Random rand = new Random();
            int mutationsAmount = (int)pMutation * MaxPop;
            List<Tree<T>> ret = new List<Tree<T>>();
            for (int i = 0; i < mutationsAmount; i++)
            {
                var t = selection[rand.Next(selection.Count)];
                var node = t.GetNode(rand.Next(t.NodeCount));
                if (node.IsLeaf)
                {
                    node.Value = Terminals[rand.Next(Terminals.Length)];
                }
                else
                {
                    node.Value = Functions[rand.Next(Functions.Length)];
                }
                ret.Add(t);
            }
            return ret;
        }

        private List<Tree<T>> RampedHalfNHalf(int popSize, int maxDepth, T[] terminals, T[] functions, float growTerminalChance)
        {
            Random rand = new Random();
            List<Tree<T>> population = new List<Tree<T>>();

            //full
            for (uint i = 0; i < popSize / 2; i++)
            {
                int localMax = rand.Next(MinDepth, maxDepth);
                Tree<T> tree = new Tree<T>();
                for (uint j = 0; j <= localMax; j++)
                {
                    for (int k = 0; k < Math.Pow(2, j); k++)
                    {
                        T data;
                        if (j == localMax)
                        {
                            data = terminals[rand.Next(terminals.Length)];
                        }
                        else
                        {
                            data = functions[rand.Next(functions.Length)];
                        }
                        tree.AddNode(data);
                    }
                }
                population.Add(tree);
            }

            //grow
            for (uint i = 0; i < popSize / 2; i++)
            {
                var tree = GenerateGrow(terminals, functions, 0, maxDepth, growTerminalChance);
                population.Add(tree);
            }
            return population;
        }

        private Tree<T> GenerateGrow(T[] terminals, T[] functions, uint depth, int maxDepth, float growTerminalChance)
        {
            Random rand = new Random();
            // check max depth, just return a random terminal program
            if (depth == maxDepth)
            {
                var t = new Tree<T>();
                t.AddNode(terminals[rand.Next(terminals.Length)], true);
                return t;
            }

            // otherwise, get a random primitive
            int numChildren;
            var ret = new Tree<T>();
            if (rand.NextDouble() > growTerminalChance)
            {
                ret.AddNode(functions[rand.Next(functions.Length)]);
                numChildren = 2;
            }
            else
            {
                ret.AddNode(terminals[rand.Next(terminals.Length)], true);
                numChildren = 0;
            }

            // recursively generate random children for it
            for (var i = 0; i < numChildren; i++)
            {
                ret.AddNode(GenerateGrow(terminals, functions, depth + 1, maxDepth, growTerminalChance).Root);
            }
            return ret;
        }
    }
}

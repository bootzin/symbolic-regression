using System;
using System.Collections.Generic;

namespace tp1
{
    public class Tree<T>
    {
        public double Fitness { get; set; }
        public int NodeCount { get; private set; }
        public int Level { get; private set; }
        public Node<T> Root { get; private set; }

        public Tree(params T[] data)
        {
            Root = BuildTree(data, 0);
        }

        public Tree(T data)
        {
            Root = new Node<T>(data);
            NodeCount = 1;
        }

        public void SetRoot(Node<T> newRoot, int level, int nodeCount)
        {
            Root = new Node<T>(newRoot);
            NodeCount = nodeCount;
            Level = level;
        }

        public void AddNode(Node<T> node)
        {
            NodeCount++;
            if (Root == null)
            {
                Root = node;
                return;
            }
            Node<T> current = Root;
            if (NodeCount >= Math.Pow(2, Level + 1)) Level++;
            for (int n = Level - 1; n > 0; n--)
                current = CheckBit(NodeCount, n) ? current.Left : current.Right;
            if (CheckBit(NodeCount, 0))
                current.Left = node;
            else
                current.Right = node;
        }

        public void AddNode(T data, bool isLeaf = false)
        {
            NodeCount++;
            if (Root == null)
            {
                Root = new Node<T>(data);
                return;
            }
            Node<T> current = Root;
            if (NodeCount >= Math.Pow(2, Level + 1)) Level++;
            for (int n = Level - 1; n > 0; n--)
                current = CheckBit(NodeCount, n) ? current.Left : current.Right;
            if (CheckBit(NodeCount, 0))
                current.Left = new Node<T>(data, isLeaf);
            else
                current.Right = new Node<T>(data, isLeaf);
        }

        public Node<T> GetNode(int index)
        {
            return GetNode(Root, index);
        }

        private Node<T> GetNode(Node<T> node, int index)
        {
            int i = 0;
            Queue<Node<T>> queue = new Queue<Node<T>>();
            queue.Enqueue(node);
            Node<T> tempNode = node;
            while (queue.Count != 0 && i < index)
            {
                tempNode = queue.Dequeue();

                /*Enqueue left child */
                if (tempNode.Left != null)
                {
                    i++;
                    queue.Enqueue(tempNode.Left);
                    if (i == index)
                    {
                        tempNode = queue.Dequeue();
                        break;
                    }
                }

                /*Enqueue right child */
                if (tempNode.Right != null)
                {
                    i++;
                    queue.Enqueue(tempNode.Right);
                    if (i == index)
                    {
                        tempNode = queue.Dequeue();
                        break;
                    }
                }
            }
            return tempNode;
        }

        public void PrintTree()
        {
            PrintTree(Root, "", false);
        }

        public void PrintTree(Node<T> tree, string indent, bool last)
        {
            Console.WriteLine(indent + "+- " + tree.Value);
            indent += last ? "   " : "|  ";

            if (!last)
            {
                if (tree.Left != null)
                    PrintTree(tree.Left, indent, tree.IsLeaf);
                if (tree.Right != null)
                    PrintTree(tree.Right, indent, tree.IsLeaf);
            }
        }

        public IEnumerable<Node<T>> PostOrder()
        {
            return DoPostOrder(Root);
        }

        private IEnumerable<Node<T>> DoPostOrder(Node<T> node)
        {
            if (node == null)
            {
                yield break;
            }

            if (node.Left != null)
            {
                foreach (var leftNode in DoPostOrder(node.Left))
                {
                    yield return leftNode;
                }
            }

            if (node.Right != null)
            {
                foreach (var rightNode in DoPostOrder(node.Right))
                {
                    yield return rightNode;
                }
            }

            yield return node;
        }

        public Node<T> BuildTree(T[] data, int index)
        {
            if (index >= data.Length) return null;
            Node<T> next = new Node<T>(data[index]);
            next.Left = BuildTree(data, (2 * index) + 1);
            next.Right = BuildTree(data, (2 * index) + 2);
            return next;
        }

        private bool CheckBit(int num, int position)
        {
            return ((num >> position) & 1) == 0;
        }

        public void RecalculateCountAndDepth()
        {
            NodeCount = 0;
            Level = 0;
            foreach (var _ in PostOrder())
            {
                NodeCount++;
                if (NodeCount >= Math.Pow(2, Level + 1)) Level++;
            }
        }
    }

    public class Node<T>
    {
        public T Value { get; set; }
        public bool IsLeaf { get; set; }
        public Node<T> Left { get; set; }
        public Node<T> Right { get; set; }

        public Node(T value, bool isLeaf = false)
        {
            Value = value;
            IsLeaf = isLeaf;
        }

        public Node(Node<T> node)
        {
            this.Replace(node);
        }

        public void Replace(Node<T> node)
        {
            this.Value = node.Value;
            this.IsLeaf = node.IsLeaf;
            this.Left = node.Left;
            this.Right = node.Right;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonteCarloTreeSearch.Tree
{
    public class Node<T> : INode<T>
    {
        private readonly List<Node<T>> _children = new List<Node<T>>();

        public Node(T value)
        {
            Value = value;
        }

        public T Value { get; }
        public INode<T> Parent { get; private set; }
        public IEnumerable<INode<T>> Children => _children;
        public INode<T> AddChild(T child)
        {
            var node = new Node<T>(child)
            {
                Parent = this
            };

            _children.Add(node);

            return node;
        }

        public IEnumerable<INode<T>> GetAllDescendants()
        {
            var list = new List<INode<T>>();

            list.AddRange(_children);

            foreach (var child in _children)
            {
                list.AddRange(child.GetAllDescendants());
            }

            return list;
        }

        public int GetNumberOfDescendants()
        {
            return _children.Count + _children.Sum(GetNumberOfDescendants);
        }

        private int GetNumberOfDescendants(Node<T> node)
        {
            return node._children.Count + node._children.Sum(GetNumberOfDescendants);
        }

        public override string ToString()
        {
            return $"{nameof(Children)} Objects: {_children.Count}, Descendant Objects: {GetNumberOfDescendants() - 2}, [{Value}]";
        }
    }
}

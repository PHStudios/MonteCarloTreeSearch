using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonteCarloTreeSearch.Tree
{
    public interface INode<T>
    {
        public T Value { get; }
        public INode<T> Parent { get; }
        public IEnumerable<INode<T>> Children { get; }
        public bool IsLeafNode => !Children.Any();

        public INode<T> AddChild(T child);

        public IList<INode<T>> AddChildren(IEnumerable<T> children)
        {
            var result = new List<INode<T>>();

            foreach (var child in children)
            {
                var addedChild = AddChild(child);

                result.Add(addedChild);
            }

            return result;
        }

        public IEnumerable<INode<T>> GetAllDescendants();
        public int GetNumberOfDescendants();
    }
}

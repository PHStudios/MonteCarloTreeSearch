using System;
using System.Collections.Generic;
using System.Text;

namespace MonteCarloTreeSearch.Tree
{
    public class Tree<T>
    {
        public INode<T> Root { get; }

        public Tree(INode<T> root)
        {
            Root = root;
        }
    }
}

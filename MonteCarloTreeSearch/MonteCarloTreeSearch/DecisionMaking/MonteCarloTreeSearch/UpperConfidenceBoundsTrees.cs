using System;
using System.Collections.Generic;
using System.Text;
using MonteCarloTreeSearch.Tree;

namespace MonteCarloTreeSearch.DecisionMaking.MonteCarloTreeSearch
{
    internal class UpperConfidenceBoundsTrees
    {
        private const int _constant = 2;

        internal double Calculate(INode<IState> state)
        {
            if (state.Parent == null)
            {
                return -1;
            }

            if (state.Value.Visits == 0)
            {
                return double.PositiveInfinity;
            }

            return (state.Value.Score / (double) state.Value.Visits) +
                   _constant * Math.Sqrt(Math.Log(state.Parent.Value.Visits) / state.Value.Visits);
        }
    }
}

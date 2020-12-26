using System;
using System.Collections.Generic;
using System.Text;

namespace MonteCarloTreeSearch.DecisionMaking.MonteCarloTreeSearch
{
    public interface IMonteCarloTreeSearchConstraint
    {
        public string Name { get; }

        public string Status { get; }
        public bool IsAlgorithmValid(MonteCarloTreeSearch mcts);
        public bool IsStateValid(IState state);
        public void Reset();
    }
}

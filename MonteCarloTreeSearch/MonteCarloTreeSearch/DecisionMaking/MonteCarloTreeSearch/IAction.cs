using System;
using System.Collections.Generic;
using System.Text;

namespace MonteCarloTreeSearch.DecisionMaking.MonteCarloTreeSearch
{
    public interface IAction
    {
        public string Name { get; }

        public void Execute(IState currentState);
    }
}

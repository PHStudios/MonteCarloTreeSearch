using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MonteCarloTreeSearch.Tree;

namespace MonteCarloTreeSearch.DecisionMaking.MonteCarloTreeSearch
{
    public class MonteCarloTreeSearchResults
    {
        public MonteCarloTreeSearchResults(INode<IState> rootState, IAction bestAction, List<IMonteCarloTreeSearchConstraint> invalidConstraints, List<IGrouping<string, string>> terminatingStatesGroup)
        {
            RootState = rootState;
            BestAction = bestAction;
            InvalidConstraints = invalidConstraints ?? new List<IMonteCarloTreeSearchConstraint>();
            TerminatingStatesGroup = terminatingStatesGroup;
        }

        public INode<IState> RootState { get; }
        public IAction BestAction { get; }
        public List<IMonteCarloTreeSearchConstraint> InvalidConstraints { get; }
        public List<IGrouping<string, string>> TerminatingStatesGroup { get; }
    }
}

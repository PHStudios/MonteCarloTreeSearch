using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MonteCarloTreeSearch.Tree;

namespace MonteCarloTreeSearch.DecisionMaking.MonteCarloTreeSearch
{
    public class MonteCarloTreeSearch
    {
        private readonly UpperConfidenceBoundsTrees _upperConfidenceBoundsTrees = new UpperConfidenceBoundsTrees();
        private Tree<IState> _tree;
        private readonly List<IMonteCarloTreeSearchConstraint> _constraints;

        public MonteCarloTreeSearch(List<IMonteCarloTreeSearchConstraint> constraints)
        {
            _constraints = constraints;
        }

        public long Iterations { get; private set; }

        public IAction GetNextBestAction(INode<IState> root)
        {
            return GetResults(root).BestAction;
        }

        public MonteCarloTreeSearchResults GetResults(INode<IState> root)
        {
            var availableActions = root.Value.GetAllActions();
            if (availableActions.Count <= 1)
            {
                return new MonteCarloTreeSearchResults(root, availableActions.FirstOrDefault(), null,
                    GetTerminatingStatesGroup());
            }

            ResetConstraints();

            PreapreTree(root);

            PerformAlgorith();

            return GetAlgorithmResults();
        }

        private void ResetConstraints()
        {
            foreach (var constraint in _constraints)
            {
                constraint.Reset();
            }
        }

        private void PrepareTree(INode<IState> root)
        {
            _tree = new Tree<IState>(root);

            Expansion(_tree.Root);
        }

        private static IEnumerable<INode<IState>> Expansion(INode<IState> current)
        {
            var allStates = current.Value.GetAllStates();
            return current.AddChildren(allStates);
        }

        private void PerformAlgorithm()
        {
            Iterations = 0;

            while (_constraints.All(constraint => constraint.IsAlgorithmValid(this)))
            {
                var bestLeafNode = TreeTraversal();

                //Resume video here
            }
        }

        private INode<IState> TreeTraversal()
        {
            var current = _tree.Root;

            while (current.IsLeafNode == false)
            {
                var nonVisitedNode = current.Children.FirstOrDefault(state => state.Value.Visits == 0);
                if (nonVisitedNode != null)
                {
                    return nonVisitedNode;
                }

                current = GetBestNodeByUCB1Score(current);
            }

            return current;
        }

        private INode<IState> GetBestNodeByUCB1Score(INode<IState> current)
        {
            var statesWithUCB1Score = current.Children.Select(state =>
                new {UCB1 = _upperConfidenceBoundsTrees.Calculate(state), State = state});

            var bestNode = statesWithUCB1Score.OrderByDescending(state => state.UCB1).First().State;

            return bestNode;
        }
    }
}

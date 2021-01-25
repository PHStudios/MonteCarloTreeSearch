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

            PrepareTree(root);

            PerformAlgorithm();

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

                if (bestLeafNode.Value.Visits > 0 && bestLeafNode.Value.IsTerminatingState() == false)
                {
                    var newStates = Expansion(bestLeafNode);
                    bestLeafNode = newStates.First();
                }

                int rolloutValue = Rollout(bestLeafNode);
                Backpropagation(bestLeafNode, rolloutValue, 1);

                Iterations++;
            }
        }

        private int Rollout(INode<IState> state)
        {
            return state.Value.Rollout(_constraints);
        }

        private static void Backpropagation(INode<IState> state, int scoreToAdd, int visitsToAdd)
        {
            var current = state;
            do
            {
                lock (current.Value)
                {
                    current.Value.Backpropagation(scoreToAdd, visitsToAdd);
                }

                current = current.Parent;
            } while (current != null);
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

        private List<IGrouping<string, string>> GetTerminatingStatesGroup()
        {
            var nodesWithRolloutPerformed = _tree.Root.GetAllDescendants()
                .Where(node => node.Value.HasRolloutBeenPerformed == true).ToList();

            return nodesWithRolloutPerformed.Select(node => node.Value.RolloutTerminatingStateDetails)
                .GroupBy(details => details).ToList();
        }

        private MonteCarloTreeSearchResults GetAlgorithmResults()
        {
            var invalidConstraints = _constraints.Where(constraint => !constraint.IsAlgorithmValid(this)).ToList();
            var terminatingStatesGroup = GetTerminatingStatesGroup();

            var actionFromParent = _tree.Root.Children.OrderByDescending(child => child.Value.Score).First().Value
                .ActionFromParent;

            return new MonteCarloTreeSearchResults(_tree.Root, actionFromParent, invalidConstraints,
                terminatingStatesGroup);
        }

        public bool IsTreeTerminating()
        {
            if (!_tree.Root.Children.Any() && !_tree.Root.Value.IsTerminatingState())
            {
                return false;
            }

            return _tree.Root.Value.IsTerminatingState() || _tree.Root.GetAllDescendants()
                .Where(descendant => !descendant.Children.Any())
                .All(descendant => descendant.Value.IsTerminatingState());
        }
    }
}

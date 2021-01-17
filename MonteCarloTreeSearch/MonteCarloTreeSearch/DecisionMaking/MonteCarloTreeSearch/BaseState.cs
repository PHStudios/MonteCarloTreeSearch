using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MonteCarloTreeSearch.Extensions;
using MonteCarloTreeSearch.Tree;

namespace MonteCarloTreeSearch.DecisionMaking.MonteCarloTreeSearch
{
    public abstract class BaseState : IState
    {
        protected BaseState(IAction actionFromParent)
        {
            ActionFromParent = actionFromParent;
            HasRolloutBeenPerformed = false;

            RolloutActionHistory = new List<string>();
        }

        public List<string> RolloutActionHistory { get; }

        public long Score { get; private set; }
        public long Visits { get; private set; }
        public int RolloutActions { get; private set; }
        public IAction ActionFromParent { get; private set; }
        public bool HasRolloutBeenPerformed { get; private set; }
        public string RolloutTerminatingStateDetails { get; private set; }
        public int Rollout(List<IMonteCarloTreeSearchConstraint> constraints)
        {
            RolloutActions = 0;
            var child = new Node<BaseState>(this);
            while (!child.Value.IsTerminatingState() &&
                   constraints.All(constraint => constraint.IsStateValid(child.Value)))
            {
                var parent = child;
                child = new Node<BaseState>(parent.Value.CreateChildState());

                PerformRandomAction(child.Value);

                parent.AddChild(child.Value);

                RolloutActions++;
                child.Value.RolloutActions = RolloutActions;
            }

            SetTerminatingStateDetails(child.Value, constraints);
            HasRolloutBeenPerformed = true;

            return child.Value.GetRolloutValue();

        }

        private void PerformRandomAction(BaseState state)
        {
            var allActions = state.GetAllActions();
            var randomAction = allActions.GetRandomValue();
            state.PerformAction(randomAction);

            RolloutActionHistory.Add(randomAction.Name);
        }

        private void PerformAction(IAction action)
        {
            action.Execute(this);
            ActionFromParent = action;
        }

        protected abstract int GetRolloutValue();

        public List<IState> GetAllStates()
        {
            var actions = GetAllActions();
            var result = new List<IState>();

            foreach (var action in actions)
            {
                result.Add(GetChildStateFromAction(action));
            }

            return result;
        }

        private IState GetChildStateFromAction(IAction action)
        {
            var copy = Copy();
            ((BaseState)copy).PerformAction(action);
            ((BaseState) copy).SetAsChildState(this);

            return copy;
        }

        public abstract List<IAction> GetAllActions();

        public void Backpropagation(int scoreToAdd, int visitsToAdd)
        {
            Score += scoreToAdd;
            Visits += visitsToAdd;
        }

        public abstract bool IsTerminatingState();

        protected void SetTerminatingStateDetails(BaseState lastStateFromRollout,
            List<IMonteCarloTreeSearchConstraint> constraints)
        {
            if (!lastStateFromRollout.IsTerminatingState())
            {
                var metConstraints =
                    constraints.Where(constraint => constraint.IsStateValid(lastStateFromRollout) == false);

                RolloutTerminatingStateDetails = string.Join(",",
                    metConstraints.Select(constraint => $"[Terminating Constraint: {constraint.Name}]"));
            }
            else
            {
                var terminatingStates = lastStateFromRollout.GetTerminatingStates();

                RolloutTerminatingStateDetails =
                    string.Join(",", terminatingStates.Select(condition => $"[{condition.Name}]"));
            }
        }

        public abstract List<(string Name, int Score)> GetTerminatingStates();

        protected BaseState CreateChildState()
        {
            var child = (BaseState) Copy();
            child.SetAsChildState(this);

            return child;
        }

        protected abstract void SetAsChildState(BaseState state);

        public abstract IState Copy();

        public override string ToString()
        {
            var result = $"{nameof(Score)}: {Score}, {nameof(Visits)}: {Visits}, Terminating State: {RolloutTerminatingStateDetails}";

            if (ActionFromParent != null) result = $"{nameof(ActionFromParent)}: {ActionFromParent.Name}, " + result;

            return result;
        }
    }
}

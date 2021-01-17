using System;
using System.Collections.Generic;
using System.Text;

namespace MonteCarloTreeSearch.DecisionMaking.MonteCarloTreeSearch
{
    public interface IState
    {
        public long Score { get; }

        public long Visits { get; }

        public int RolloutActions { get; }

        public IAction ActionFromParent { get; }

        public bool HasRolloutBeenPerformed { get; }

        public string RolloutTerminatingStateDetails { get; }

        public int Rollout(List<IMonteCarloTreeSearchConstraint> constraints);
        public List<IState> GetAllStates();
        public List<IAction> GetAllActions();
        public void Backpropagation(int scoreToAdd, int visitsToAdd);
        public bool IsTerminatingState();
        public IState Copy();
    }
}

using Accord.MachineLearning;
using Accord.Math;
using System;
using System.Collections.Generic;
using System.Data;
using PokémonAPI;

namespace PokémonSimulator
{
    /// <summary>
    /// PokéAgents have a mapping from a battle situation to an enumeration of states, as well as a list of actions 
    /// </summary>
    public interface IPokéAgent_old
    {
        int StateMapping(Battle b);
        List<Action> ViableActions(Battle b);
        double[] EstimateRewards();
    }

    public class PolicyImmediateGratification_old : IExplorationPolicy
    {
        public int ChooseAction(double[] actionEstimates)
        {
            //Returns the index estimated to have the highest reward.
            return actionEstimates.ArgMax();
        }
    }
}

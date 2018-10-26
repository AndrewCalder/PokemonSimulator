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
    public interface IPokéAgent
    {
        int StateMapping(Battle b);
        List<Move> ViableMoves(Battle b);
        void EstimateRewards();
        int ChooseMove(Battle b);
    }

    public class PolicyImmediateGratification : IExplorationPolicy
    {
        public int ChooseAction(double[] actionEstimates)
        {
            //Returns the index estimated to have the highest reward.
            return actionEstimates.ArgMax();
        }
    }
}

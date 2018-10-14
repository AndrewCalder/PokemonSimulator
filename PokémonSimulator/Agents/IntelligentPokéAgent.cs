using System;
using System.Collections.Generic;
using System.Text;
using PokémonAPI;

namespace PokémonSimulator
{
    class IntelligentPokéAgent : IPokéAgent
    {
        private double[] rewards;
        private double alpha = 0.1;       //learning rate
        private int lambda = 1;      //eligibility trace length
        private double gamma = 0.9;       //decay rate

        public int StateMapping(Battle b)
        {
            //Use TD-learning to select the best move
            //We assume this pokemon is the AGENT in the Battle
            return 0;
        }

        public List<Move> ViableMoves(Battle b)
        {
            return b.GetAgentMoves();
        }

        public double[] EstimateRewards()
        {
            //TODO
            double[] rewards = new double[4];

            return rewards;
        }
    }
}

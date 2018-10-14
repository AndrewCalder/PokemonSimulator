using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PokémonAPI;

namespace PokémonSimulator
{
    /// <summary>
    /// 
    /// </summary>
    public class Battle
    {
        public Pokémon agent;
        public Pokémon defender;

        public Pokémon DoBattle(Pokémon agentPokémon, IPokéAgent agentAi, Pokémon defenderPokémon, IPokéAgent defenderAi)
        {
            //Store variables
            agent = agentPokémon;
            defender = defenderPokémon;

            //Perform the battle until one pokemon has fainted
            while (true)
            {
                //Fastest pokemon goes first
                //TODO: what do we do if speed is the same?
                if (ComparePokémonSpeed(agent, defender) >= 0)
                {
                    //We are faster
                    //Agent pokemon's turn
                    DoTurn(agent, defender, agentAi);
                        //Did they faint?
                        if (defender.IsFainted) { return agent; }
                    //Opponent pokemon's turn
                    DoTurn(defender, agent, defenderAi);
                }
                else
                {
                    //Opponent is faster
                    //Opponent pokemon's turn
                    DoTurn(defender, agent, defenderAi);
                        //Did we faint?
                        if (agent.IsFainted) { return defender; }
                    //Agent pokemon's turn
                    DoTurn(agent, defender, agentAi);
                }

                //Check if either pokemon has fainted
                //If so, the other pokemon is the winner, so return that pokemon
                if (agent.IsFainted) { return defender; }
                if (defender.IsFainted) { return agent; }

                //TESTING
                return agent;
            }
        }

        private void DoTurn(Pokémon actor, Pokémon enemy, IPokéAgent ai)
        {
            //Perform the turn according to the given AI
            actor.Use(ai.StateMapping(this), enemy);
        }

        private static int ComparePokémonSpeed(Pokémon p1, Pokémon p2)
        {
            if (p1.Stats[PokémonAPI.Stat.Speed] > p1.Stats[PokémonAPI.Stat.Speed])
            {
                //First 'mon is faster than second 'mon
                return 1;
            }
            else if (p1.Stats[PokémonAPI.Stat.Speed] < p1.Stats[PokémonAPI.Stat.Speed])
            {
                //First 'mon is slower than second 'mon
                return -1;
            }
            else
            {
                //First 'mon is just as fast as second 'mon
                return 0;
            }
        }



        //Methods for tracking the state (features)

        //Get the health approximation of agent/defender
        public int GetAgentHealthQuadrant()
        {
            return GetHealthQuadrant(agent);
        }
        public int GetDefenderHealthQuadrant()
        {
            return GetHealthQuadrant(defender);
        }
        private int GetHealthQuadrant(Pokémon p)
        {
            //Get the ratio of pokemon's current health to total health
            double approx = p.RemainingHealth / p.Stats[PokémonAPI.Stat.HP];

            //Put the ratio on a scale of 1 to 4 (instead of 0.0 to 1.0)
            //0 = dead, so we don't have to worry about this case
            //1 = 1-25%
            //2 = 26-50%
            //3 = 51-75%
            //4 = 76-100%
            approx = approx * 4;

            return (int) Math.Ceiling(approx);
        }

        //Get the movesets of agent/defender
        public List<Move> GetAgentMoves()
        {
            return GetMoves(agent);
        }
        public List<Move> GetDefenderMoves()
        {
            return GetMoves(defender);
        }
        private List<Move> GetMoves(Pokémon p)
        {
            return p.Moves;
        }

        //Get the types of agent/defender
        public List<PokémonAPI.Type> GetAgentTypes()
        {
            return GetTypes(agent);
        }
        public List<PokémonAPI.Type> GetDefenderTypes()
        {
            return GetTypes(defender);
        }
        private List<PokémonAPI.Type> GetTypes(Pokémon p)
        {
            return p.Types;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PokémonAPI;
using Accord.MachineLearning;

namespace PokémonSimulator
{
    /// <summary>
    /// 
    /// </summary>
    public class Battle
    {
        public Pokémon agent;
        public Pokémon defender;

        private int agentReward = 0;

        public Pokémon DoBattle(Pokémon agentPokémon, IntelligentPokéAgent agentAi, Pokémon defenderPokémon, IPokéAgent defenderAi)
        {
            //Store variables
            agent = agentPokémon;
            defender = defenderPokémon;

            //Print log to console
            agentAi.StateMapping(this);
            Console.WriteLine("RL STATE NUMBER: " + agentAi.currentState);
            Console.WriteLine("\tLevel " + agent.Level + " " + agent.Species.Name + " has " + agent.RemainingHealth + " health.");
            /*
            Console.WriteLine("\t\tAttack " + agent.Stats[Stat.Attack]);
            Console.WriteLine("\t\tDefense " + agent.Stats[Stat.Defense]);
            Console.WriteLine("\t\tHP " + agent.Stats[Stat.HP]);
            Console.WriteLine("\t\tSpecial " + agent.Stats[Stat.Special]);
            Console.WriteLine("\t\tSpeed " + agent.Stats[Stat.Speed]);
            */
            Console.WriteLine("\tLevel " + defender.Level + " " + defender.Species.Name + " has " + defender.RemainingHealth + " health.");
            /*
            Console.WriteLine("\t\tAttack " + defender.Stats[Stat.Attack]);
            Console.WriteLine("\t\tDefense " + defender.Stats[Stat.Defense]);
            Console.WriteLine("\t\tHP " + defender.Stats[Stat.HP]);
            Console.WriteLine("\t\tSpecial " + defender.Stats[Stat.Special]);
            Console.WriteLine("\t\tSpeed " + defender.Stats[Stat.Speed]);
            */

            //Perform the battle until one pokemon has fainted
            bool weFainted = false;
            bool theyFainted = false;
            while (true)
            {
                //Fastest pokemon goes first
                //TODO: what do we do if speed is the same?
                if (ComparePokémonSpeed(agent, defender) >= 0)
                {
                    //We are faster
                    //Agent pokemon's turn
                    agentAi.StartNewTurnEpisode();
                    agentReward = DoTurn(agent, defender, agentAi);
                    agentAi.ApplyRewardDealDamage(agentReward);

                    //~~~~~~~~~~~~~~~TESTING: what if ice beam is much more effective?~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                    //if (agentAi.lastAction == 0) { defender.Damage += 100; }

                    //Did they faint?
                    if (defender.IsFainted) { theyFainted = true; }
                    else
                    {
                        //Opponent pokemon's turn
                        agentReward = DoTurn(defender, agent, defenderAi);
                        agentAi.ApplyRewardTakeDamage(agentReward);

                        //Did we faint?
                        if (agent.IsFainted) { weFainted = true; }
                    }
                }
                else
                {
                    //Opponent is faster
                    //Opponent pokemon's turn
                    agentReward = DoTurn(defender, agent, defenderAi);
                    agentAi.ApplyRewardTakeDamage(agentReward);

                    //Did we faint?
                    if (agent.IsFainted) { weFainted = true; }
                    else
                    {
                        //Agent pokemon's turn
                        agentAi.StartNewTurnEpisode();
                        agentReward = DoTurn(agent, defender, agentAi);
                        agentAi.ApplyRewardDealDamage(agentReward);

                        //Did they faint?
                        if (defender.IsFainted) { theyFainted = true; }
                    }
                }

                //If someone fainted, assign additional reward for winning or losing the battle
                if (weFainted) { agentAi.ApplyRewardLose(); }
                if (theyFainted) { agentAi.ApplyRewardWin(); }

                //Update the learner
                agentAi.qlearn.UpdateState(agentAi.lastState, agentAi.lastAction,
                    agentAi.lastReward);//, agentAi.currentState);
                Console.WriteLine("\t\tQL: From state " + agentAi.lastState + " to state " + agentAi.currentState +
                    " by action " + agentAi.lastAction + " for reward " + agentAi.lastReward + ".");

                //If someone fainted, break the loop
                if (weFainted || theyFainted) { break; }
            }

            //The battle is over
            //Return the winning pokemon
            if (weFainted) { return defender; }
            else { return agent; }
        }

        private int DoTurn(Pokémon actor, Pokémon enemy, IPokéAgent ai)
        {
            //Perform the turn according to the given AI
            int damageDone = actor.Use(ai.ChooseMove(this), enemy);

            //Print out remaining HP of enemy
            Console.WriteLine("\t" + enemy.Species.Name + " has " + enemy.RemainingHealth + " health remaining.");

            return damageDone;
        }

        private Pokémon WinThisBattle(IntelligentPokéAgent agentAi)
        {
            //Apply reward
            agentAi.ApplyRewardWin();

            //Update the learner

            //Return the winning pokemon
            return agent;
        }

        private Pokémon LoseThisBattle(IntelligentPokéAgent agentAi)
        {
            //Apply reward
            agentAi.ApplyRewardLose();

            //Update the learner

            //Return the winning pokemon
            return defender;
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

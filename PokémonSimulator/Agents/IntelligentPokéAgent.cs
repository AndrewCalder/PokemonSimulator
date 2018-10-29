using System;
using System.Collections.Generic;
using System.Text;
using PokémonAPI;
using Accord.MachineLearning;

namespace PokémonSimulator
{
    public class IntelligentPokéAgent : IPokéAgent
    {
        //parameters
        public const double ALPHA = 0.1;       //initial learning rate
        public const int LAMBDA = 1;           //eligibility trace length
        public const double GAMMA = 0.9;       //decay rate
        public const double EPSILON = 0.5;     //initial exploration rate
        public double variableEpsilon = EPSILON;
        public const double EPSILON_DECAY = 0.9;
        public const int STATE_INITIAL_REWARD = 0;
        public const int REWARD_DEAL_DAMAGE = 1;//2;    //reward multiplier per damage dealt to opponent
        public const int REWARD_TAKE_DAMAGE = 0;    //reward multiplier per damage taken
        public const int REWARD_WIN = 100;//200;     //reward for winning a battle
        public const int REWARD_LOSE = 0;//-1000;   //penalty (negative reward) for losing a battle

        public const uint STATE_SPACE = 960400;       //5*5*4*4*7*7*7*7      //50625;        //15^8
        public const int ACTION_SPACE = 4;

        //reward tracking
        private readonly int[,] stateActionValue = new int[STATE_SPACE, ACTION_SPACE];
        private readonly int[] stateValue = new int[STATE_SPACE];
        public List<int> myBattleRewards = new List<int>();
        public List<int> myTurnRewards = new List<int>();


        //Comparison algorithm - a SARSA algorithm from accord.net machine learning api
        public Sarsa comparisonAlgorithm;// = new Sarsa(STATE_SPACE, ACTION_SPACE,new EpsilonGreedyExploration(EPSILON));

        public int currentState = 0;
        public int lastState = 0;
        public int lastReward = 0;
        public int lastAction = 0;

        public int StateMapping(Battle b)
        {
            //For our state, we consider:
            //      The agent's health quartile
            //      The enemy's health quartile
            //      The enemy's first type
            //      The enemy's second type (note: if enemy has no second type, 
            //      The type of move 1
            //      The type of move 2
            //      The type of move 3
            //      The type of move 4
            //With 15 different possibilities for each of these types, our state space's size is:
            //      15^8        = 2 562 890 625        ~= 10^9
            //However, our sample battle will be much simpler
            //      there are only 4 unique enemies (4 possible enemy type combinations)
            //      moves are set in stone, so there is only 1 possible type for each move
            //      thus, our practical state space is much lower than 15^8

            //We will use this information to construct a number that represents the state
            int stateNum = 0;
            int digitMult = 7;    //15;     //should keep this the same

            //Get the opponent's types
            int agentHealthQuartile = b.GetAgentHealthQuadrant();
            int opponentHealthQuartile = b.GetDefenderHealthQuadrant();
            List<PokémonAPI.Type> opponentTypes = b.GetDefenderTypes();
            int opponentTypeIndex1 = (int) opponentTypes[0];
            int opponentTypeIndex2 = opponentTypeIndex1;
            if (opponentTypes.Count > 1) { opponentTypeIndex2 = (int) opponentTypes[1]; }
            int move1TypeIndex = (int) b.GetAgentMoves()[0].AttackType;
            int move2TypeIndex = (int) b.GetAgentMoves()[1].AttackType;
            int move3TypeIndex = (int) b.GetAgentMoves()[2].AttackType;
            int move4TypeIndex = (int) b.GetAgentMoves()[3].AttackType;

            //this structure will help us build the number
            int currentNum = 0;
            for (int i = 0; i < 8; ++i)
            {
                switch (i)
                {
                    case (0):
                        currentNum = 0;//opponentHealthQuartile;
                        break;
                    case (1):
                        currentNum = 0;//agentHealthQuartile;
                        break;
                    case (2):
                        currentNum = opponentTypeIndex1;
                        break;
                    case (3):
                        currentNum = opponentTypeIndex2;
                        break;
                    case (4):
                        currentNum = move1TypeIndex;
                        break;
                    case (5):
                        currentNum = move2TypeIndex;
                        break;
                    case (6):
                        currentNum = move3TypeIndex;
                        break;
                    case (7):
                        currentNum = move4TypeIndex;
                        break;
                    default: break;
                }

                //Add to the stateNum, multiplied by a certain order of magnitude
                //Console.Write("state aspect " + i + ": " + currentNum + "; ");
                stateNum += (int) (currentNum * Math.Pow(digitMult, (double) i));
            }
            Console.WriteLine();
            return stateNum;
        }

        public List<Move> ViableMoves(Battle b)
        {
            return b.GetAgentMoves();
        }

        public void EstimateRewards()
        {
            //Initialize values of states
            for (uint i = 0; i < STATE_SPACE; ++i)
            {
                stateValue[i] = STATE_INITIAL_REWARD;

                //Also the state-action pair values
                for (int j = 0; j < ACTION_SPACE; ++j)
                {
                    stateActionValue[i, j] = STATE_INITIAL_REWARD;
                }
            }

            return;
        }

        public int ChooseMove(Battle b)
        {
            //Use Q-learning to select the best move
            //We assume this pokemon is the AGENT in the Battle

            //Get the state, and map it to an integer
            int currentStateTemp = StateMapping(b);

            //Next we use an algorithm to decide the move to take
            int move = 0;

            //Get the move from the comparison accord.net SARSA algorithm
            //move = comparisonAlgorithm.GetAction(currentState);
            
            /*
            //Get the move from the random-choice comparison algorithm
            //We assume this pokemon is the DEFENDER in the Battle
            //Get the number of moves to choose from
            int numMoves = ViableMoves(b).Count;
            //Return a random int between 0 and that number, this will be the index of a random move
            Random r = new Random();
            //int move = Program.rnd.Next(numMoves);
            move = r.Next(numMoves);
            */

            //Get the move from our defined optimal-choice algorithm
            /*
            if (b.defender.Species.Name == "Charizard" || b.defender.Species.Name == "Venusaur")
            {
                move = 0;       //ice beam
            }
            else    //blastoise
            {
                move = 1;       //thunderbolt
            }
            */

            //Get the move from the RL algorithm
            //TD bellman:  V^pi(s) = SUM[a](pi(s, a)) * SUM[s'](P.ss' ^a * [R.ss' ^a + GAMMA * V^pi(s')])
            move = ChooseMoveEGreedy(currentState);
            
            //lastState = currentState;
            //currentState = currentStateTemp;

            //Print out the move
            Console.WriteLine("" + b.agent.Species.Name + " used " + ViableMoves(b)[move].Name + "!");

            //Store the move taken
            lastAction = move;

            //Return the move to apply its effects
            return move;
        }

        public int ChooseMoveEGreedy(int state)
        {
            //Chance EPSILON to move randomly, else move optimally
            
            Console.WriteLine("eps = " + variableEpsilon);
            //Generate random number...
            Random r = new Random();
            if (r.NextDouble() < variableEpsilon)
            {
                //Choose random move
                return ChooseMoveRandom();
            }
            else
            {
                //Choose optimal move
                int bestMove = 0;
                //Console.Write("Choosing from: ");
                for (int i = 0; i < ACTION_SPACE; ++i)
                {
                    //Console.Write("" + stateActionValue[state, i] + ", ");
                    if (stateActionValue[state, i] > stateActionValue[state, bestMove])
                    {
                        bestMove = i;
                    }
                }
                //Console.WriteLine("for state #" + state + " - chose move #" + bestMove + ".");
                return bestMove;
            }
        }

        public int ChooseMoveRandom()
        {
            //Get the move from the random-choice comparison algorithm
            //Return a random int between 0 and the total possible number of moves - 1
            Random r = new Random();
            //int move = Program.rnd.Next(numMoves);
            int move = r.Next(ACTION_SPACE);
            //Console.WriteLine("Choosing RANDOM move " + move + ".");
            return move;
        }

        public void LearnerUpdate(Battle b)
        {
            //Update the learner function
            //TD update:   V(s.t) <- V(s.t) + ALPHA*[r.(t+1) + GAMMA*V(s.(t+1)) - V(s.t)]
            stateValue[lastState] = stateValue[lastState] + (int) Math.Round(
                ALPHA * (lastReward + GAMMA * stateValue[currentState] - stateValue[lastState]));
            double newStateActionValue = 0.0;
            for (int i = 0; i < ACTION_SPACE; ++i)
            {
                newStateActionValue += stateActionValue[lastState, lastAction] +
                    ALPHA * (lastReward + GAMMA * stateActionValue[currentState, i] - stateActionValue[lastState, lastAction]);
            }

            stateActionValue[lastState, lastAction] = (int) Math.Round(newStateActionValue/4.0);

            //Update the current state
            //Get the state, and map it to an integer
            lastState = currentState;
            currentState = StateMapping(b);

            //Update the comparison SARSA algorithm
            /*
            comparisonAlgorithm.UpdateState(lastState, lastAction,
                lastReward);//, agentAi.currentState);
                */
        }

        public void ResetState(Battle b)
        {
            currentState = StateMapping(b);
            lastState = currentState;
        }


        //RL helper functions

        public void ApplyRewardWin()
        {
            ApplyReward(REWARD_WIN);
        }
        public void ApplyRewardLose()
        {
            ApplyReward(REWARD_LOSE);
        }
        public void ApplyRewardDealDamage(int damageDone)
        {
            ApplyReward(REWARD_DEAL_DAMAGE * damageDone);
        }
        public void ApplyRewardTakeDamage(int damageTaken)
        {
            ApplyReward(REWARD_TAKE_DAMAGE * damageTaken);
        }
        private void ApplyReward(int r)
        {
            //Give the agent a reward for the current episode (the latest element in the list)
            myBattleRewards[myBattleRewards.Count - 1] += r;
            myTurnRewards[myTurnRewards.Count - 1] += r;

            //Update the reward tracker
            lastReward = myTurnRewards[myTurnRewards.Count - 1];
        }

        public void StartNewBattleEpisode()
        {
            //Start a new battle episode
            myBattleRewards.Add(0);
        }
        public void StartNewTurnEpisode()
        {
            //Start a new turn episode
            //meanwhile, lastReward saves the reward from the last episode (the last turn)
            if (myTurnRewards.Count > 0) { lastReward = myTurnRewards[myTurnRewards.Count - 1]; }
            myTurnRewards.Add(0);
        }
    }
}

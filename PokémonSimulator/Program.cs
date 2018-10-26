using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PokémonAPI;
using PokémonSimulator;
using Accord.MachineLearning;

namespace PokémonSimulator
{
    class Program
    {
        public static Random rnd = new Random();

        static void Main(string[] args)
        {
            /*
            //TESTING PORTION
            //A pre-made Porygon
            Pokémon pokemon1 = RentalPokémon.RentalPorygon;
            //A pre-made Venusaur
            Pokémon pokemon2 = RentalPokémon.RentalVenusaur;
            Console.WriteLine("Porygon health: " + pokemon1.RemainingHealth);
            Console.WriteLine("Venusaur health: " + pokemon2.RemainingHealth);

            //An example of Porygon attacking Venusaur
            int damageDealt = pokemon1.Use(2, pokemon2);
            Console.WriteLine("Damage dealt by Porygon: " + damageDealt);

            //Checking remaining health
            int p1Remaining = pokemon1.RemainingHealth;
            int p2Remaining = pokemon2.RemainingHealth;
            Console.WriteLine("Porygon health: " + p1Remaining);
            Console.WriteLine("Venusaur health: " + p2Remaining);

            //See if Pokémon has fainted
            bool hasFainted = pokemon2.IsFainted;
            Console.WriteLine("Did Venusaur faint? " + hasFainted);
            */

            //Do the experiment a certain number of times
            int numIterations = 50;

            //Perform a number of pokemon battles each iteration
            int numBattles = 1000;

            //Keep track of running sums for averaging later
            List<int> runningSumOfRewards = new List<int>();
            List<double> averageOfRewards = new List<double>();
            for (int j = 0; j < numBattles; ++j)
            {
                runningSumOfRewards.Add(0);
                averageOfRewards.Add(0);
            }

            for (int j = 1; j <= numIterations; ++j)
            {
                //Track stats
                int agentWins = 0;
                int opponentWins = 0;

                //Create the AI for our battles
                IntelligentPokéAgent agentAi = new IntelligentPokéAgent();

                //Initialize learning algorithm
                agentAi.EstimateRewards();

                //These are for the comparison algorithm, a SARSA algorithm from accord.net machine learning library
                /*
                var epol = new EpsilonGreedyExploration(IntelligentPokéAgent.EPSILON);
                var tpol = new TabuSearchExploration(IntelligentPokéAgent.ACTION_SPACE, epol);
                var sarsa = new Sarsa(IntelligentPokéAgent.STATE_SPACE, IntelligentPokéAgent.ACTION_SPACE, tpol);
                agentAi.comparisonAlgorithm = sarsa;
                */

                //agentAi.qlearn.LearningRate = IntelligentPokéAgent.ALPHA;
                //agentAi.qlearn.DiscountFactor = IntelligentPokéAgent.GAMMA;
                RandomPokéAgent opponentAi = new RandomPokéAgent();

                //Borrow some already-created pokemon for the battle
                Pokémon agent = RentalPokémon.RentalPorygon;
                List<Pokémon> opponents = new List<Pokémon>();
                Pokémon opponent1 = RentalPokémon.RentalVenusaur;
                Pokémon opponent2 = RentalPokémon.RentalBlastoise;
                Pokémon opponent3 = RentalPokémon.RentalCharizard;
                opponents.Add(opponent1);
                opponents.Add(opponent2);
                opponents.Add(opponent3);
                Pokémon opponent;

                Battle testBattle = new Battle();

                //Battle
                for (int i = 0; i < numBattles; ++i)
                {
                    //Start a new episode
                    agentAi.StartNewBattleEpisode();

                    //Decrease exploration rate gradually
                    //agentAi.variableEpsilon = IntelligentPokéAgent.EPSILON - (i / (double)numBattles) * IntelligentPokéAgent.EPSILON;
                    agentAi.variableEpsilon *= IntelligentPokéAgent.EPSILON_DECAY;
                    /*
                    //TESTING - reset stuff for this battle
                    epol.Epsilon = IntelligentPokéAgent.EPSILON - (i / (double)numBattles) * IntelligentPokéAgent.EPSILON;
                    //agentAi.qlearn.LearningRate = IntelligentPokéAgent.ALPHA - (i / (double)numBattles) * IntelligentPokéAgent.ALPHA;
                    tpol.ResetTabuList();
                    */

                    //Get a random opponent
                    opponent = opponents[rnd.Next(opponents.Count)];


                    //~~~~~~~~~~~~~~~~~~~~~TESTING: what if pokemon had much more health?~~~~~~~~~~~~~~~~~~~~~~~~~~~
                    //agent.Stats[Stat.HP] = 1000;
                    //opponent.Stats[Stat.HP] = 1000;
                    agent.Heal();
                    opponent.Heal();
                    

                    //Print battle text to console
                    Console.WriteLine("~~~~~ITERATION " + j + ", BATTLE " + (i+1) + "~~~~~");
                    Console.WriteLine("A wild " + opponent.Species.Name + " appears! Go, " + agent.Species.Name + "!");

                    //Do the battle and record the winner
                    Pokémon winner = testBattle.DoBattle(agent, agentAi, opponent, opponentAi);

                    //Print winner to console
                    Console.WriteLine("The winner is " + winner.Species.Name + ", with " + winner.RemainingHealth + " HP left!\n");

                    //Increment stats
                    if (winner == agent)
                    {
                        agentWins++;
                    }
                    else
                    {
                        opponentWins++;
                    }

                    //Refresh pokemon health
                    agent.Heal();
                    opponent.Heal();
                }

                //Print out stats
                Console.WriteLine("Out of " + numBattles + " battles:");
                Console.WriteLine("\tThe agent won " + agentWins + " times.");
                Console.WriteLine("\tThe opponent won " + opponentWins + " times.");
                /*
                Console.WriteLine("Agent total reward per battle: ");
                for (int i = 0; i < numBattles; ++i)
                {
                    //Write rewards in a cluster
                    Console.Write("" + agentAi.myRewards[i]);
                    if (i == numBattles - 1) { Console.WriteLine(); }
                    else { Console.Write(", "); }
                
                    //Write rewards by line
                    //Console.WriteLine("\tBattle #" + (i+1) + ": " + agentAi.myRewards[i]);
                }
                */

                //Write rewards to file for graphing
                //String outputFile = "rewarddata" + j + ".txt";
                //using (System.IO.StreamWriter outputWriter = new System.IO.StreamWriter(outputFile))
                //{
                    for (int i = 0; i < numBattles; ++i)
                    {
                        //outputWriter.WriteLine("" + i + ", " + agentAi.myBattleRewards[i]);

                        //Also add to the running sums
                        runningSumOfRewards[i] += agentAi.myBattleRewards[i];
                    }
                //}
                /*
                outputFile = "rewarddata" + j + ".csv";
                using (System.IO.StreamWriter outputWriter = new System.IO.StreamWriter(outputFile))
                {
                    for (int i = 0; i < numBattles; ++i)
                    {
                        outputWriter.WriteLine("" + i + ", " + agentAi.myBattleRewards[i]);
                    }
                }
                */
            }

            //Finally, go through and average all the rewards from all the iterations
            for (int j = 0; j < numBattles; ++j)
            {
                averageOfRewards[j] = (int) Math.Round(runningSumOfRewards[j] / (double)numIterations);
            }

            //Finally, write the averages to an outfile
            String outputFileAvg = "rewarddata_averages.txt";
            using (System.IO.StreamWriter outputWriter = new System.IO.StreamWriter(outputFileAvg))
            {
                for (int i = 0; i < numBattles; ++i)
                {
                    outputWriter.WriteLine("" + i + ", " + averageOfRewards[i]);
                }
            }

            //Done
            return;
        }
    }
}

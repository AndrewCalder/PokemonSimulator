using System;
using System.Collections.Generic;
using System.Text;
using PokémonAPI;

namespace PokémonSimulator
{
    public class RandomPokéAgent : IPokéAgent
    {
        public int StateMapping(Battle b)
        {
            //don't need to worry about this function for a random-choice AI
            return 0;
        }

        public List<Move> ViableMoves(Battle b)
        {
            return b.GetDefenderMoves();
        }

        public void EstimateRewards()
        {
            //don't need to worry about this function for a random-choice AI
            return;
        }

        public int ChooseMove(Battle b)
        {
            //Choose a move at random
            //We assume this pokemon is the DEFENDER in the Battle

            //Get the number of moves to choose from
            int numMoves = ViableMoves(b).Count;

            //Return a random int between 0 and that number, this will be the index of a random move
            Random r = new Random();
            //int move = Program.rnd.Next(numMoves);
            int move = r.Next(numMoves);

            //Print out the move
            Console.WriteLine("" + b.defender.Species.Name + " used " + ViableMoves(b)[move].Name + "!");

            return move;
        }
    }
}

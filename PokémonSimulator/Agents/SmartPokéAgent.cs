using System;
using System.Collections.Generic;
using System.Text;
using PokémonAPI;

namespace PokémonSimulator
{
    public class SmartPokéAgent : IPokéAgent
    {
        public int StateMapping(Battle b)
        {
            //don't need to worry about this function for the AI
            return 0;
        }

        public List<Move> ViableMoves(Battle b)
        {
            return b.GetDefenderMoves();
        }

        public void EstimateRewards()
        {
            //don't need to worry about this function for the AI
            return;
        }

        public int ChooseMove(Battle b)
        {
            //Choose the best move
            //We assume this pokemon is the DEFENDER in the Battle (not the agent)
            int move = 0;

            //Take a look at the opponent's types
            if (b.GetAgentTypes()[0] == PokémonAPI.Type.Fire)
            {
                //vs. Charizard - Electric or Water is best, then Normal
                move = GetMoveWithType(ViableMoves(b), PokémonAPI.Type.Electric);
                if (move == -1)
                {
                    move = GetMoveWithType(ViableMoves(b), PokémonAPI.Type.Water);
                    if (move == -1)
                    {
                        //Can't find our top choices
                        move = GetMoveWithoutType(ViableMoves(b), PokémonAPI.Type.Normal);
                    }
                }
            }
            else if (b.GetAgentTypes()[0] == PokémonAPI.Type.Ghost)
            {
                //vs Gengar - Ground or Psychic is best, then anything but Normal
                move = GetMoveWithType(ViableMoves(b), PokémonAPI.Type.Ground);
                if (move == -1)
                {
                    move = GetMoveWithType(ViableMoves(b), PokémonAPI.Type.Psychic);
                    if (move == -1)
                    {
                        //Can't find our top choices
                        move = GetMoveWithoutType(ViableMoves(b), PokémonAPI.Type.Normal);
                    }
                }
            }
            else if (b.GetAgentTypes()[0] == PokémonAPI.Type.Grass)
            {
                //vs Venusaur - Fire or Ice is best, then Normal
                move = GetMoveWithType(ViableMoves(b), PokémonAPI.Type.Fire);
                if (move == -1)
                {
                    move = GetMoveWithType(ViableMoves(b), PokémonAPI.Type.Ice);
                    if (move == -1)
                    {
                        //Can't find our top choices
                        move = GetMoveWithType(ViableMoves(b), PokémonAPI.Type.Normal);
                    }
                }
            }
            else if (b.GetAgentTypes()[0] == PokémonAPI.Type.Normal)
            {
                //vs Porygon - any move is fine
                Random r = new Random();
                move = r.Next(ViableMoves(b).Count);
            }
            else if (b.GetAgentTypes()[0] == PokémonAPI.Type.Water)
            {
                //vs Blastoise - Electric or Grass is best, then Normal
                move = GetMoveWithType(ViableMoves(b), PokémonAPI.Type.Electric);
                if (move == -1)
                {
                    move = GetMoveWithType(ViableMoves(b), PokémonAPI.Type.Grass);
                    if (move == -1)
                    {
                        //Can't find our top choices
                        move = GetMoveWithType(ViableMoves(b), PokémonAPI.Type.Normal);
                    }
                }
            }

            if (move == -1)
            {
                //Something's gone wrong
                Random r = new Random();
                move = r.Next(2) + 2;
                Console.WriteLine("ERROR ERROR ERROR, TRIED TO SELECT MOVE -1, WHAT, NONE OF THEM ARE GOOD ENOUGH FOR YOU???");
            }

            //Print out the move
            Console.WriteLine("" + b.defender.Species.Name + " used " + ViableMoves(b)[move].Name + "!");

            return move;
        }

        public int GetMoveWithType(List<PokémonAPI.Move> moves, PokémonAPI.Type type)
        {
            //Return the first move encountered that has a given type
            //Iterate through the moves (4 moves assumed)
            for (int i = 0; i < 4; ++i)
            {
                //Console.WriteLine("Comparing type " + moves[i].AttackType + " with " + type + "...");
                if (moves[i].AttackType == type)
                {
                    //Type found
                    //Console.WriteLine("Match!");
                    return i;
                }
            }

            //Type not found
            //Console.WriteLine("No match");
            return -1;
        }

        public int GetMoveWithoutType(List<PokémonAPI.Move> moves, PokémonAPI.Type type)
        {
            //Return the first move encountered that DOES NOT have a given type
            //Iterate through the moves (4 moves assumed)
            for (int i = 0; i < 4; ++i)
            {
                if (moves[i].AttackType != type)
                {
                    //Type found
                    return i;
                }
            }

            //Type not found
            return -1;
        }
    }
}

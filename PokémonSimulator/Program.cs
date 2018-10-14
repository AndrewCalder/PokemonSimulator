using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PokémonAPI;
using PokémonSimulator;

namespace PokémonSimulator
{
    class Program
    {
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

            //Testing to see if Pokémon has fainted
            bool hasFainted = pokemon2.IsFainted;
            Console.WriteLine("Did Venusaur faint? " + hasFainted);
            */




            //TEST: Perform one pokemon battle

            //Borrow 2 already-created pokemon for the battle
            //A porygon (ours) and a venusaur (opponent)
            Pokémon porygon = RentalPokémon.RentalPorygon;
            Pokémon venusaur = RentalPokémon.RentalVenusaur;

            //Create the AI for our battle
            IntelligentPokéAgent porygonAi = new IntelligentPokéAgent();
            RandomPokéAgent venusaurAi = new RandomPokéAgent();

            //Print battle text to console
            Console.WriteLine("A wild " + venusaur.Species.Name + " appears! Go, " + porygon.Species.Name + "!");

            //Do the battle and record the winner
            Battle testBattle = new Battle();
            Pokémon winner = testBattle.DoBattle(porygon, porygonAi, venusaur, venusaurAi);
            
            //Print winner to console
            Console.WriteLine("The winner is " + winner.Species.Name + ".");

            //Testing
            //Console.WriteLine("Porygon moves: " + porygonAi.ViableMoves(testBattle)[0].Name);
        }
    }
}

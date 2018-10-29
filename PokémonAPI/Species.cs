using System.Collections.Generic;
using System.Linq;

namespace PokémonAPI
{
    /// <summary>
    /// Enumerated class that contains valid Pokémon species
    /// </summary>
    public class Species
    {
        /// <summary>
        /// Gets all of the valid Pokémon species.
        /// </summary>
        /// <value>
        /// All the valid Pokémon species.
        /// </value>
        public List<Species> AllSpecies { get; private set; } = new List<Species>();

        //public static Species Bulbasaur { get; } = new Species("Bulbasaur", Type.Grass, Type.Poison, StatsGen(262, 165, 163, 197, 157));
        //public static Species Ivysaur { get; } = new Species("Ivysaur", Type.Grass, Type.Poison, StatsGen(262, 165, 163, 197, 157));
        public static Species Venusaur { get; } = new Species("Venusaur", HelperMethods.Listify(Type.Grass, Type.Poison), StatsGen(262, 165, 163, 197, 157));
        //public static Species Charmander { get; } = new Species("Charmander", Type.Fire, StatsGen(250, 171, 151, 167, 197));
        //public static Species Charmeleon { get; } = new Species("Charmeleon", Type.Fire, StatsGen(280, 185, 175, 189, 219));
        public static Species Charizard { get; } = new Species("Charizard", HelperMethods.Listify(Type.Fire, Type.Flying), StatsGen(306, 213, 201, 215, 245));
        //public static Species Squirtle { get; } = new Species("Squirtle", Type.Water, StatsGen(260, 163, 195, 167, 153));
        //public static Species Wartortle { get; } = new Species("Wartortle", Type.Water, StatsGen(282, 183, 219, 189, 175));
        public static Species Blastoise { get; } = new Species("Blastoise", HelperMethods.Listify(Type.Water), StatsGen(308, 211, 245, 215, 201));
        public static Species Gengar { get; } = new Species("Gengar", HelperMethods.Listify(Type.Ghost, Type.Poison),
            StatsGen(270, 175, 165, 305, 265));
        public static Species Porygon { get; } = new Species("Porygon", HelperMethods.Listify(Type.Normal), StatsGen(294, 177, 199, 209, 139) );

        /// <summary>
        /// Initializes a new Pokémon <see cref="Species"/> and adds it to the <see cref="AllSpecies"/> list.
        /// </summary>
        /// <param name="name">The name of the species.</param>
        /// <param name="types">The types that members of this species are.</param>
        /// <param name="baseStats">The base stats of the species.</param>
        private Species(string name, IEnumerable<Type> types, Dictionary<Stat, int> baseStats)
        {
            Name = name;
            Types = types.ToList();
            BaseStats = baseStats;

            AllSpecies.Add(this);
        }

        /// <summary>
        /// Gets the name of this Pokémon species.
        /// </summary>
        /// <value>
        /// The name of this Pokémon species.
        /// </value>
        public string Name { get; }

        /// <summary>
        /// Gets the types that members of this Pokémon species are (Water, Ice, Fire, etc.)
        /// </summary>
        /// <value>
        /// The types that members of this Pokémon species are (Water, Ice, Fire, etc.)
        /// </value>
        public List<Type> Types { get; }

        /// <summary>
        /// Gets the base stats of this Pokémon species.
        /// </summary>
        /// <value>
        /// The base stats of this Pokémon species.
        /// </value>
        public Dictionary<Stat, int> BaseStats { get; }


        /// <summary>
        /// Generates a dictionary associating <see cref="Stat"/>s with integer values.
        /// </summary>
        /// <param name="hp">The HP stat.</param>
        /// <param name="attack">The Attack stat.</param>
        /// <param name="defense">The Defense stat.</param>
        /// <param name="special">The Special stat.</param>
        /// <param name="speed">The Speed stat.</param>
        /// <returns>A dictionary associating <see cref="Stat"/>s with integer values.</returns>
        private static Dictionary<Stat, int> StatsGen(int hp, int attack, int defense, int special, int speed) =>
            new Dictionary<Stat, int>
            {
                { Stat.HP, hp },
                { Stat.Attack, attack },
                { Stat.Defense, defense },
                { Stat.Special, special },
                { Stat.Speed, speed }
            };
    }
}

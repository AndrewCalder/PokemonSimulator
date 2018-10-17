using System;
using System.Collections.Generic;
using System.Linq;

namespace PokémonAPI
{
    /// <summary>
    /// Data structure that represents individual Pokémon.
    /// </summary>
    public class Pokémon
    {
        private int _damage = 0;

        /// <summary>
        /// Constructs a Pokémon of a given species with the given moves
        /// </summary>
        /// <param name="species">The maximum number of hit points this Pokémon can have.</param>
        /// <param name="moves">The moves this Pokémon has</param>
        public Pokémon(Species species, IEnumerable<Move> moves)
        {
            Species = species;
            Moves = moves.ToList();
        }

        /// <summary>
        /// Retrieves this Pokémon's level
        /// </summary>
        /// <value>
        /// This Pokémon's level.
        /// </value>
        public int Level { get; } = 100;

        /// <summary>
        /// Gets the species this Pokémon is a member of.
        /// </summary>
        /// <value>
        /// The species this Pokémon is a member of.
        /// </value>
        public Species Species { get; }

        /// <summary>
        /// Gets or sets the types this Pokémon has.
        /// </summary>
        /// <value>
        /// The types this Pokémon has.
        /// </value>
        public List<Type> Types => Species.Types;

        /// <summary>
        /// The list of <see cref="Move" />s this Pokémon has, presumably 4 or fewer.
        /// </summary>
        /// <value>
        /// The assortment of moves this Pokémon has
        /// </value>
        public List<Move> Moves { get; set; }

        /// <summary>
        /// Gets or sets how much damage this Pokémon has received. Caps at the max HP value.
        /// </summary>
        /// <value>
        /// The damage this Pokémon has received. Caps at the max HP value.
        /// </value>
        public int Damage
        {
            get => _damage;
            set => _damage = Math.Min(Stats[Stat.HP], _damage + value);
        }

        /// <summary>
        /// Heals the Pokémon fully.
        /// </summary>
        public void Heal()
        {
            _damage = 0;
        }

        /// <summary>
        /// Gets this Pokémon's full stats (i.e., what would show up on an in-game stats screen).
        /// </summary>
        /// <value>
        /// This Pokémon's stats.
        /// </value>
        public Dictionary<Stat, int> Stats => Species.BaseStats;

        /// <summary>
        /// Gets the remaining health this Pokémon has.
        /// </summary>
        /// <value>
        /// The remaining health this Pokémon has.
        /// </value>
        public int RemainingHealth => Stats[Stat.HP] - Damage;

        /// <summary>
        /// Retrieves whether this Pokémon has fainted;
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is fainted; otherwise, <c>false</c>.
        /// </value>
        public bool IsFainted => RemainingHealth == 0;
    }

    /// <summary>
    /// Additional methods for the Pokémon
    /// </summary>
    public static class PokémonExtensions
    {
        /// <summary>
        /// Uses the specified move, as invoked by the <paramref name="user"/> on the <paramref name="defender"/>
        /// </summary>
        /// <param name="user">The user of the <paramref name="move"/>.</param>
        /// <param name="move">The move being used.</param>
        /// <param name="defender">The defender whom the <paramref name="move"/> is being used upon.</param>
        /// <returns>The amount of damage done by the move.</returns>
        public static int Use(this Pokémon user, Move move, Pokémon defender)
        {
            int initialHP = defender.RemainingHealth;
            move.Use(user, defender);
            int laterHP = defender.RemainingHealth;
            return initialHP - laterHP;
        }

        /// <summary>
        /// Uses the specified move, as invoked by the <paramref name="user"/> on the <paramref name="defender"/>
        /// </summary>
        /// <param name="user">The user of the move.</param>
        /// <param name="moveIndex">The index corresponding to which of the <paramref name="user"/>'s moves to use.</param>
        /// <param name="defender">The defender whom the move is being used upon.</param>
        /// <returns>The amount of damage done by the move.</returns>
        public static int Use(this Pokémon user, int moveIndex, Pokémon defender) =>
            Use(user, user.Moves[moveIndex], defender);
    }
}
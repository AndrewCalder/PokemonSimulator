using System;
using System.Collections.Generic;

namespace PokémonAPI
{
    /// <summary>
    /// Enumeration of valid Pokémon moves.
    /// </summary>
    public partial class Move
    {
        private Move(string name, Type type, MoveCategory category, int attackPower,
            Action<Pokémon, Pokémon> additionalEffects)
        {
            Name = name;
            AttackType = type;
            Category = category;
            AttackPower = attackPower;
            AllMoves.Add(this);
        }

        /// <summary>
        /// A relatively powerful, damage-dealing ice-type move. Would normally have a chance of freezing the opponent, but for simplification's sake it does not at the moment.
        /// </summary>
        public static Move IceBeam { get; } = new Move("Ice Beam", Type.Ice, MoveCategory.Special, 95, null);

        /// <summary>
        /// A relatively powerful, damage-dealing electric-type move. Would normally have a chance of paralyzing the opponent, but for simplification's sake it does not at the moment.
        /// </summary>
        public static Move Thunderbolt { get; } =
            new Move("Thunderbolt", Type.Electric, MoveCategory.Special, 95, null);

        /// <summary>
        /// A relatively powerful, damage-dealing fire-type move. Would normally have a chance of burning the opponent, but for simplification's sake it does not at the moment.
        /// </summary>
        public static Move Flamethrower { get; } =
            new Move("Flamethrower", Type.Fire, MoveCategory.Special, 95, null);

        /// <summary>
        /// A relatively powerful, damage-dealing psychic-type move.
        /// </summary>
        public static Move Psychic { get; } =
            new Move("Psychic", Type.Psychic, MoveCategory.Special, 90, null);

        public static Move Tackle { get; } = new Move("Tackle", Type.Normal, MoveCategory.Physical, 35, null);

        public static Move Swift { get; } = new Move("Swift", Type.Normal, MoveCategory.Physical, 60, null);

        public static Move Cut { get; } = new Move("Cut", Type.Normal, MoveCategory.Physical, 50, null);

        public static Move Strength { get; } = new Move("Strength", Type.Normal, MoveCategory.Physical, 80, null);

        /// <summary>
        /// A relatively powerful, damage-dealing normal-type move. Would normally have a chance of paralyzing the opponent, but for simplification's sake it does not do so at the moment.
        /// </summary>
        public static Move BodySlam { get; } = new Move("Body Slam", Type.Normal, MoveCategory.Physical, 85, null);

        /// <summary>
        /// A powerful, damage-dealing ground-type move.
        /// </summary>
        public static Move Earthquake { get; } = new Move("Earthquake", Type.Ground, MoveCategory.Physical, 100, null);

        public static Move SolarBeam { get; } = new Move("SolarBeam", Type.Grass, MoveCategory.Special, 120, null);

        public static Move HydroPump { get; } = new Move("Hydro Pump", Type.Water, MoveCategory.Special, 120, null);

        /// <summary>
        /// Changes the user's types to match the opponent's
        /// </summary>
        public static Move Conversion { get; } = new Move("Conversion", Type.Normal, MoveCategory.Status, 0,
            (Pokémon attacker, Pokémon defender) => { attacker.Types.Clear(); attacker.Types.AddRange(defender.Types); }
            );

        public List<Move> AllMoves { get; } = new List<Move>();
    }
}

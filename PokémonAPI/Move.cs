using System;
using System.Linq;

namespace PokémonAPI
{
    /// <summary>
    /// The skills used by the Pokémon in battle (https://bulbapedia.bulbagarden.net/wiki/Move).
    /// </summary>
    public partial class Move
    {
        private Random r = new Random();


        /// <summary>
        /// Gets the name of the move.
        /// </summary>
        /// <value>
        /// The name of the move.
        /// </value>
        public string Name { get; }

        /// <summary>
        /// Gets the type of the move.
        /// </summary>
        /// <value>
        /// The type of the move.
        /// </value>
        public Type AttackType { get; }

        /// <summary>
        /// Gets the category of the move (Physical, Special, Status)
        /// </summary>
        /// <value>
        /// The category of the move (Physical, Special, Status)
        /// </value>
        public MoveCategory Category { get; }

        /// <summary>
        /// Gets the power of the move.
        /// </summary>
        /// <value>
        /// The power of the move
        /// </value>
        public int AttackPower { get; }

        /// <summary>
        /// The primary effect of the move.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="defender">The defender.</param>
        public void Use(Pokémon user, Pokémon defender)
        {
            if (Category!=MoveCategory.Status)
                defender.Damage += CalculateDamage(user, defender);
            AdditionalEffects?.Invoke(user, defender);
        }

        /// <summary>
        /// Gets the additional effects the move may have.
        /// </summary>
        /// <value>
        /// The additional effects the move may have (i.e. pretty much anything that isn't straightforward pure damage move effects)
        /// </value>
        public Action<Pokémon, Pokémon> AdditionalEffects { get; } = null;

        /// <summary>
        /// Calculates the raw damage, assuming this move has no additional effects.
        /// </summary>
        /// <param name="user">The Pokémon using this move.</param>
        /// <param name="defender">The Pokémon defending against this move.</param>
        /// <returns></returns>
        public int CalculateDamage (Pokémon user, Pokémon defender)
        {
            //Attacker's Level
            int A = user.Level;
            //Attacker's Attack or Special stat
            int B;
            //Attack's power
            int C = AttackPower;
            //Defender's Defense or Special stat
            int D;
            //STAB modifier (1.5 if this attack's type is one of the user's types; 1, otherwise).
            double X = user.Types.Contains(AttackType)?1.5:1;

            var temp = AttackType.AttackMultipliers();
            var temp2 = temp[Type.Grass];
            //Type effectiveness modifier. Product of the type effectiveness modifiers of this move's type against each of the defender's types.
            double Y = defender.Types.Aggregate
            (
                seed: 1.0,
                //seed: r.NextDouble(),
                func: (result, item) => result * AttackType.AttackMultipliers()[item],
                resultSelector: result=>result
            );
            
            //TESTING
            //Manually put in the type effectiveness chart for our limited state space
            if (((defender.Types[0] == Type.Grass) && AttackType == Type.Ice) ||
                defender.Types[0] == Type.Water &&
                AttackType == Type.Electric)
            {
                Y = 2.0;
            } else if ((defender.Types[0] == Type.Water && AttackType == Type.Ice) ||
                (defender.Types[0] == Type.Grass && AttackType == Type.Electric))
            {
                Y = 0.5;
            }
            else
            {
                Y = 1.0;
            }
            Console.WriteLine("Type effectiveness of " + AttackType + " against " + defender.Species.Name + ": " + Y);

            //Random number between 217 and 255
            int Z = new Random().Next(217, 256);
            //int Z = r.Next(217, 256);

            switch (Category)
            {
                case MoveCategory.Physical:
                    B = user.Stats[Stat.Attack];
                    D = defender.Stats[Stat.Defense];
                    break;
                case MoveCategory.Special:
                    B = user.Stats[Stat.Special];
                    D = user.Stats[Stat.Special];
                    break;
                default:
                    B = 0;
                    D = int.MaxValue;
                    break;
            }

            double damage = 2 * A / 5.0 + 2;
            damage *= B;
            damage *= C;
            damage /= D;
            damage = damage / 50 + 2;
            damage *= X;
            damage *= Y;
            damage *= Z;
            damage /= 255.0;
            /*
            double damage2 = 2 * A / 5.0 + 2;
            damage2 *= B;
            damage2 *= C;
            damage2 /= D;
            damage2 = damage2 / 50 + 2;
            damage2 *= X;
            //damage2 *= Y;
            damage2 *= Z;
            damage2 /= 255.0;

            //Console.WriteLine("Damage with attack type multiplier: " + damage);
            //Console.WriteLine("Damage without attack type multiplier: " + damage2);
            */

            return (int) damage;
        }
    }

    /// <summary>
    /// The category of the move (https://bulbapedia.bulbagarden.net/wiki/Damage_category).
    /// This determines which stat is used for attack/defense (i.e., Attack/Special or Defense/Special),
    /// as well as whether the move does damage at all (i.e., Status moves do not).
    /// </summary>
    public enum MoveCategory
    {
        Physical,
        Special,
        Status
    }

}

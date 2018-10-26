using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace PokémonAPI
{
    /// <summary>
    /// Enumeration of Pokémon/Move types
    /// </summary>
    public enum Type
    {
        [Display(Name="Normal")]
        [AttackingMultiplier(Rock, 0.5, Ghost, 0)]
        Normal,

        [Display(Name = "Fighting")]
        [AttackingMultiplier(Normal, 2, Flying, 0.5, Poison, 0.5, Rock, 2, Bug, 0.5, Ghost, 0, Psychic, 0.5, Ice, 2)]
        Fighting,

        [Display(Name = "Flying")]
        [AttackingMultiplier(Fighting, 2, Rock, 0.5, Bug, 2, Grass, 2, Electric, 0.5)]
        Flying,

        [Display(Name = "Poison")]
        [AttackingMultiplier(Poison, 0.5, Ground, 0.5, Rock, 0.5, Bug, 2, Ghost, 0.5, Grass, 2)]
        Poison,

        [Display(Name = "Ground")]
        [AttackingMultiplier(Flying, 0, Poison, 2, Rock, 2, Bug, 0.5, Fire, 2, Grass, 0.5, Electric, 2)]
        Ground,

        [Display(Name = "Rock")]
        [AttackingMultiplier(Fighting, 0.5, Flying, 2, Ground, 0.5, Bug, 2, Fire, 2, Ice, 2)]
        Rock,

        [Display(Name = "Bug")]
        [AttackingMultiplier(Fighting, 0.5, Flying, 0.5, Poison, 2, Ghost, 0.5, Fire, 0.5, Grass, 2, Psychic, 2)]
        Bug,

        [Display(Name = "Ghost")]
        [AttackingMultiplier(Normal, 0, Ghost, 2, Psychic, 0)]
        Ghost,

        [Display(Name = "Fire")]
        [AttackingMultiplier(Rock, 0.5, Bug, 2, Fire, 0.5, Water, 0.5, Grass, 2, Ice, 2, Dragon, 0.5)]
        Fire,

        [Display(Name = "Water")]
        [AttackingMultiplier(Ground, 2, Rock, 2, Fire, 2, Water, 0.5, Grass, 0.5, Dragon, 0.5)]
        Water,

        [Display(Name = "Grass")]
        [AttackingMultiplier(Flying, 0.5, Poison, 0.5, Ground, 2, Rock, 2, Bug, 0.5, Fire, 0.5, Water, 2, Grass, 0.5, Dragon, 0.5)]
        Grass,

        [Display(Name = "Electric")]
        [AttackingMultiplier(Flying, 2, Ground, 0, Water, 2, Grass, 0.5, Electric, 0.5, Dragon, 0.5)]
        Electric,

        [Display(Name = "Psychic")]
        [AttackingMultiplier(Fighting, 2, Poison, 2, Psychic, 0.5)]
        Psychic,

        [Display(Name = "Ice")]
        [AttackingMultiplier(Fire, 0.5, Flying, 2, Ground, 2, Water, 0.5, Grass, 2, Ice, 0.5, Dragon, 2)]
        Ice,

        [Display(Name = "Dragon")]
        [AttackingMultiplier(Dragon, 2)]
        Dragon
    }

    /// <summary>
    /// Adding methods to the <see cref="Type"/> (equipped with the <see cref="AttackMultipliers"/> attribute) enumeration for retrieving type effectiveness
    /// </summary>
    internal static class TypeExtensions
    {
        private static Dictionary<Type, double> _attackMultipliers=null;
        private static Dictionary<Type, double> _defenseMultipliers=null;

        public static ImmutableDictionary<Type, double> AttackMultipliers(this Type attackingType)
        {
            if (_attackMultipliers == null)
            {
                var attackAttribute = attackingType.GetAttributeOfType<AttackingMultiplierAttribute>();
                _attackMultipliers = attackAttribute.AttackMultipliers();
            }

            return _attackMultipliers.ToImmutableDictionary();
        }

        public static ImmutableDictionary<Type, double> DefenseMultipliers(this Type defendingType)
        {
            if (_defenseMultipliers == null)
            {
                _defenseMultipliers = new Dictionary<Type, double>();

                //Populate the defense multipliers with nontrivial values
                foreach (Type attackingType in Enum.GetValues(typeof(Type)))
                {
                    var attackAttribute = attackingType.GetAttributeOfType<AttackingMultiplierAttribute>();
                    Dictionary<Type, double> attackMultipliers = attackAttribute.AttackMultipliers();
                    if (attackMultipliers[defendingType] != 1)
                    {
                        _defenseMultipliers.Add(attackingType, attackMultipliers[defendingType]);
                    }
                }

                //Populate the remaining defense multipliers with trivial values
                foreach (Type attackingType in Enum.GetValues(typeof(Type)))
                {
                    if (!_defenseMultipliers.ContainsKey(attackingType))
                    {
                        _defenseMultipliers.Add(attackingType, 1);
                    }
                }
            }

            return _attackMultipliers.ToImmutableDictionary();
        }
    }

    /// <inheritdoc />
    /// <summary>
    /// Attribute for equipping Pokémon move types with attack/defense modifiers
    /// </summary>
    /// <seealso cref="T:System.Attribute" />
    [AttributeUsage(AttributeTargets.Field)]
    internal class AttackingMultiplierAttribute : Attribute
    {

        private readonly Dictionary<Type, double> _attackMultipliers;

        public AttackingMultiplierAttribute(params object[] modifiers)
        {
            var attackModifiers = modifiers.Select((v, i) => new { Index = i, Value = v })
                .GroupBy(x => x.Index / 2, x => x.Value)
                .Select(g => Tuple.Create<Type, double>((Type)(g.First()), Convert.ToDouble(g.Skip(1).FirstOrDefault())));

            _attackMultipliers = attackModifiers.ToDictionary(t => t.Item1, t => t.Item2);

            foreach (Type type in Enum.GetValues(typeof(Type)))
            {
           
                if (!_attackMultipliers.ContainsKey(type))
                    _attackMultipliers.Add(type, 1);
            }
        }

        public Dictionary<Type, double> AttackMultipliers()
        {
            return _attackMultipliers;
        }
    }

}

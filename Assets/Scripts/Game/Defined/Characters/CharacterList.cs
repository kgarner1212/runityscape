﻿using Scripts.Game.Defined.Characters;
using Scripts.Game.Defined.Serialized.Brains;
using Scripts.Game.Defined.Serialized.Items;
using Scripts.Game.Defined.Serialized.Spells;
using Scripts.Game.Defined.Serialized.Statistics;
using Scripts.Model.Characters;
using Scripts.Model.Items;
using Scripts.Model.Spells;
using Scripts.Model.Stats;
using UnityEngine;

namespace Scripts.Game.Defined.Characters {

    public static class CharacterUtil {

        public static Character RemoveFlag(this Character c, Model.Characters.Flag flag) {
            c.RemoveFlag(flag);
            return c;
        }

        public struct ItemCount {
            public readonly Item Item;
            public readonly int Count;

            public ItemCount(Item item, int count) {
                this.Count = count;
                this.Item = item;
            }

            public ItemCount(Item item) {
                this.Item = item;
                this.Count = 1;
            }
        }

        public static Character StandardEnemy(Stats stats, Look look, Brain brain) {
            Character enemy = new Character(stats, look, brain);
            enemy.AddFlag(Model.Characters.Flag.DROPS_ITEMS);
            enemy.AddFlag(Model.Characters.Flag.GIVES_EXPERIENCE);
            return enemy;
        }

        public static Character AddStats(this Character c, params Stat[] stats) {
            foreach (Stat stat in stats) {
                c.Stats.AddStat(stat);
            }
            return c;
        }

        public static Character AddSpells(this Character c, params SpellBook[] books) {
            foreach (SpellBook sb in books) {
                c.Spells.AddSpellBook(sb);
            }
            return c;
        }

        public static Character AddFlags(this Character c, params Model.Characters.Flag[] flags) {
            foreach (Model.Characters.Flag flag in flags) {
                c.AddFlag(flag);
            }
            return c;
        }

        public static Character AddItems(this Character c, params ItemCount[] items) {
            foreach (ItemCount itemCount in items) {
                c.Inventory.ForceAdd(itemCount.Item, itemCount.Count);
            }
            return c;
        }

        public static Character AddItem(this Character c, Item item, int count, bool isAdded = true) {
            if (isAdded) {
                c.Inventory.ForceAdd(item, count);
            }
            return c;
        }

        public static Character AddItem(this Character c, Item item, bool isAdded = true) {
            return c.AddItem(item, 1, isAdded);
        }

        public static Character AddEquips(this Character c, params EquippableItem[] equips) {
            foreach (EquippableItem equip in equips) {
                Inventory dummy = new Inventory();
                dummy.ForceAdd(equip);
                c.Equipment.AddEquip(dummy, new Model.Buffs.BuffParams(c.Stats, c.Id), equip);
            }
            return c;
        }

        public static Character AddEquip(this Character c, EquippableItem equip, bool isAdded = true) {
            if (isAdded) {
                Inventory dummy = new Inventory();
                dummy.ForceAdd(equip);
                c.Equipment.AddEquip(dummy, new Model.Buffs.BuffParams(c.Stats, c.Id), equip);
            }
            return c;
        }
    }

    public static class CharacterList {

        public static Character Hero(string name) {
            return new Character(
                new Stats(0, 1, 1, 2, 5),
                new Look(
                    name,
                    "player",
                    "It's you!",
                    Breed.PROGRAMMER
                    ),
                new Player())
                .AddFlags(Model.Characters.Flag.PLAYER, Model.Characters.Flag.PERSISTS_AFTER_DEFEAT, Model.Characters.Flag.HERO)
                .AddStats(new Experience());
        }

        public static Character Partner(string name) {
            return new Character(
                new Stats(0, 1, 2, 1, 5),
                new Look(
                    name,
                    "visored-helm",
                    "Has a helmet icon despite not wearing one.",
                    Breed.HUMAN
                    ),
                new Player())
                .AddFlags(Model.Characters.Flag.PLAYER, Model.Characters.Flag.PERSISTS_AFTER_DEFEAT, Model.Characters.Flag.PARTNER)
                .AddStats(new Experience());
        }

        public static Character TestEnemy() {
            Character c = new Character(
                new Stats(5, 1, 1, 5, 1),
                    new Look(
                    "Test Enemy",
                    "fox-head",
                    "Hello???",
                    Breed.UNKNOWN,
                    Color.magenta
                    ),
                new Player())
                .AddItems(new CharacterUtil.ItemCount(new Apple(), 7), new CharacterUtil.ItemCount(new PoisonArmor()), new CharacterUtil.ItemCount(new Money(), 100))
                .AddFlags(Model.Characters.Flag.DROPS_ITEMS)
                .AddSpells(new InflictPoison(), new SetupCounter())
                .AddStats(new Skill());
            return c;
        }
    }
}

namespace Scripts.Game.Undefined.Characters {

    public class CreditsDummy : Character {

        public CreditsDummy(
            Breed breed,
            int level,
            string name,
            string spriteLoc,
            string tip)
            : base(new Stats(level, 1, 1, 1, 10),
                  new Look(name, spriteLoc, tip, breed),
                  new DebugAI()) {
        }
    }
}
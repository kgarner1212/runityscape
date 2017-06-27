﻿using Scripts.Game.Defined.Serialized.Spells;
using Scripts.Model.Characters;
using Scripts.Model.Interfaces;
using Scripts.Model.Items;
using Scripts.Model.Processes;
using Scripts.Model.Spells;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Model.Pages {
    public static class PageUtil {
        public static readonly Sprite SPELLBOOK = Util.GetSprite("spell-book");
        public static readonly Sprite INVENTORY = Util.GetSprite("swap-bag");
        public static readonly Sprite EQUIPMENT = Util.GetSprite("shoulder-armor");

        public static Grid GenerateSpellBooks(
            Page p,
            IButtonable previous,
            SpellParams owner,
            SpellBook excluded,
            IEnumerable<ISpellable> spellCollection,
            Action<IPlayable> addPlay
            ) {
            return GenerateSpellableGrid(
                p,
                previous,
                owner,
                excluded,
                spellCollection,
                addPlay,
                SPELLBOOK,
                "Spells",
                "Cast a spell.");
        }

        public static Grid GenerateItems(
            Page p,
            IButtonable previous,
            SpellParams owner,
            SpellBook excluded,
            IEnumerable<ISpellable> spellCollection,
            Action<IPlayable> addPlay
            ) {
            return GenerateSpellableGrid(
                p,
                previous,
                owner,
                excluded,
                spellCollection,
                addPlay,
                INVENTORY,
                string.Format("Items ({0}/{1})", owner.Inventory.TotalOccupiedSpace, owner.Inventory.Capacity),
                string.Format("Use an item.\n{0} out of {1} inventory space is occupied.", owner.Inventory.TotalOccupiedSpace, owner.Inventory.Capacity)
                );
        }

        public static Grid GenerateBackableGrid(IButtonable previous, Sprite icon, string name, string tooltip) {
            Grid grid = new Grid(name);
            grid.Icon = icon;
            grid.Tooltip = tooltip;
            List<IButtonable> buttons = new List<IButtonable>();
            buttons.Add(GenerateBack(previous));
            grid.List = buttons;
            return grid;
        }

        public static Grid GenerateSpellableGrid(
            Page p,
            IButtonable previous,
            SpellParams owner,
            SpellBook excluded,
            IEnumerable<ISpellable> spellCollection,
            Action<IPlayable> addPlay,
            Sprite sprite,
            string name,
            string description) {

            Grid grid = GenerateBackableGrid(previous, sprite, name, description);

            foreach (ISpellable myS in spellCollection) {
                ISpellable s = myS;
                if (!s.Equals(excluded)) {
                    grid.List.Add(GenerateSpellProcess(p, grid, owner, s, addPlay));
                }
            }

            return grid;
        }

        public static Process GenerateSpellProcess(Page p, IButtonable previous, SpellParams owner, ISpellable spellable, Action<IPlayable> handlePlayable) {
            SpellBook sb = spellable.GetSpellBook();
            return new Process(sb.GetDetailedName(owner), sb.Icon, sb.CreateDescription(owner),
                () => {
                    if (sb.IsMeetPreTargetRequirements(owner.Stats)) {
                        GenerateTargets(p, previous, owner, spellable, handlePlayable).Invoke();
                    }
                });
        }

        public static Grid GenerateTargets(Page p, IButtonable previous, SpellParams owner, ISpellable spellable, Action<IPlayable> handlePlayable) {
            SpellBook sb = spellable.GetSpellBook();
            ICollection<Character> targets = sb.TargetType.GetTargets(owner.Character, p);
            Grid grid = GenerateBackableGrid(previous, sb.Icon, sb.Name, sb.CreateDescription(owner));

            grid.Icon = sb.Icon;

            foreach (Character myTarget in targets) {
                SpellParams target = new SpellParams(myTarget);
                grid.List.Add(GenerateTargetProcess(previous, owner, target, sb, handlePlayable));

            }
            Item item = spellable as Item;
            if (item != null && item.HasFlag(Items.Flag.TRASHABLE)) {
                grid.List.Add(
                    new Process(
                        string.Format("Toss {0}", item.Name),
                        string.Format("Throw away {0}.", item.Name),
                        () => {
                            handlePlayable(
                                owner.Spells.CreateSpell(new TossItem(item, owner.Inventory), owner, owner)
                                );
                        }
                        ));
            }
            return grid;
        }

        public static Process GenerateTargetProcess(IButtonable previous, SpellParams owner, SpellParams target, SpellBook sb, Action<IPlayable> handlePlayable) {
            return new Process(CreateDetailedTargetName(owner, target, sb),
                                target.Look.Sprite,
                                sb.CreateTargetDescription(owner, target),
                                () => {
                                    if (sb.IsCastable(owner, target)) {
                                        handlePlayable(owner.Spells.CreateSpell(sb, owner, target));
                                        previous.Invoke();
                                    }
                                });
        }

        public static Grid GenerateUnequipGrid(IButtonable previous, SpellParams owner, Action<IPlayable> handlePlayable) {
            Grid grid = GenerateBackableGrid(previous, Util.GetSprite("shoulder-armor"), "Equipment", "Manage equipped items.");

            foreach (EquipType myET in EquipType.AllTypes) {
                EquipType et = myET;
                Process p = null;
                Equipment eq = owner.Equipment;
                if (owner.Equipment.Contains(et)) {
                    CastUnequipItem unequip = new CastUnequipItem(owner.Inventory, owner.Equipment, eq.PeekItem(et));
                    p = new Process(unequip.Name, unequip.CreateDescription(owner),
                        () => {
                            handlePlayable(owner.Spells.CreateSpell(unequip, owner, owner));
                            previous.Invoke();
                        }
                            );
                } else {
                    p = new Process(Util.ColorString(et.Name, Color.grey), et.Sprite, "No item is equipped in this slot. Items can be equipped via the inventory menu.");
                }
                grid.List.Add(p);
            }
            return grid;
        }

        public static string CreateDetailedTargetName(SpellParams owner, SpellParams target, SpellBook sb) {
            return Util.ColorString(target.Look.DisplayName, sb.IsCastable(owner, target));
        }

        public static Process GenerateBack(IButtonable previous) {
            return new Process(
                "Back",
                Util.GetSprite("plain-arrow"),
                string.Format("Go back to {0}.", previous.ButtonText),
                () => previous.Invoke()
                );
        }
    }
}
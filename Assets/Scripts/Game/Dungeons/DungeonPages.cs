﻿using Scripts.Game.Serialized;
using Scripts.Model.Characters;
using Scripts.Model.Interfaces;
using Scripts.Model.Pages;
using Scripts.Model.Processes;
using System.Collections.Generic;

namespace Scripts.Game.Dungeons {
    public class DungeonPages : PageGroup {
        private readonly Party party;
        private readonly Flags flags;
        private readonly Page previous;

        public DungeonPages(Page previous, Party party, Flags flags) : base(new Page("Quest")) {
            var buttons = new List<IButtonable>();
            this.party = party;
            this.flags = flags;
            this.previous = previous;   

            Root.Icon = Util.GetSprite("dungeon-gate");
            Root.AddCharacters(Side.LEFT, party);
            Root.Condition = PageUtil.GetVisitProcessCondition(flags, party);

            buttons.Add(PageUtil.GenerateBack(previous));

            Area currentArea = GetCurrentArea(flags.CurrentArea);

            for (int i = 0; i < currentArea.Dungeons.Length; i++) {
                if (IsStagePlayable(i, currentArea)) {
                    buttons.Add(GetDungeonEntryProcess(i, currentArea));
                } else {
                    buttons.Add(new Process("<color=grey>???</color>", "Complete the previous stage to unlock."));
                }
            }

            Get(ROOT_INDEX).Actions = buttons;
            Get(ROOT_INDEX).OnEnter = () => {
                Get(ROOT_INDEX)
                .AddText(
                    "Select a stage."
                    );
            };
        }

        private Area GetCurrentArea(AreaType type) {
            return AreaList.ALL_AREAS[type](flags, party, previous, Root);
        }

        private bool IsStagePlayable(int index, Area area) {
            return index <= 0 || area.IsStageCleared(index - 1);
        }

        private Process GetDungeonEntryProcess(int index, Area area) {
            Dungeon dungeon = area.Dungeons[index];
            return new Process(
                    dungeon.ButtonText,
                    dungeon.Sprite,
                    () => dungeon.Invoke()
                );
        }
    }
}
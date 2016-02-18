﻿using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Meditate : Spell {
    public const string NAME = "Meditate";
    public const string DESCRIPTION = "Heal yourself.";
    public const SpellType SPELL_TYPE = SpellType.BOOST;
    public const SpellTarget TARGET_TYPE = SpellTarget.SELF;
    public static readonly string[] CAST_TEXT = new string[] { "* {0} calms their mind, restoring {1} HP!" };
    public static readonly Dictionary<ResourceType, int> COSTS = new Dictionary<ResourceType, int>() {
        {ResourceType.SKILL, 3 }
    };

    public Meditate() : base(NAME, DESCRIPTION, SPELL_TYPE, TARGET_TYPE, COSTS) {
    }

    public override double CalculateHitRate(Character caster, Character target) {
        return 1;
    }

    public override int CalculateAmount(Character caster, Character target) {
        return Amount = Math.Min(caster.GetResourceCount(ResourceType.HEALTH, true) / 2,
                                 caster.GetResourceCount(ResourceType.HEALTH, true) - caster.GetResourceCount(ResourceType.HEALTH, false));
    }

    protected override void OnHit(Character caster, Character target) {
        caster.AddToResource(ResourceType.HEALTH, false, Amount);
        if (Amount > 0) {
            OnSuccess(caster, target);
        } else {
            OnFailure(caster, target);
        }
    }

    protected override void OnSuccess(Character caster, Character target) {
        CastText = string.Format(CAST_TEXT[0], caster.Name, Amount);
        EffectsManager.CreateHitsplat(Amount, Color.green, target);
        SoundView.Instance.Play("Sounds/Zip_0");
    }

    protected override void OnFailure(Character caster, Character target) {
        CastText = string.Format(CAST_TEXT[0], caster.Name, Amount);
        EffectsManager.CreateHitsplat(Amount, Color.grey, target);
    }

    protected override void OnMiss(Character caster, Character target) {
        throw new NotImplementedException();
    }

    public override void Undo() {
        Caster.AddToResource(ResourceType.HEALTH, false, -Amount);
    }
}

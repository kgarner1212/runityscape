﻿using UnityEngine;
using System.Collections;

public class RightBox : AvatarBox {
    public override TextBoxType Type { get { return TextBoxType.RIGHT; } }

    public RightBox(string spriteLoc,
                   string text,
                   Color color,
                   TextEffect effect = TextEffect.TYPE,
                   string soundLocation = "Sounds/Blip_0",
                   float timePerLetter = 0.05f)
                   : base(spriteLoc, text, color, effect, soundLocation, timePerLetter) {
    }
}
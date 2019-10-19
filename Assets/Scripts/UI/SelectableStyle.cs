﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReleaseVersion.UI
{
    [CreateAssetMenu(menuName = "Button Style")]
    public class SelectableStyle : ScriptableObject
    {
        public Color NormalColor;
        public Color SelectedColor;
        public Color ActiveColor;
        public Color DisabledColor;
    }
}
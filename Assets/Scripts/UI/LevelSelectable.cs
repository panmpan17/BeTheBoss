using UnityEngine;
using Menu;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class LevelSelectable : SelectableButton
{
    [SerializeField]
    private SpriteRenderer targetRenderer;
    [SerializeField]
    private int levelId, requireUnlockLevel;

    private void Start() {
        disabled = !LevelSelectMgr.LevelHasUnlock(requireUnlockLevel);
        ApplyStyle();
    }

    public override void Submit() {
        if (requireUnlockLevel < 0 || LevelSelectMgr.LevelHasUnlock(requireUnlockLevel))
            LevelSelectMgr.LoadLevel(levelId);
    }

    public override void ApplyStyle()
    {
        if (style == null) return;

        if (!disabled && !actived && !selected)
        {
            Color = style.NormalColor;
        }
        else if (disabled)
        {
            if (selected) Color = Color.Lerp(style.SelectedColor, style.DisabledColor, 0.5f);
            else Color = style.DisabledColor;
        }
        else if (actived)
        {
            if (selected) Color = Color.Lerp(style.SelectedColor, style.ActiveColor, 0.5f);
            else Color = style.ActiveColor;
        }
        else if (selected) Color = style.SelectedColor;
    }
}

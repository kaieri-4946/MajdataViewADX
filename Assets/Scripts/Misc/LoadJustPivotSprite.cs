using UnityEngine;

public class LoadJustPivotSprite : LoadJustSprite
{
    protected override void refreshSprite()
    {
        spriteRenderer.sprite = Majdata<SkinManager>.Instance!
            .JustPivot[_0curv1str2wifi + indexOffset + judgeOffset];
    }
}
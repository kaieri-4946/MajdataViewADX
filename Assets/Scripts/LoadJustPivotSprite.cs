using UnityEngine;

public class LoadJustPivotSprite : LoadJustSprite
{
    protected override void refreshSprite()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = GameObject.Find("Outline").GetComponent<CustomSkin>()
            .JustPivot[_0curv1str2wifi + indexOffset + judgeOffset];
    }
}
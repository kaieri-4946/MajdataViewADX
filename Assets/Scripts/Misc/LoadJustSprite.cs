#region

using UnityEngine;

#endregion

public class LoadJustSprite : MonoBehaviour
{
    [SerializeField]
    protected int _0curv1str2wifi;
    
    protected int indexOffset;
    protected int judgeOffset = 0;

    protected SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public int setR()
    {
        indexOffset = 0;
        refreshSprite();
        return _0curv1str2wifi;
    }

    public int setL()
    {
        indexOffset = 3;
        refreshSprite();
        return _0curv1str2wifi;
    }
    public void setFastGr()
    {
        judgeOffset = 6;
        refreshSprite();
    }
    public void setFastGd()
    {
        judgeOffset = 12;
        refreshSprite();
    }
    public void setLateGr()
    {
        judgeOffset = 18;
        refreshSprite();
    }
    public void setLateGd()
    {
        judgeOffset = 24;
        refreshSprite();
    }
    public void setMiss()
    {
        judgeOffset = 30;
        refreshSprite();
    }
    protected virtual void refreshSprite()
    {
        spriteRenderer.sprite = Majdata<SkinManager>.Instance!
            .Just[_0curv1str2wifi + indexOffset + judgeOffset];
    }
}
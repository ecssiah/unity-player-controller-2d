using UnityEngine;

[System.Serializable]
public struct TriggerInfo
{
    public Surface Top;
    public Surface Mid;
    public Surface Low;

    public bool ClimbableTrigger;
    public bool LedgeContact => !Top && Mid;
    public bool WallContact => Top && Mid && Low;

    public void Reset()
    {
        Top = null;
        Mid = null;
        Low = null;

        ClimbableTrigger = false;
    }
}

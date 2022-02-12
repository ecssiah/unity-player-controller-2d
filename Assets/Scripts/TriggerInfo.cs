using UnityEngine;

[System.Serializable]
public struct TriggerInfo
{
    public Surface Top;
    public Surface Mid;
    public Surface Low;

    public void Reset()
    {
        Top = null;
        Mid = null;
        Low = null;
    }
}

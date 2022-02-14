using UnityEngine;

[System.Serializable]
public struct TriggerInfo
{
    public Surface Top;
    public Surface Mid;
    public Surface Low;

    public bool Grounded;
    public bool Climbable;
    public bool Ledge => !Top && Mid;
    public bool Wall => Top && Mid && Low;

    public void Reset()
    {
        Top = null;
        Mid = null;
        Low = null;

        Grounded = false;
        Climbable = false;
    }
}

using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerMemento
{
    public Vector3 Position;
    public Dictionary<string, int> Items;
    public int Progress;

    public PlayerMemento(Vector3 position, Dictionary<string, int> items, int progress)
    {
        Position = position;
        Items = new Dictionary<string, int>(items);
        Progress = progress;
    }
}

using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance;

    private PlayerMemento _lastCheckpointMemento;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void SaveCheckpoint(Player player)
    {
        _lastCheckpointMemento = player.CreateMemento();
    }

    public void RestoreFromLastCheckpoint(Player player)
    {
        if (_lastCheckpointMemento != null)
        {
            player.SaveMemento(_lastCheckpointMemento);
        }
    }
}

using UnityEngine;

public class MiniMapManager : MonoBehaviour
{
    [SerializeField]private DungeonGeneration dungeonGeneration;

    [SerializeField]private Sprite hideRoomSprite;
    [SerializeField]private Sprite revealedRoomSprite;
    [SerializeField]private Sprite hideTunnelSprite;
    [SerializeField]private Sprite revealedTunnelSprite;

    private void OnEnable()
    {
        
    }
}

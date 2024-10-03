using System.Collections;
using UnityEngine;

public class PickUpManager : MonoBehaviour
{
    [Header("Prefab to spawn")]
    [SerializeField] private GameObject pickUpPrefab;

    [Header("Prefab to spawn")]
    [SerializeField] private CharacterController player;

    [Header("Select Card Menu")]
    [SerializeField] private SelectCardMenu selectCardMenu;

    private GameObject prefab;
    private PickUpCollider pickUpCollider;

    public IEnumerator SpawnPickUp(int time) 
    {
        yield return new WaitForSeconds(time);

        prefab = Instantiate(pickUpPrefab, new Vector3(player.transform.position.x - 2f, player.transform.position.y + 1, player.transform.position.z), Quaternion.identity);
        pickUpCollider = prefab.GetComponent<PickUpCollider>();
        pickUpCollider.onPlayerInteractPickUp.AddListener(PlayerInteractPickUp);
    } 

    public void PlayerInteractPickUp() 
    {
        Debug.Log("Evento LLamado");
        selectCardMenu.ShowSelectCardMenu(true);
    }

    private void OnDestroy()
    {
        pickUpCollider.onPlayerInteractPickUp.RemoveListener(PlayerInteractPickUp);
    }
}

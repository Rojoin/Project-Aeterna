using System.Collections;
using UnityEngine;

public class PickUpManager : MonoBehaviour
{
    [Header("Prefab to spawn")]
    [SerializeField] private GameObject pickUpPrefab;

    [Header("Prefab to spawn")]
    [SerializeField] private CharacterController player;

    public IEnumerator SpawnPickUp(int time) 
    {
        yield return new WaitForSeconds(time);

        Instantiate(pickUpPrefab, new Vector3(player.transform.position.x - 1.5f, player.transform.position.y + 1, player.transform.position.z), Quaternion.identity);
    } 
}

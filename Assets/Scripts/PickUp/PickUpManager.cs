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

    [Header("Restart time speed")]
    [SerializeField] private float restartTimeSpeed = 0.4f;

    private GameObject prefab;
    private PickUpCollider pickUpCollider;
    private bool stopTime;

    public IEnumerator SpawnPickUp(int time) 
    {
        Time.timeScale = 0.1f;
        stopTime = true;

        yield return new WaitForSeconds(time);

        prefab = Instantiate(pickUpPrefab, new Vector3(player.transform.position.x - 2f, player.transform.position.y + 1, player.transform.position.z), Quaternion.identity);
        pickUpCollider = prefab.GetComponent<PickUpCollider>();
        pickUpCollider.onPlayerInteractPickUp.AddListener(PlayerInteractPickUp);
    }

    private void Update()
    {
        RestartTime();
    }

    public void RestartTime() 
    {
        if (stopTime) 
        {
            Time.timeScale += restartTimeSpeed * Time.deltaTime;
        }

        if(Time.timeScale >= 1) 
        {
            Time.timeScale = 1f;
            stopTime = false;
        }
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

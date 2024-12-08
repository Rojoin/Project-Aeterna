using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class PickUpManager : MonoBehaviour
{
    [Header("Prefab to spawn")] [SerializeField]
    private GameObject pickUpPrefab;

    [Header("Prefab to spawn")] [SerializeField]
    private CharacterController player;

    [Header("Select Card Menu")] [SerializeField]
    private SelectCardMenu selectCardMenu;

    [Header("Restart time speed")] [SerializeField]
    private float restartTimeSpeed = 0.4f;

    [Header("NextCardIndicator")] [SerializeField]
    private NextCardIndicator nextCardIndicator;

    private GameObject prefab;
    private PickUpCollider pickUpCollider;
    private bool stopTime;
    public float slowDownTime = 1.0f;
    public float timeUntilCardAppears = 0.5f;

    public VoidChannelSO activeSlowTime;
    public Vector3ChannelSO OnSpawnPickUpLocation;
    public VoidChannelSO OnPickUpSpawning;
    public AnimationCurve slowTimeCurve;

    public int maxSpawnPoint = 10;
    public int minSpawnPoint = 5;

    public void OnEnable()
    {
        activeSlowTime.Subscribe(StartSlowTime);
    }
[ContextMenu("Create PickUp")]
    private void TestPickUp()
    {
        StartCoroutine(SpawnPickUp());
    }
    public IEnumerator SpawnPickUp()
    {
        OnPickUpSpawning.RaiseEvent();
        yield return new WaitForSeconds(timeUntilCardAppears);

        Vector3 newPickUpPos = new Vector3(GetRandomPointOnNavMesh(player.transform.position, maxSpawnPoint, minSpawnPoint).x, 1, GetRandomPointOnNavMesh(player.transform.position, maxSpawnPoint, minSpawnPoint).z);

        prefab = Instantiate(pickUpPrefab,
            newPickUpPos,
            Quaternion.identity);

        pickUpCollider = prefab.GetComponent<PickUpCollider>();
        OnSpawnPickUpLocation.RaiseEvent(prefab.transform.position);
        pickUpCollider.onPlayerChooseCard.AddListener(PlayerInteractPickUp);
    }

    private void Update()
    {
        // RestartTime();
    }

    public void StartSlowTime()
    {
        StartCoroutine(SlowTime());
    }

    private IEnumerator SlowTime()
    {
        float time = 0;
        while (time < slowDownTime)
        {
            Time.timeScale = slowTimeCurve.Evaluate(time / slowDownTime);
            time+= Time.unscaledDeltaTime;
            yield return null;
        }
        Time.timeScale = 1;
        stopTime = false;
    }

    public void RestartTime()
    {
        // if (stopTime)
        // {
        //     Time.timeScale += restartTimeSpeed * Time.deltaTime;
        // }
        //
        // if (Time.timeScale >= 1)
        // {
        //     Time.timeScale = 1f;
        //     stopTime = false;
        // }
    }

    public void PlayerInteractPickUp()
    {
        //nextCardIndicator.RestartIndicatorCard();
        selectCardMenu.ShowSelectCardMenu(true);
    }

    private void OnDisable()
    {
        activeSlowTime.Unsubscribe(StartSlowTime);
    }

    private Vector3 GetRandomPointOnNavMesh(Vector3 center, float maxDistance, float minDistance)
    {
        NavMeshHit hit;
        Vector3 randomDirection;

        do
        {
            randomDirection = Random.insideUnitSphere * maxDistance;
            randomDirection += center;

        } while (!NavMesh.SamplePosition(randomDirection, out hit, maxDistance, NavMesh.AllAreas) ||
                 Vector3.Distance(center, hit.position) < minDistance);

        return hit.position;
    }
}
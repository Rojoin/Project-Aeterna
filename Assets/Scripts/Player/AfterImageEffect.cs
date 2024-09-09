using System.Collections;
using UnityEngine;
using UnityEngine;
using System.Collections;

public class AfterImageEffect : MonoBehaviour
{
    
    public float afterImageDuration = 1f;
    public float afterImageInterval = 0.1f;
    public int poolSize = 10;

    [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;
    [SerializeField] private GameObject meshDefault;
    [SerializeField] private Material afterMaterial;
    private GameObject[] afterImagePool;
    private int currentIndex = 0;
    private float afterImageTimer = 0f;
    private bool shouldSpawnImages;
    private Coroutine startCoroutine;

    void Awake()
    {
        InitializePool();
    }

    void Update()
    {
        // afterImageTimer += Time.deltaTime;
        // if (afterImageTimer >= afterImageInterval)
        // {
        //     SpawnAfterImage();
        //     afterImageTimer = 0f; // Reset the timer after spawning an afterimage
        // }
    }

    // Initialize the object pool for afterimages
    void InitializePool()
    {
        afterImagePool = new GameObject[poolSize];
        for (int i = 0; i < poolSize; i++)
        {
            afterImagePool[i] = Instantiate(meshDefault);
            afterImagePool[i].SetActive(false);
        }
    }

    public void StartSpawn()
    {
        if (startCoroutine != null)
        {
            StopCoroutine(startCoroutine);
        }

        shouldSpawnImages = true;
        startCoroutine = StartCoroutine(SpawnAfterImage());
    }

    public void StopSpawn()
    {
        shouldSpawnImages = false;
    }

    IEnumerator SpawnAfterImage()
    {
        while (shouldSpawnImages)
        {
            yield return new WaitForSeconds(afterImageInterval);

            Mesh afterImageMesh = new Mesh();
            skinnedMeshRenderer.BakeMesh(afterImageMesh);
            currentIndex = (currentIndex + 1) % poolSize;
            GameObject afterImage = afterImagePool[currentIndex];
            afterImage.GetComponent<MeshFilter>().mesh = afterImageMesh;
            afterImage.transform.SetPositionAndRotation(transform.position, transform.rotation);
            afterImage.GetComponent<MeshRenderer>().material.color = afterMaterial.color;
            afterImage.SetActive(true);

            StartCoroutine(FadeOut(afterImage));
            currentIndex = (currentIndex + 1) % poolSize;
        }
    }

    IEnumerator FadeOut(GameObject afterImage)
    {
        var material = afterImage.GetComponent<MeshRenderer>().material;
        float elapsedTime = 0f;
        Color initialColor = material.color;

        while (elapsedTime < afterImageDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / afterImageDuration);
            material.color = new Color(initialColor.r, initialColor.g, initialColor.b, alpha);
            yield return null;
        }

        afterImage.SetActive(false);
    }
}
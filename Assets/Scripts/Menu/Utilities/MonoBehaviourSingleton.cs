using UnityEngine;

public abstract class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviourSingleton<T>
{
	[SerializeField] private bool dontDestroyOnLoad;

	private static T instance;
	private static bool wasDestroyed;

	public static T Instance
	{
		get
		{
			if (!instance)
			{
				instance = FindObjectOfType<T>();

				if (!instance && !wasDestroyed)
					instance = new GameObject(typeof(T).Name).AddComponent<T>();
			}

			return instance;
		}
	}


	void Awake ()
	{
		if (Instance == this)
		{
			wasDestroyed = false;

			if (dontDestroyOnLoad)
				DontDestroyOnLoad(gameObject);

			OnAwaken();
		}
		else
		{
			Destroy(gameObject);
		}
	}

	void OnDestroy ()
	{
		if (instance == this)
		{
			wasDestroyed = true;
			instance = null;

			OnDestroyed();
		}
	}


	protected virtual void OnAwaken () { }
	protected virtual void OnDestroyed () { }
}
using RuntimeUndo;
using UnityEngine;

public class GameManager : Singleton<GameManager> {

    [SerializeField] private GameObject fruitPrefab;
    [SerializeField] private GameObject sliderPrefab;

    public static GameObject garbage;

    void Awake()
    {
        garbage = Instantiate(new GameObject("Garbage collection"));
    }

	void Start ()
	{
	    HitObjectManager.instance.fruitPrefab = fruitPrefab;
	    HitObjectManager.instance.sliderPrefab = sliderPrefab;
	}

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            if (Input.GetKeyDown(KeyCode.Z))
                Undo.PerformUndo();
            else if (Input.GetKeyDown(KeyCode.Y))
                Undo.PerformRedo();
        }
    }
	
}

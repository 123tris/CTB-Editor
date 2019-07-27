using RuntimeUndo;
using UnityEngine;

public class GameManager : Singleton<GameManager> {

    [SerializeField] private GameObject fruitPrefab;
    [SerializeField] private GameObject sliderPrefab;

    public Transform level;
    public static GameObject garbage;

    protected override void Awake()
    {
        base.Awake();
        if (level == null) Debug.LogError("Reference to the level instance is not set in the game manager",this);
        garbage = new GameObject("Garbage Collection");
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
            else if (Input.GetKeyDown(KeyCode.V))
                CopyManager.Paste();
        }
    }
	
}

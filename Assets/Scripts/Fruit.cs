using UnityEngine;
using UnityEngine.UI;

public class Fruit : HitObject {
    [SerializeField] private Sprite unhighlightedFruit;
    [SerializeField] private Sprite highlightedFruit;

    public int hitTime;

    public override void OnHightlight()
    {
        GetComponent<Image>().sprite = highlightedFruit;
    }
    public override void UnHighlight()
    {
        GetComponent<Image>().sprite = unhighlightedFruit;
    }
}

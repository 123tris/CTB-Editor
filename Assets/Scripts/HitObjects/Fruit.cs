using UnityEngine;
using UnityEngine.UI;

public class Fruit : HitObject {
    [SerializeField] private Sprite unhighlightedFruit;
    [SerializeField] private Sprite highlightedFruit;

    public const float OBJECT_RADIUS = 44;

    public float Scale =>
        (1.0f - 0.7f * (TextUI.Instance.CS - 5) / 5f) * HitObjectManager.WidthRatio;

    public Fruit()
    {
        type = HitObjectType.Fruit;
    }

    void Start()
    {
        UpdateCircleSize();
    }

    public override void UpdateCircleSize()
    {
        float circleSizePX = OBJECT_RADIUS * Scale;
        RectTransform r = GetComponent<RectTransform>();
        r.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, circleSizePX);
        r.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, circleSizePX);
    }

    public override void OnHightlight()
    {
        GetComponent<Image>().sprite = highlightedFruit;
    }
    public override void UnHighlight()
    {
        GetComponent<Image>().sprite = unhighlightedFruit;
    }
}

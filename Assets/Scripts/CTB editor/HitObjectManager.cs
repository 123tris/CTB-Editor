using System;
using System.Collections.Generic;
using System.Linq;
using OsuParsers.Beatmaps.Objects.Catch;
using OsuParsers.Enums.Beatmaps;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Object = UnityEngine.Object;
using PSlider = OsuParsers.Beatmaps.Objects.Slider;

public static class HitObjectManager
{
    public static GameObject sliderPrefab;
    public static GameObject fruitPrefab;

    private static Grid grid => Grid.Instance;

    private static List<Fruit> fruits = new List<Fruit>();
    private static List<Slider> sliders = new List<Slider>();

    #region DomainReloadingSupport
    [RuntimeInitializeOnLoadMethod]
    static void Init() //Static references need to be cleared if the domain is not reloaded
    {
        fruits.Clear();
        sliders.Clear();
    }
    #endregion

    /// <summary> Instantiates a fruit and adds it to the managed hitobjects of the hitobjectmanager </summary>
    public static Fruit CreateFruit(Vector2 position, Transform parent)
    {
        Fruit fruit = Object.Instantiate(fruitPrefab, parent).GetComponent<Fruit>();
        fruit.SetPosition(position);
        AddFruit(fruit);
        return fruit;
    }

    public static void CreateFruitByParser(CatchFruit hitObject)
    {
        Vector2 position = new Vector2(hitObject.Position.X, hitObject.StartTime);

        Fruit fruit = Object.Instantiate(fruitPrefab, GameManager.Instance.level).GetComponent<Fruit>();
        fruit.hitSound = hitObject.HitSound;
        fruit.hitSound ^= HitSoundType.Normal;
        fruit.isNewCombo = hitObject.IsNewCombo;
        fruit.position = position.ToVector2Int();

        float xPos = position.x * Grid.GetWidthRatio();
        float yPos = grid.GetYPosition(position.y);
        fruit.transform.position = new Vector3(xPos, yPos) + grid.transform.position;

        AddFruit(fruit);
    }

    public static void CreateSliderByParser(CatchJuiceStream pSlider)
    {
        //TODO: implement repeat sliders & remove hardcode
        if (pSlider.Repeats > 1) return;
        int startTime = pSlider.StartTime;
        int endTime = pSlider.EndTime;

        List<System.Numerics.Vector2> points = pSlider.SliderPoints;

        float xPos = pSlider.Position.X * Grid.GetWidthRatio();
        float yPos = grid.GetYPosition(startTime);

        Vector2 position = new Vector2(xPos, yPos);

        Brush brush = GameManager.Instance.brush;
        Slider slider = CreateSlider(position, brush.transform);
        slider.hitSound = pSlider.HitSound;
        slider.isNewCombo = pSlider.IsNewCombo;

        Vector2 sliderDirection = (points[0] - pSlider.Position).ToUnityVector().normalized;

        xPos += (float)(pSlider.PixelLength * sliderDirection.x * Grid.GetWidthRatio());
        yPos = grid.GetYPosition(endTime);
        position = new Vector2(xPos, yPos);

        slider.AddFruit(position);
    }

    public static Slider CreateSlider(Vector2 position, Transform parent)
    {
        Slider slider = Object.Instantiate(sliderPrefab, parent).GetComponent<Slider>();
        slider.SetPosition(position);
        slider.AddFruit(position);
        AddSlider(slider);
        return slider;
    }

    /// <summary>
    /// Adds a created fruit to the hitobjectmanager's list of fruits that it manages over
    /// <para>Use HitObjectManager.CreateFruit whenever possible</para>
    /// </summary>
    public static void AddFruit(Fruit fruit)
    {
        fruits.Add(fruit);
        fruits.Sort();
    }

    public static void AddSlider(Slider slider)
    {
        sliders.Add(slider);
        sliders.Sort();
    }

    public static void EditFruitTimeStamp(Fruit fruit, int timeStamp)
    {
        fruit.position.y = timeStamp;
        fruits.Sort();
    }

    public static Fruit GetFruitByTime(int timeStamp)
    {
        List<Fruit> fruitsAtTimeStamp = fruits.Where(fruit => fruit.position.y == timeStamp).ToList();
        if (fruitsAtTimeStamp.Count == 0)
            return null;
        else
            return fruitsAtTimeStamp.First();
    }

    public static bool ContainsFruit(int timeStamp)
    {
        return fruits.Any(fruit => fruit.position.y == timeStamp);
    }

    public static Fruit GetPreviousFruit(Fruit fruit)
    {
        if (fruit == null) return null; //Fix code that references this function that forces this check

        if (fruits.IndexOf(fruit) == 0)
            return null;

        return fruits[fruits.IndexOf(fruit) - 1];
    }

    public static HitObject GetPreviousFruitByTimeStamp(int timestamp)
    {
        foreach (Fruit fruit in fruits)
        {
            if (timestamp > fruit.position.y)
                return fruit;
        }
        return null;
    }

    public static HitObject GetNextFruit(Fruit fruit)
    {
        if (fruit == null) return null; //Fix code that references this function that forces this check

        if (fruits.IndexOf(fruit) == fruits.Count - 1)
            return null;

        return fruits[fruits.IndexOf(fruit) + 1];
    }

    public static HitObject GetNextFruitByTimeStamp(int timestamp)
    {
        foreach (Fruit fruit in fruits)
        {
            if (timestamp < fruit.position.y)
                return fruit;
        }
        return null;
    }

    /// <summary>Returns all fruits that could be either a slider fruit or independent fruit. If you want non slider fruits uses GetNonSliderFruits()</summary>
    public static List<Fruit> GetFruits() => fruits;

    public static List<Fruit> GetNonSliderFruits()
    {
        return fruits.Where(i => !i.isSliderFruit).ToList();
    }

    public static List<Slider> GetSliders() => sliders;

    ///<summary>Clears the slider and fruit lists after destroying all the containing objects</summary>
    public static void Reset()
    {
        sliders.ForEach(slider => Object.Destroy(slider.gameObject));
        fruits.ForEach(fruit => Object.Destroy(fruit.gameObject));

        //TODO: bug found where after destruction of fruits/sliders the instances weren't removed from the list so further testing required before removing commented code
        //sliders.Clear();
        //fruits.Clear();
    }

    public static void RemoveFruit(Fruit hitObject) => fruits.Remove(hitObject);

    public static void RemoveSlider(Slider slider) => sliders.Remove(slider);

    public static List<Fruit> GetFruitsByRange(float startTime, float endTime)
    {
        List<Fruit> output = new List<Fruit>();
        foreach (Fruit fruit in fruits)
        {
            if (endTime < fruit.position.y) break;

            if (startTime <= fruit.position.y)
            {
                output.Add(fruit);
            }
        }
        return output;
    }

    public static void UpdateFruits()
    {
        foreach (Fruit fruit in fruits)
        {
            //if (fruit.gameObject.activeInHierarchy)
                fruit.OnUpdate();
        }
    }
}

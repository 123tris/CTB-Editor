using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(SpriteRenderer))]
public class Catcher : MonoBehaviour
{
    public const float CATCHER_SIZE = 106.75f;
    public const double BASE_SPEED = 1.0; /*/ 512;//*/

    [SerializeField] private float maxRange = 5;
    public float dashSpeedBoost = 2;
    public float speed = 5;
    float hInput;
    bool dashInput;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        GetInput();

        if (!(transform.position.x >= maxRange && hInput > 0) //Going out of right range
            && !(transform.position.x <= -maxRange && hInput < 0)) //going out of left range
        {
            float movement = hInput * speed;
            if (dashInput)
                movement *= dashSpeedBoost;

            transform.position += new Vector3(movement * Time.deltaTime, 0);
        }

        if (hInput > 0)
            spriteRenderer.flipX = false;
        else if (hInput < 0)
            spriteRenderer.flipX = true;
    }

    void GetInput()
    {
        hInput = Input.GetAxisRaw("Horizontal");
        dashInput = Input.GetKey(KeyCode.LeftShift) || Input.GetMouseButton(0);
    }

    public static double GetCatcherSize()
    {
        return CATCHER_SIZE / HitObjectManager.DEFAULT_OSU_PLAYFIELD_WIDTH * (1.0f - 0.7f * (BeatmapSettings.CS - 5) / 5);
    }
}

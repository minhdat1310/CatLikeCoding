using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class FrameRateCounter : MonoBehaviour
{
    int frames;

    float duration, bestDuration = float.MaxValue, worstDuration;

    [SerializeField]
    TextMeshProUGUI display;

    [SerializeField, Range(0.1f, 2f)]
    float sampleDuration = 1f;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float frameDuration = Time.unscaledDeltaTime;
        frames += 1;
        duration += frameDuration;

        if (frameDuration < bestDuration)
        {
            bestDuration = frameDuration;
        }
        if (frameDuration > worstDuration)
        {
            worstDuration = frameDuration;
        }

        if (duration >= sampleDuration)
        {
            display.SetText(
                "FPS\n{0:0}\n{1:0}\n{2:0}",
                1f / bestDuration,
                frames / duration,
                1f / worstDuration
            );
            frames = 0;
            duration = 0f;
            bestDuration = float.MaxValue;
            worstDuration = 0f;
        }
    }
}

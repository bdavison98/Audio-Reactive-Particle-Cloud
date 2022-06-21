using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AudioFlow : MonoBehaviour
{
    NoiseFlow noiseFlow;
    public AudioReactive audioReactive;


    public bool useSpeed;
    public Vector2 moveSpeedMinMax, rotateSpeedMinMax;

    public bool useScale;
    public Vector2 scaleMinMax;



    public Material material;
    private Material[] audioMaterial;
    public bool useColor1;
    public bool useColor2;
    public Gradient gradient1;
    public Gradient gradient2;
    private Color[] color1;
    public string colorName1;
    private Color[] color2;
    public string colorName2;

    [Range(0f,1f)]
    public float colorThreshold1;
    public float colorMult1;
    [Range(0f,1f)]
    public float colorThreshold2;
    public float colorMult2;

    // Start is called before the first frame update
    void Start()
    {
        noiseFlow = GetComponent<NoiseFlow>();

        audioMaterial = new Material[64];
        color1 = new Color[64];
        color2 = new Color[64];
        
        for(int i = 0; i < 64; i++)
        {
            color1[i] = gradient1.Evaluate((1f/64f) * 1);
            color2[i] = gradient2.Evaluate((1f/64f) * 1);
            audioMaterial[i] = new Material(material); 
        }

        int countBand = 0;

        for(int i = 0; i < noiseFlow.maxParticles; i++)
        {
            int band = countBand % 64;

            noiseFlow.particleMeshRenderer[i].material = audioMaterial[band];
            noiseFlow.particles[i].audioBand = band;

            countBand++;

        }
    }

    // Update is called once per frame
    void Update()
    {
        if(useSpeed)
        {
            noiseFlow.particleMoveSpeed = Mathf.Lerp(moveSpeedMinMax.x, moveSpeedMinMax.y, audioReactive.AmplitudeBuffer);
            noiseFlow.particleRotateSpeed = Mathf.Lerp(rotateSpeedMinMax.x, rotateSpeedMinMax.y, audioReactive.AmplitudeBuffer);
        }
        for(int i=0; i < noiseFlow.maxParticles; i++)
        {
            if(useScale)
            {
                float scale = Mathf.Lerp(scaleMinMax.x, scaleMinMax.y, audioReactive.audioBandBuffer[noiseFlow.particles[i].audioBand]);
                noiseFlow.particles[i].transform.localScale = new Vector3(scale, scale, scale);
                
            }
            
        }
        for(int i = 0; i < 64; i++)
        {
            if(useColor1)
            {
                if(audioReactive.audioBandBuffer[i] > colorThreshold1)
                {
                    audioMaterial[i].SetColor(colorName1, color1[i] * audioReactive.audioBandBuffer[i] * colorMult1);
                }
                else
                {
                    audioMaterial[i].SetColor(colorName1, color1[i] * 0.5f);

                }
            }
            if(useColor2)
            {
                if(audioReactive.audioBand[i] > colorThreshold2)
                {
                    audioMaterial[i].SetColor(colorName2, color2[i] * audioReactive.audioBand[i] * colorMult2);
                }
                else
                {
                    audioMaterial[i].SetColor(colorName2, color2[i] * 0.5f);

                }
            }
        }

    }
}

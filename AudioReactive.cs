using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioReactive : MonoBehaviour
{
    public AudioSource audio;
    public static int freqBandLength = 64;
    public float[] samples = new float[512];
    float[] frequencyBands = new float[freqBandLength];
    public static float[] bandBuffer = new float[freqBandLength];
    float[] bufferDecrease = new float[freqBandLength];
    float[] maxFreq = new float[freqBandLength];
    public float[] audioBand = new float[freqBandLength];
    public float[] audioBandBuffer = new float[freqBandLength];

    public float Amplitude, AmplitudeBuffer;
    public float audioProfile;
    float amplitudeMax;
    void spectrumAnalysis() {

        audio.GetSpectrumData(samples, 0,FFTWindow.Blackman);
    }

    void AudioBands()
    {
        for(int f = 0; f < freqBandLength; f++)
        {
            if(frequencyBands[f] > maxFreq[f])
            {
                maxFreq[f] = frequencyBands[f];
            }
            audioBand[f] = (frequencyBands[f] / maxFreq[f]);
            audioBandBuffer[f] = (bandBuffer[f] / maxFreq[f]);
        
        }
    }

    void AudioProfile(float profile)
    {
        for(int i = 0; i < freqBandLength; i++)
        {
            maxFreq[i] = profile; 
        }
    }

    void BandBuffer(){
        for(int f = 0; f < freqBandLength; f++)
        {
            if(frequencyBands[f] > bandBuffer[f])
            {
                bandBuffer[f] = frequencyBands[f];
                bufferDecrease[f] = 0.01f;
            }

            if(frequencyBands[f] < bandBuffer[f])
            {
                bandBuffer[f] -= bufferDecrease[f];
                bufferDecrease[f] *= 1.2f;
            }
        }
    }
    void MakeFreqBands()
    {
        int count = 0;

        for(int f = 0; f < freqBandLength; f++)
        {
            //int sampleCount = (int)Mathf.Pow(2,f) * 2;
            int sampleCount = (int)Mathf.Lerp(2f, samples.Length-1, Mathf.Pow(f,2) / Mathf.Pow(frequencyBands.Length-1, 2))-1;
            
            float avg = 0;
            if(f==freqBandLength-1)
            {
                sampleCount +=2;
            }

            for(int s = count; s < sampleCount; s++)
            {                
                avg += samples[count] * (count + 1);
                count++;
            }
            avg /= count;
            frequencyBands[f] = avg * 10;
        }

    }

    void GetAmplitude()
    {
        float currentAmplitude = 0;
        float currentAmplitudeBuffer = 0;

        for(int i = 0; i < freqBandLength; i++)
        {
            currentAmplitude += audioBand[i];
            currentAmplitudeBuffer += audioBandBuffer[i];


            if(currentAmplitude >= amplitudeMax)
            {
                amplitudeMax = currentAmplitude;
                
            }
        }
        
        if(Time.frameCount == 0) {
            Amplitude = Mathf.Clamp01(0.2f / 1);
            AmplitudeBuffer = Mathf.Clamp01(0.2f / 1);
        }

        Amplitude = Mathf.Clamp01(currentAmplitude / amplitudeMax);
        AmplitudeBuffer = Mathf.Clamp01(currentAmplitudeBuffer / amplitudeMax);


    }

    // Start is called before the first frame update
    void Start()
    {
        audio = GetComponent<AudioSource>();
        audioProfile = 3f;
        AudioProfile(audioProfile);
    }

    // Update is called once per frame
    void Update()
    {
        spectrumAnalysis();
        MakeFreqBands();
        BandBuffer();
        AudioBands();
        GetAmplitude();
    }

}

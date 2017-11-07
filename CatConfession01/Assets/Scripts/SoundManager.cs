using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]	//AudioSourceは必須.
[DisallowMultipleComponent]		//複数アタッチさせない.
public class SoundManager : MonoBehaviour {


    public float GetLoudness()
    {
        return loudness;
    }

    public float sensitivity = 100;

    float loudness;
    float lastLoudness;

    [Range(0, 0.95f)]
    public float lastLoudnessInfluence;


    void InitRecord()
    {
        GetComponent<AudioSource>().clip = Microphone.Start(null, true, 10, 44100);
        GetComponent<AudioSource>().loop = true;
        GetComponent<AudioSource>().mute = true;
        while (!(Microphone.GetPosition("") > 0)) { }
        GetComponent<AudioSource>().Play();

    }


    void Update()
    {
        CalcLoudness();
    }

    //現フレームの音量を計算します.
    void CalcLoudness()
    {
        lastLoudness = loudness;
        loudness = GetAveragedVolume() * sensitivity * (1 - lastLoudnessInfluence) + lastLoudness * lastLoudnessInfluence;
    }

    //現在フレームで再生されているAudioClipから平均音量を出す。
    float GetAveragedVolume()
    {
        float[] data = new float[256];
        float a = 0;
        GetComponent<AudioSource>().GetOutputData(data, 0);
        foreach (float s in data)
        {
            a += Mathf.Abs(s);
        }
        //平均を返します.
        return a / 256;
    }


}

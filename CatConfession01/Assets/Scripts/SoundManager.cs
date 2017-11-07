using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour {


    public float GetAveValume()
    {
        return vol;
    }

    float vol;

    private new AudioSource audio;
    void Start()
    {
        audio = GetComponent<AudioSource>();
        audio.clip = Microphone.Start(null, true, 999, 44100);  // マイクからのAudio-InをAudioSourceに流す
        audio.loop = true;                                      // ループ再生にしておく
 //       audio.mute = true;                                      // マイクからの入力音なので音を流す必要がない
        while (!(Microphone.GetPosition("") > 0)) { }             // マイクが取れるまで待つ。空文字でデフォルトのマイクを探してくれる
        audio.Play();                                           // 再生する
    }

    void Update()
    {
        vol = GetAveragedVolume();
//        Debug.Log(vol);
    }

    float GetAveragedVolume()
    {
        float[] data = new float[256];
        float a = 0;
        audio.GetOutputData(data, 0);
        foreach (float s in data)
        {
            a += Mathf.Abs(s);
        }
        return a / 256.0f;
    }


}

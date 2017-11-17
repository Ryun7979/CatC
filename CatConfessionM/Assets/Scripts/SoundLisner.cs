using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class SoundLisner : MonoBehaviour {


    public float GetAveValume()
    {
        return vol;
    }
    public int GetSoundScale()
    {
        return soundScaleInt;
    }


    float vol;
    float qsmp;

    private int qSample = 1024; //配列のサイズ
    private float threshold = 0.04f; //ピッチとして検出する最小分布
    private float pitchValue = 99;   //ピッチの周波数

    float[] spectrum = new float[1024]; //FFTされたデータ
    private float fSample;  //サンプリング周波数
    private string soundScaleTex;
    private int soundScaleInt;

    new AudioSource audio;

    void Start()
    {
        audio = GetComponent<AudioSource>();
        audio.clip = Microphone.Start(null, true, 999, 44100);  // マイクからのAudio-InをAudioSourceに流す
        audio.loop = true;                                      // ループ再生にしておく
        while (!(Microphone.GetPosition("") > 0)) { }             // マイクが取れるまで待つ。空文字でデフォルトのマイクを探してくれる
        audio.Play();                                           // 再生する
    }

    void Update()
    {
        vol = GetAveragedVolume();
        qsmp = ConvertHertzToScale(AnalyzeSound());
        soundScaleInt = ConvertScaleToInt(qsmp);

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


    float AnalyzeSound()
    {
        audio.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);

        float maxV = 0;
        int maxN = 0;
        //最大値(ピッチ)を見つける。ただし閾値は越えている。
        for(int i = 0; i < qSample; i++)
        {
            if (spectrum[i] > maxV && spectrum[i] > threshold)
            {
                maxV = spectrum[i];
                maxN = i;
            }
        }

        float freqN = maxN;
        if (maxN > 0 && maxN < qSample - 1)
        {
            //隣のスペクトルも考慮
            float dL = spectrum[maxN - 1] / spectrum[maxN];
            float dR = spectrum[maxN + 1] / spectrum[maxN];
            freqN += 0.5f * (dR * dR - dL * dL);
        }

        fSample = AudioSettings.outputSampleRate;
        pitchValue = freqN * (fSample / 2) / qSample;

        return pitchValue;

    }

    //ヘルツから音階へ変換
    public float ConvertHertzToScale(float hertz)
    {
        float value = 0.0f;
        if (hertz == 0.0f) return value;
        else
        {
            value = 12.0f * Mathf.Log(hertz / 110.0f) / Mathf.Log(2.0f);
            return value;
        }
    }

    //数値音階から文字音階への変換

    int ConvertScaleToInt(float scale)
    {
        //12音階の何倍の精度で音階をみるか
        int precision = 2;

        //今の場合だと、mod24が0ならA、1ならA#の間、2ならA#....
        int s = (int)scale;
        if (scale - s >= 0.5) s += 1;   //四捨五入
        s *= precision;

        int smod = s % (12 * precision);    //音階
//        int soct = s / (12 * precision);    //オクターブ

        int value;   //返す値

        if (smod == 0) value = 1;           //"A";
        else if (smod == 1) value = 1;      //"A+";
        else if (smod == 2) value = 2;      //"A#";
        else if (smod == 3) value = 2;      //"A#+";
        else if (smod == 4) value = 3;      //"B";
        else if (smod == 5) value = 3;      //"B+";
        else if (smod == 6) value = 4;      //"C";
        else if (smod == 7) value = 4;      //"C+";
        else if (smod == 8) value = 5;      //"C#";
        else if (smod == 9) value = 5;      //"C#+";
        else if (smod == 10) value = 6;     //"D";
        else if (smod == 11) value = 6;     //"D+";
        else if (smod == 12) value = 7;     //"D#";
        else if (smod == 13) value = 7;     //"D#+";
        else if (smod == 14) value = 8;     //"E";
        else if (smod == 15) value = 8;     //"E+";
        else if (smod == 16) value = 9;     //"F";
        else if (smod == 17) value = 9;     //"F+";
        else if (smod == 18) value = 10;    //"F#";
        else if (smod == 19) value = 10;    //"F#+";
        else if (smod == 20) value = 11;    //"G";
        else if (smod == 21) value = 11;    //"G+";
        else if (smod == 22) value = 12;    //"G#";
        else value = 12;                    //"G#+";
//        value += soct + 1;

        return value;
    }



}

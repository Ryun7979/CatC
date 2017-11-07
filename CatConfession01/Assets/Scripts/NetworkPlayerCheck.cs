using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;

public class NetworkPlayerCheck : Photon.MonoBehaviour {



    //同期するデータ
    Vector3 position;
    Quaternion rotation;
    Animator animator;
    float speed;


    //スムーズ度合い
    float smooth = 10f;

	// Use this for initialization
	void Start () {

        animator = GetComponent<Animator>();


        //自分で操作する以外のキャラクターの不要な機能は使えないようにしておく。
        if (!photonView.isMine)
        {
            GetComponent<Chara>().enabled = false;

            //初期値を設定
            position = transform.position;
            rotation = transform.rotation;

            speed = animator.GetFloat("Speed");

            //自分のキャラクター以外のデータを同期
            StartCoroutine("UpdateMove");
        

        }
		
	}

    //自分のキャラクター以外のデータを同期するためのコルーチン
    IEnumerator UpdateMove()
    {
        while (true)
        {
            //スムーズに補間させる
            transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * smooth);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * smooth);
            animator.SetFloat("Speed", speed);

            yield return null;

        }
    }

    //observed componentsに設定したスクリプトで呼ばれるメソッド
    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //データの読み込み
        if (stream.isReading)
        {
            position = (Vector3)stream.ReceiveNext();
            rotation = (Quaternion)stream.ReceiveNext();
            speed = (float)stream.ReceiveNext();

            //データの書き込み
        }
        else
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(animator.GetFloat("Speed"));
        }

    }

	// Update is called once per frame
	void Update () {

    }
}

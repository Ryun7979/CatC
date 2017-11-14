using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour {


    public Vector3 difference;
    static string pN;

    // Use this for initialization
    void Start()
    {


        //pNの中にプレイヤーの名前を拾っておく
        pN = NetworkManager.pName;

    }

    // Update is called once per frame
    void Update()
    {

        //pNに入ってるプレイヤーの名前を使ってカメラのターゲット先を指定する
        if (GameObject.Find(pN) == true)
        {
            Vector3 startVec = GameObject.Find(pN).transform.localPosition;
            transform.localPosition = new Vector3(startVec.x + difference.x, difference.y, startVec.z + difference.z);
        }

    }

}

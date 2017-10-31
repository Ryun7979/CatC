using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour {


    public Transform target;        //ターゲットへの参照
    private Vector3 offset;         //相対座標

	// Use this for initialization
	void Start () {
        offset = GetComponent<Transform>().position - target.position;
		
	}
	
	// Update is called once per frame
	void Update () {

        //自分の座標にtargetの座標を入れる
        GetComponent<Transform>().position = target.position + offset;
		
	}
}

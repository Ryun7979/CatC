using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OperationName : MonoBehaviour {

    private GameObject namePlate;   //名前を表示しているプレート
    public Text nameText;       //名前のテキスト

	// Use this for initialization
	void Start () {

        namePlate = nameText.transform.parent.gameObject;
		
	}


    private void LateUpdate()
    {
       namePlate.transform.rotation = Camera.main.transform.rotation;
    }

    [PunRPC]
    void SetName(string name)
    {
        nameText.text = name;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkToys : Photon.MonoBehaviour {

    private Vector3 correctToysPos;
    private Quaternion correctToysRot;
    
    
    // Update is called once per frame
    void Update()
    {
        //部屋を作った人じゃないなら
        if (!photonView.isMine)
        {
//            GetComponent<Rigidbody>().isKinematic = true;
            transform.position = Vector3.Lerp(transform.position, this.correctToysPos, Time.deltaTime * 10);
            transform.rotation = Quaternion.Lerp(transform.rotation, this.correctToysRot, Time.deltaTime * 10);

//            Debug.Log("部屋作成者でないよ");
        }
    }

    //こっち側が、同期するべきデータの内容。
    public void OnPhotonSerializeView(PhotonStream stream,PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            this.correctToysPos = (Vector3)stream.ReceiveNext();
            this.correctToysRot = (Quaternion)stream.ReceiveNext();
        }
    }

	
}

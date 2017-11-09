using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chara : MonoBehaviour {


    SoundLisner soundLisner;  //サウンドマネージャーの初期化


    private Animator animator;
    private CharacterController cCon;
    private float x;
    private float y;
    private Vector3 velocity;
    private int VoiceNo;

    public float jumpPower;
    private PhotonView myPhotonView;

    // Use this for initialization
    void Start () {
        animator = GetComponent<Animator>();
        cCon = GetComponent<CharacterController>();
        velocity = Vector3.zero;


        //サウンドマネージャーのオブジェクト
        soundLisner = GameObject.FindObjectOfType<SoundLisner>();
    }

    // Update is called once per frame
    void Update () {

        myPhotonView = this.GetComponent<PhotonView>();
        float lou = soundLisner.GetAveValume();


        //地面に設置しているときは初期化
        if (cCon.isGrounded)
        {
            velocity = Vector3.zero;

            x = Input.GetAxis("Horizontal");
            y = Input.GetAxis("Vertical");
            Vector3 input = new Vector3(x, 0, y);

            //方向キーが多少押されている
            if(input.magnitude > 0.1f && !animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
            {
                animator.SetFloat("Speed", input.magnitude);
                transform.LookAt(transform.position + input);
                velocity += input.normalized * 1f;
                //キーの押しが少なすぎるときは移動しない
            }
            else
            {
                animator.SetFloat("Speed", 0);
            }

            if(Input.GetButtonDown("Jump"))
            {
                velocity.y += jumpPower;
            }
        }

        velocity.y += Physics.gravity.y * Time.deltaTime;
        cCon.Move(velocity * Time.deltaTime);


        if (lou > 0.05)
        {
            VoiceNo = 0;

            if (lou > 0.1)
            {
                VoiceNo = 1;
            }else if (lou > 0.3)
            {
                VoiceNo = 2;
            }

            this.myPhotonView.RPC("CatHowl", PhotonTargets.All);
        }

    }

    [PunRPC]
    void CatHowl(){

        SoundManager.Instance.PlayVoice(VoiceNo);

    }


    //Toysタグが付いたオブジェクトとの当たり判定

    void OnControllerColliderHit(ControllerColliderHit hit)
    {

        //物体を押す処理（Toysタグのついてるオブジェクトは押せる）
        if (hit.gameObject.tag == "Toys")
        {
            Rigidbody body = hit.collider.attachedRigidbody;

            if (body == null || body.isKinematic) { return; }    //rigidBodyがない、もしくは物理演算の影響を受けない設定をされている 
            if (hit.moveDirection.y < -0.3) { return; }            //押す力が弱い 

            Vector3 pushDir = new Vector3(hit.moveDirection.x, 1, hit.moveDirection.z);    //y成分を１に 

            float pushPower = 3.0f;
            body.velocity = pushDir * pushPower;    //押す力を加える 
        }

    }


}

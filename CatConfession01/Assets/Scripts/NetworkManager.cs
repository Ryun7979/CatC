using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviour {


    SoundLisner soundLisner;  //サウンドマネージャーの初期化

    public Text text;   //情報用のテキスト
    public Text MicIndex;

    public GameObject loginUI;      //ログイン画面
    public GameObject logoutUI;     //ログアウトボタン

    public Dropdown roomList;       //部屋リストを表示するドロップダウン
    public InputField roomName;     //部屋の名前
    public InputField playerName;   //プレイヤーの名前
    public static string pName;    //入力された名前を格納しておく。

//    private bool connectFailed = false;
    public GameObject player;
    public GameObject Toyball;
    public GameObject titleCamera;   //タイトル画面用のカメラ
    public GameObject cameraPrefab;


    // Use this for initialization
    void Start()
    {
        //ログをすべて表示する
        PhotonNetwork.logLevel = PhotonLogLevel.Full;
        
        //ロビーに自動で入る
        PhotonNetwork.autoJoinLobby = true;

        //ゲームのバージョン設定
        PhotonNetwork.ConnectUsingSettings("0.1");

        //サウンドマネージャーのオブジェクト
        soundLisner = GameObject.FindObjectOfType<SoundLisner>();

        Debug.Log("開始");

    }


    // Update is called once per frame
    void Update()
    {

        text.text = PhotonNetwork.connectionStateDetailed.ToString();

        float lou = soundLisner.GetAveValume();
        MicIndex.text = lou.ToString();
    }


    //マスターサーバへの接続完了
    void OnConnectedToMastar()
    {
        Debug.Log("マスターサーバに接続");
    }

    //ロビーに入った
    void OnJoinedLobby()
    {
        Debug.Log("ロビーに入る");
        loginUI.SetActive(true);
    }


    //ログインボタンを押したときに実行
    public void LoginGame()
    {
        //ルームオプションを設定
        RoomOptions ro = new RoomOptions();
        //ルームを見えるようにする
        ro.IsVisible = true;
        //部屋の入室最大数
        ro.MaxPlayers = 10;

        if (roomName.text != "")
        {
            //部屋がない場合は作って入室
            PhotonNetwork.JoinOrCreateRoom(roomName.text, ro, TypedLobby.Default);
        }
        else
        {
            //部屋が存在したら
            if(roomList.options.Count != 0)
            {
                Debug.Log(roomList.options[roomList.value].text);
                PhotonNetwork.JoinRoom(roomList.options[roomList.value].text);
            //部屋が無ければDefaultRoomという名前で部屋を作成
            }
            else
            {
                PhotonNetwork.JoinOrCreateRoom("DefaultRoom", ro, TypedLobby.Default);
            }
        }
    }


    void OnReceivedRoomListUpdate()
    {
        Debug.Log("部屋更新");

        //部屋情報を取得
        RoomInfo[] rooms = PhotonNetwork.GetRoomList();

        //ドロップダウンリストに追加する文字列用リストを作成
        List<string> list = new List<string>();

        //部屋情報を部屋リストに表示
        foreach (RoomInfo room in rooms)
        {
            //部屋が満員でなければ追加
            if(room.PlayerCount < room.MaxPlayers)
            {
                list.Add(room.Name);
            }
        }

        //ドロップダウンリストをリセット
        roomList.ClearOptions();

        //部屋が一つでもあれば、ドロップダウンリストに追加
        if (list.Count != 0)
        {
            roomList.AddOptions(list);
        }

    }

    //部屋に入室したときに呼ばれるメソッド
    void OnJoinedRoom()
    {
        loginUI.SetActive(false);
        logoutUI.SetActive(true);
        Debug.Log("入室");

        //Inputfieldに入力した名前を設定
        if(playerName.text != "")
        {
            PhotonNetwork.player.NickName = playerName.text;
            pName = playerName.text;
        }
        else
        {
            PhotonNetwork.player.NickName = "HUNTER";
            pName = "HUNTER";
        }

        Instantiate(cameraPrefab, new Vector3(0f, 0.5f, -1.2f), Quaternion.identity);   //ゲーム内用のカメラを作成
        titleCamera.SetActive(false);   //タイトル画面用のカメラを削除

        //ToyItemを出現させる。
        Vector3 Toypos = new Vector3(0.2f,0.5f,1.0f);   //ボールが出てくる場所指定
        Toyball = PhotonNetwork.InstantiateSceneObject("ball", Toypos, Quaternion.identity, 0,null);

        //ログインを監視する
        StartCoroutine("SetPlayer", 0f);

    }

    //プレイヤーをゲームの世界に出現させる
    IEnumerator SetPlayer(float time)
    {
        yield return new WaitForSeconds(time);
        player = PhotonNetwork.Instantiate("cat", Vector3.up, Quaternion.identity, 0);
        player.name = pName;        //作成されたプレイヤーのインスタンスに名前を付ける
        player.GetPhotonView().RPC("SetName", PhotonTargets.AllBuffered, PhotonNetwork.player.NickName);
    }



    //部屋の入室に失敗したときに呼ばれるメソッド
    void OnPhotonJoinedRoomFailed()
    {
        Debug.Log("入室に失敗");

        //ルームオプションを設定
        RoomOptions ro = new RoomOptions();
        //ルームを見えるようにする
        ro.IsVisible = true;
        //部屋の入室最大人数
        ro.MaxPlayers = 10;
        //入室に失敗したらDefaultRoomを作成し入室
        PhotonNetwork.JoinOrCreateRoom("DefaultRoom", ro, TypedLobby.Default);
    }


    //接続が切断されたときにコール
    public void OnDisconnectedFromPhoton()
    {
        Debug.Log("接続が切れたよぉ。Disconnected from Photon.");
    }


    //ログアウトボタンを押したときの処理
    public void LogoutGame()
    {
        PhotonNetwork.LeaveRoom();
        titleCamera.SetActive(true);   //タイトル画面用のカメラを作成

    }

    //部屋を退室したときの処理
    void OnLeftRoom()
    {
        Debug.Log("退室");
        logoutUI.SetActive(false);
    }




}

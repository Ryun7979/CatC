using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviour {


    SoundLisner soundLisner;  //サウンドマネージャーの初期化

    public Text text;   //情報用のテキスト
    public Text MicIndex;
    public Text pitchTxt;

    public GameObject loginUI;      //ログイン画面
    public GameObject logoutUI;     //ログアウトボタン
    public GameObject mobileUI;     //バーチャルパッド

    public Dropdown roomList;       //部屋リストを表示するドロップダウン
    public InputField roomName;     //部屋の名前
    public static string pName;    //入力された名前を格納しておく。

//    private bool connectFailed = false;
    public GameObject player;
    public GameObject titleCamera;   //タイトル画面用のカメラ
    public GameObject mainCamera;   //ゲーム中のカメラ

//    public GameObject Toyball;
    public GameObject ToyScball;
    public GameObject ToyTeddy;

    private int playerCountRoom1;
    private int playerCountRoom2;
    private int playerCountRoom3;


    // Use this for initialization
    void Start()
    {

        //UIの初期化
        logoutUI.SetActive(false);
        mobileUI.SetActive(false);

        //ドロップダウンリストに追加する文字列用リストを作成
        List<string> list = new List<string>();
        list.Add("Room1");
        list.Add("Room2");
        list.Add("Room3");
        roomList.AddOptions(list);


        //ログをすべて表示する
        PhotonNetwork.logLevel = PhotonLogLevel.Full;
        
        //ロビーに自動で入る
        PhotonNetwork.autoJoinLobby = true;

        //ゲームのバージョン設定
        PhotonNetwork.ConnectUsingSettings("v0.1");

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

        if(lou > 0.05)
        {
            int sndScale = soundLisner.GetSoundScale();
            pitchTxt.text = sndScale.ToString();
        }


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

        //リストから選んだ部屋を
        if (roomList.options.Count != 0)
        {
            Debug.Log(roomList.options[roomList.value].text);
            PhotonNetwork.JoinOrCreateRoom(roomList.options[roomList.value].text, ro, TypedLobby.Default);
        //部屋が無ければDefaultRoomという名前で部屋を作成
        }
    }
    

    void OnReceivedRoomListUpdate()
    {
        Debug.Log("部屋更新");

        //部屋情報を取得
        RoomInfo[] rooms = PhotonNetwork.GetRoomList();

        foreach (RoomInfo room in rooms)
        {
            if (room.Name == "Room1") playerCountRoom1 = room.PlayerCount;
            else if (room.Name == "Room2") playerCountRoom2 = room.PlayerCount;
            else if (room.Name == "Room3") playerCountRoom3 = room.PlayerCount;

            Debug.Log("なまえは～："+room.Name + playerCountRoom1 + playerCountRoom2 + playerCountRoom3);  //部屋名
            Debug.Log("いまいるひとは～："+room.PlayerCount);    //部屋の入場人数
            Debug.Log("さいだいは～："+room.MaxPlayers);  //最大人数
        }


        /*
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

        */

    }

    //部屋に入室したときに呼ばれるメソッド
    void OnJoinedRoom()
    {
        loginUI.SetActive(false);
        logoutUI.SetActive(true);
        mobileUI.SetActive(true);
        Debug.Log("入室");

        //プレイヤーの名前は、ROOMの中の人数から自動決定
        pName = "CAT_" + PhotonNetwork.room.playerCount;
        PhotonNetwork.player.NickName = pName;

        /*
        //Inputfieldに入力した名前を設定
        if (playerName.text != "")
        {
            PhotonNetwork.player.NickName = playerName.text;
            pName = playerName.text;
        }
        else
        {
            PhotonNetwork.player.NickName = "HUNTER";
            pName = "HUNTER";
        }
        */

        titleCamera.SetActive(false);   //タイトル画面用のカメラを無効
        mainCamera.SetActive(true);     //ゲーム内のカメラを有効

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


        ToyScball = PhotonNetwork.InstantiateSceneObject("Soccer Ball", new Vector3(-0.2f, 0.5f, 0.8f), Quaternion.identity, 0, null);
        ToyTeddy = PhotonNetwork.InstantiateSceneObject("Teddybear", new Vector3(0.1f, 0.5f, 1.5f), Quaternion.identity, 0, null);
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
        titleCamera.SetActive(true);   //タイトル画面用のカメラを有効
        mainCamera.SetActive(false);    //ゲーム中のカメラを無効
    }

    //部屋を退室したときの処理
    void OnLeftRoom()
    {
        Debug.Log("退室");
        logoutUI.SetActive(false);
        mobileUI.SetActive(false);

    }


}

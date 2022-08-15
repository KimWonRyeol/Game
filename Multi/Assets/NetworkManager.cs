using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public GameObject RespawnPanel;
    public GameObject DisconnectPanel;
    public TMP_InputField InputNickname;


    void Awake()
    {
        //server�� ������ �������� ����? �� �𸣰ڴ�. ���� ���ٰ� �Ѵ�..
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;
        Screen.SetResolution(960, 540, false);
    }

    private void Update()
    {
        //���� �߿� ESC ��ư�� ������ ���� ����
        if (Input.GetKeyDown(KeyCode.Escape) && PhotonNetwork.IsConnected) PhotonNetwork.Disconnect();
    }

    public void Connect() => PhotonNetwork.ConnectUsingSettings();
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.LocalPlayer.NickName = InputNickname.text;
        PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions { MaxPlayers = 6 }, null);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity);
        DisconnectPanel.SetActive(false);
    }


    public void Spawn()
    {
        PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity);
        RespawnPanel.SetActive(false);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        DisconnectPanel.SetActive(true);
        RespawnPanel.SetActive(false);
    }
}
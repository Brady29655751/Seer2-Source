using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoomManager : Manager<RoomManager>
{
    [SerializeField] private RoomSettingsView roomSettingsView;
    [SerializeField] private PetBagPanel petBagPanel;

    protected override void Awake()
    {
        base.Awake();
        InitSubscriptions();
    }

    private void Start() {
        InitPlayer();
        SetMyReadyProperty(false);
    }

    private void InitSubscriptions() {
        NetworkManager.instance.onPlayerPropsUpdateEvent += OnPlayerReady;
        NetworkManager.instance.onOtherPlayerJoinedRoomEvent += OnOtherPlayerJoin;
        NetworkManager.instance.onOtherPlayerLeftRoomEvent += OnOtherPlayerLeft;
    }

    private void RemoveSubscriptions() {
        NetworkManager.instance.onPlayerPropsUpdateEvent -= OnPlayerReady;
        NetworkManager.instance.onOtherPlayerJoinedRoomEvent -= OnOtherPlayerJoin;
        NetworkManager.instance.onOtherPlayerLeftRoomEvent -= OnOtherPlayerLeft;
    }

    private void InitPlayer() {
        petBagPanel.SetActive(true);
        petBagPanel.SetActive(false);

        var room = PhotonNetwork.CurrentRoom.CustomProperties;
        var player = PhotonNetwork.LocalPlayer.CustomProperties;
        int seed = Random.Range(int.MinValue, int.MaxValue);
        int petCount = (int)room["count"];

        if (PhotonNetwork.IsMasterClient) {
            room["seed"] = seed;
            PhotonNetwork.CurrentRoom?.SetCustomProperties(room);
        }

        var myPets = Enumerable.Repeat(3, petCount).Select(x => Pet.GetExamplePet(x));
        petBagPanel.SetPetBag(myPets.ToArray());
        roomSettingsView.SetPet(myPets.ToList(), true);
    }

    public void LeaveRoom() {
        RemoveSubscriptions();
        SetMyReadyProperty(false);
        NetworkData networkData = new NetworkData() {
            networkAction = NetworkAction.Leave,
        };
        NetworkManager.instance.StartNetworkAction(networkData);
    }

    private void OnOtherPlayerJoin(Photon.Realtime.Player player) {
        roomSettingsView.SetName(player.NickName, false);
        roomSettingsView.SetPet(((int[])player.CustomProperties["pet"])?.Select(x => Pet.GetExamplePet(x)).ToList(), false);
    }

    private void OnOtherPlayerLeft(Photon.Realtime.Player player) {
        roomSettingsView.SetName("未知", false);
        roomSettingsView.SetPet(null, false);
    }

    private void OnPlayerReady(Photon.Realtime.Player player) {
        var allPlayers = PhotonNetwork.PlayerList;        
        var otherPlayer = PhotonNetwork.PlayerListOthers;
        var hash = player.CustomProperties;
        if ((!player.IsLocal) && (bool)hash["ready"]) {
            roomSettingsView.SetReady(null, true, false);
            roomSettingsView.SetPet(((int[])hash["pet"]).Select(x => Pet.GetExamplePet(x)).ToList(), false);
        }

        bool isAllReady = (allPlayers.Length > 1) && allPlayers.All(x => (bool)x.CustomProperties["ready"]);
        if (isAllReady) {
            Battle battle = new Battle(
                PhotonNetwork.CurrentRoom.CustomProperties,
                PhotonNetwork.LocalPlayer.CustomProperties,
                otherPlayer[0].CustomProperties
            );
            StartBattle();
        }
    }

    private void SetMyReadyProperty(bool isReady) {
        var hash = PhotonNetwork.LocalPlayer.CustomProperties;
        hash["ready"] = isReady;

        if (!isReady) {
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
            return;
        }

        hash["pet"] = petBagPanel.petBag.Where(x => x != null).Select(x => x.id).ToArray();
        hash["char"] = petBagPanel.petBag.Where(x => x != null).Select(x => (int)x.basic.personality).ToArray();
        hash["ev"] = petBagPanel.petBag.Where(x => x != null).Select(x => x.talent.ev.ToArray()).ToArray();
        hash["skill"] = petBagPanel.petBag.Where(x => x != null).Select(x => x.normalSkill.Select(x => x.id).ToArray()).ToArray();
        hash["super"] = petBagPanel.petBag.Where(x => x != null).Select(x => x.superSkill.id).ToArray();
        
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

        return;
    }

    public void SetMyReady(bool isReady) {
        roomSettingsView.SetReady(() => SetMyReadyProperty(isReady), isReady, true);
    }

    public void StartBattle() {
        SceneLoader.instance.ChangeScene(SceneId.Battle, true);
    }

    public void OpenPetBag() {
        petBagPanel.SetActive(true);
    }

    public void ClosePetBag() {
        roomSettingsView.SetPet(petBagPanel.petBag.ToList(), true);
        petBagPanel.SetActive(false);
    }
}

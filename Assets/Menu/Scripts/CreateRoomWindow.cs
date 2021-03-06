﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateRoomWindow : MonoBehaviour
{
    public Toast toast;                     // 提示标签
    public InputField roomName;             // 房间名称
    public InputField roomSize;             // 房间大小
    public Button createRoomButton;         // 创建房间按钮
    public int minRoomSize = 1;             // 房间最小容量
    public int maxRoomSize = 4;             // 房间最大容量

    private int createRoomSize;             // 创建房间的大小

    /// <summary>
    /// 限制房间大小
    /// </summary>
    public void RoomSizeClamp()
    {
        if (string.IsNullOrEmpty(roomSize.text))
            return;
        createRoomSize = Mathf.Clamp(int.Parse(roomSize.text), minRoomSize, maxRoomSize);
        roomSize.text = createRoomSize.ToString();
    }

    /// <summary>
    /// 创建房间
    /// </summary>
    public void CreateRoom()
    {
        if (!RoomInputInfoCompleted())
            return;
        createRoomButton.interactable = false;
        PhotonNetwork.CreateRoom(roomName.text, new RoomOptions() { MaxPlayers = (byte)createRoomSize }, null);
    }

    /// <summary>
    /// 检查房间输入信息是否完整，不完整显示提示
    /// </summary>
    /// <returns>房间信息输入完整返回True</returns>
    public bool RoomInputInfoCompleted()
    {
        if (string.IsNullOrEmpty(roomName.text) || string.IsNullOrEmpty(roomSize.text))
        {
            toast.ShowToast(3f, "房间信息不完整。");
            return false;
        }
        return true;
    }

    /// <summary>
    /// 创建房间成功时响应
    /// </summary>
    public void OnCreatedRoom()
    {
        Debug.Log("创建房间成功");
        createRoomButton.interactable = true;
        AllSceneManager.LoadScene(GameScene.RoomScene);
    }

    /// <summary>
    /// 创建失败
    /// </summary>
    public void OnPhotonCreateRoomFailed()
    {
        Debug.Log("创建房间失败");
        createRoomButton.interactable = true;
        toast.ShowToast(3f, "该房间已存在。");
    }


}

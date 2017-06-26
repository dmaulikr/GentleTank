﻿using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public abstract class Skill : MonoBehaviour
{
    public float coolDownTime = 1f;                 // 冷却时间
    public Slider coolDownSlider;                   // 冷却条
    public Camera mainCamera;                       // 相机，用于获取鼠标点击位置
    public Image aimImage;                          // 瞄准图片

    protected Image buttonImage;                    // 按钮图片
    protected float remainReleaseTime = 0f;         // 距离下一次可以释放技能的时间，为0就是可以释放技能
    protected bool isReady = false;                 // 准备释放技能（第一次点击）
    protected Vector3 inputHitPos;                  // 点击的位置
    protected GameObject inputHitGameObject;        // 点击的物体

    private bool gamePlaying = false;               // 是否正在游戏中
    private bool isOnButton = false;                // 鼠标是否位于按钮上
    private Color normalColor = Color.white;                    // 默认颜色
    private Color hightLightColor = new Color(1, 1, 0.45f);     // 高亮颜色
    private Color pressedColor = new Color(1, 0.27f, 0.27f);      // 点击颜色
    private Color disableColor = new Color(0.46f, 0.46f, 0.46f);  // 失效颜色

    /// <summary>
    /// 初始化滑动条最大值
    /// </summary>
    private void Awake()
    {
        coolDownSlider.maxValue = coolDownTime;
        remainReleaseTime = coolDownTime;
        buttonImage = GetComponent<Image>();
    }

    #region 更新冷却时间、检查是否释放技能

    /// <summary>
    /// 更新冷却时间、鼠标位置、鼠标点击时释放技能
    /// </summary>
    private void FixedUpdate()
    {
        if (!GameRoundPlaying())        //游戏没开始就直接return
            return;
        UpdateCoolDown();               //更新冷却时间
        if (CanRelease())               //点了技能按钮，并且有效，可以释放
        {
            RaycastObject(Input.mousePosition);                             //根据鼠标射线对象
            //成功更新准心位置 且 满足自定义释放条件，且按钮鼠标左键，释放技能
            if (UpdateAimPosition() &&ReleaseCondition() && Input.GetMouseButton(0))              
                Release();
        }
    }

    /// <summary>
    /// 根据游戏是否开始，来设置技能能否使用，返回是否可以使用（是否正在游戏）
    /// </summary>
    /// <returns></returns>
    private bool GameRoundPlaying()
    {
        if (GameRecord.instance.CurrentGameState == GameState.Playing)
            return SetSkillEnableByGamePlaying(true);
        return SetSkillEnableByGamePlaying(false);
    }

    /// <summary>
    /// 只有当gamePlaying 和 isPlaying 不同时才会改变技能是否有效。即只有从 开始到进行触发一次，进行到结束触发一次
    /// </summary>
    /// <param name="isPlaying">是否正在游戏</param>
    /// <returns></returns>
    private bool SetSkillEnableByGamePlaying(bool isPlaying)
    {
        if (gamePlaying != isPlaying)
        {
            gamePlaying = isPlaying;
            SetSkillEnable(isPlaying);
        }
        return isPlaying;
    }

    /// <summary>
    /// 点击更新点击位置以及点击物体
    /// </summary>
    /// <param name="screenPos">点击屏幕位置</param>
    private void RaycastObject(Vector2 screenPos)
    {
        RaycastHit info;
        if (Physics.Raycast(mainCamera.ScreenPointToRay(screenPos), out info, 200))
        {
            inputHitPos = info.point;
            inputHitGameObject = info.collider.gameObject;
            return;
        }
        inputHitGameObject = null;
    }

    /// <summary>
    /// 更新准心位置
    /// </summary>
    private bool UpdateAimPosition()
    {
        if (inputHitPos == null)
        {
            Debug.LogError("Raycast Didn't Hit,And None 'inputHitPos'.");
            return false;
        }
        aimImage.gameObject.transform.position = new Vector3(inputHitPos.x, inputHitPos.y + 0.5f, inputHitPos.z);
        return true;
    }

    /// <summary>
    /// 更新冷却时间，通过Time.fixedDeltaTime改变。同时改变按钮颜色和滑动条长度
    /// </summary>
    private void UpdateCoolDown()
    {
        coolDownSlider.value = Mathf.Lerp(coolDownSlider.maxValue, coolDownSlider.minValue, remainReleaseTime / (coolDownSlider.maxValue - coolDownSlider.minValue));
        if (remainReleaseTime == 0)
            return;
        if (remainReleaseTime < 0)
        {
            buttonImage.color = normalColor;
            remainReleaseTime = 0;
            return;
        }
        remainReleaseTime -= Time.fixedDeltaTime;
        buttonImage.color = disableColor;
    }

    #endregion

    #region 鼠标点击、进入、离开，准备状态、取消准备

    /// <summary>
    /// 点击技能时响应
    /// </summary>
    public void OnClicked()
    {
        if (remainReleaseTime > 0)
            return;
        isReady = !isReady;
        if (isReady)
            Ready();
        else
            Cancel();
    }

    /// <summary>
    /// 鼠标移动到按钮或移出按钮时响应
    /// </summary>
    /// <param name="enter">true进入，否则是移出</param>
    public void OnMouseOnButton(bool enter)
    {
        isOnButton = enter;
        if (remainReleaseTime > 0)
            return;
        if (isReady)
        {
            aimImage.gameObject.SetActive(!enter);
            return;
        }
        if (enter)
            buttonImage.color = hightLightColor;
        else
            buttonImage.color = normalColor;
    }

    /// <summary>
    /// 准备释放技能状态
    /// </summary>
    public void Ready()
    {
        aimImage.gameObject.SetActive(true);
        aimImage.gameObject.transform.position = Input.mousePosition;
        buttonImage.color = pressedColor;
    }

    /// <summary>
    /// 取消准备释放技能状态
    /// </summary>
    public void Cancel()
    {
        aimImage.gameObject.SetActive(false);
        buttonImage.color = normalColor;
    }

    #endregion

    /// <summary>
    /// 设置技能能否使用
    /// </summary>
    /// <param name="enable">是否激活技能</param>
    /// <returns>返回是否激活</returns>
    public bool SetSkillEnable(bool enable)
    {
        if (enable)
        {
            if (isOnButton)
                buttonImage.color = hightLightColor;
            else
                buttonImage.color = normalColor;
        }
        else
        {
            coolDownSlider.value = coolDownSlider.minValue;
            remainReleaseTime = coolDownTime;
            buttonImage.color = disableColor;
            if (isReady)
            {
                isReady = false;
                aimImage.gameObject.SetActive(false);
            }
        }
        return enabled;
    }

    /// <summary>
    /// 是否可释放技能
    /// </summary>
    /// <returns>返回True，可以释放技能</returns>
    public bool CanRelease()
    {
        return remainReleaseTime <= 0f && isReady && !isOnButton;
    }

    /// <summary>
    /// 释放技能
    /// </summary>
    public void Release()
    {
        remainReleaseTime = coolDownSlider.maxValue;
        isReady = false;
        Cancel();
        StartCoroutine(SkillEffect());
    }

    /// <summary>
    /// 自定义条件判断是否释放技能
    /// </summary>
    /// <returns>返回是否满足自定义条件</returns>
    virtual public bool ReleaseCondition()
    { return true; }

    /// <summary>
    /// 技能效果
    /// </summary>
    abstract public IEnumerator SkillEffect();
}
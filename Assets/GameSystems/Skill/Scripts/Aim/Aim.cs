﻿using UnityEngine;
using UnityEngine.UI;

namespace GameSystem.Skill
{
    public class Aim : MonoBehaviour
    {
        public AimMode aimMode;                         // 当前瞄准模型
        public Image groundAimImage;                    // 显示在世界区域的瞄准图片

        public Vector3 HitPosition { get { return inputHitPos; } }                  //获取指中目标位置
        public GameObject HitGameObject { get { return inputHitGameObject; } }      //获取指中对象

        protected bool aimEnable = true;                // 瞄准是否有效

        private Image aimImage;                         // 当前瞄准图片
        private Camera gameCamera;                      // 游戏镜头
        private Vector3 inputHitPos;                    // 鼠标射线射到的点
        private GameObject inputHitGameObject;          // 射线射到的物体
        private TagWithColor tagWithColor;              // 射到物体的标签对应瞄准模型

        /// <summary>
        /// 获取图片组件
        /// </summary>
        private void Awake()
        {
            aimImage = GetComponent<Image>();
            gameCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
            aimImage.color = aimMode.normalColor;
            SetAimMode(aimMode);
        }

        /// <summary>
        /// 更新瞄准获取的对象值
        /// </summary>
        private void Update()
        {
            SetPosition(Input.mousePosition);
            RaycastObject();
            UpdateAimColor();
        }

        /// <summary>
        /// 以鼠标位置从屏幕射线
        /// </summary>
        private void RaycastObject()
        {
            RaycastHit info;
            if (Physics.Raycast(gameCamera.ScreenPointToRay(gameObject.transform.position), out info, 200, LayerMask.GetMask("Level")))
            {
                inputHitPos = info.point;
                inputHitGameObject = info.collider.gameObject;
            }
        }

        /// <summary>
        /// 更新状态
        /// </summary>
        private void UpdateAimColor()
        {
            if (aimEnable == false)                                 // 如果失效，设置为失效颜色
            {
                aimImage.color = aimMode.disableColor;
                return;
            }
            tagWithColor = aimMode.GetTagWithColorByTag(inputHitGameObject.tag); // 如果模型有定义该标签的颜色，修改之
            if (tagWithColor != null)
            {
                aimImage.color = tagWithColor.color;
                return;
            }
            aimImage.color = aimMode.normalColor;                   // 都没有就改成默认颜色
        }

        /// <summary>
        /// 设置当前瞄准模式
        /// </summary>
        /// <param name="aimMode">瞄准模式</param>
        public void SetAimMode(AimMode aimMode)
        {
            this.aimMode = aimMode;
            aimImage.rectTransform.sizeDelta = Vector2.one * aimMode.screenSpriteRadius * 2;
            aimImage.enabled = aimMode.showInScreen;
            aimImage.sprite = aimMode.screenSprite;
            groundAimImage.rectTransform.sizeDelta = Vector2.one * aimMode.groundSpriteRadius * 2;
            groundAimImage.enabled = aimMode.showInGround;
            groundAimImage.sprite = aimMode.groundSprite;
        }

        /// <summary>
        /// 设置对象激活状态
        /// </summary>
        /// <param name="active">是否激活</param>
        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
            groundAimImage.gameObject.SetActive(active);
        }

        /// <summary>
        /// 设置瞄准图片是否有效
        /// </summary>
        /// <param name="enable">是否有效</param>
        public void SetEnable(bool enable)
        {
            aimEnable = enable;
        }

        /// <summary>
        /// 设置瞄准图片位置
        /// </summary>
        /// <param name="position">位置</param>
        public void SetPosition(Vector3 position)
        {
            gameObject.transform.position = position;
            groundAimImage.transform.position = inputHitPos + Vector3.up * 0.5f;
        }
    }
}

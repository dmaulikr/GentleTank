﻿using Item.Ammo;
using System.Collections;
using UnityEngine;

namespace Item.Tank
{
    public class TankAttackBoxing : TankAttack
    {
        public SpringManager springManager;     // 弹簧控制
        public BoxingAmmo boxingGlove;          // 弹簧拳组件
        public Collider boxingCollider;         // 拳套
        public AnimationCurve launchDistance = AnimationCurve.Linear(0, 0, 0.4f, 1);    // 发射距离比例,来回对称
        public float launchTotalTime = 0.8f;    // 总共发射来回时间

        private float launchElapsed;            // 发射后经过的时间

        protected new void OnEnable()
        {
            base.OnEnable();
            springManager.fillAmount = 0;
        }

        protected new void OnDisable()
        {
            base.OnDisable();
            springManager.fillAmount = 0;
        }

        /// <summary>
        /// 攻击实际效果
        /// </summary>
        protected override void OnAttack(params object[] values)
        {
            if (values == null || values.Length == 0)
                Launch(forceSlider.value, damage, coolDownTime);
            else if (values.Length == 3)
                Launch((float)values[0], (float)values[1], (float)values[2]);
        }

        /// <summary>
        /// 发射炮弹，自定义参数变量
        /// </summary>
        /// <param name="launchForce">发射力度</param>
        /// <param name="fireDamage">伤害值</param>
        /// <param name="coolDownTime">发射后冷却时间</param>
        private void Launch(float launchForce, float fireDamage, float coolDownTime)
        {
            springManager.maxDistance = launchForce / 10f;
            boxingGlove.damage = fireDamage;
            boxingCollider.enabled = true;

            StartCoroutine(LaunchBoxingGlove());

            cdTimer.Reset(coolDownTime);
        }

        /// <summary>
        /// 发射弹簧拳协程
        /// </summary>
        /// <returns></returns>
        private IEnumerator LaunchBoxingGlove()
        {
            launchElapsed = 0f;
            boxingGlove.needTurnBack = false;
            while (launchElapsed < launchTotalTime && launchElapsed >= 0f)
            {
                if (!boxingGlove.needTurnBack && launchElapsed < launchTotalTime / 2f)
                {
                    springManager.fillAmount = launchDistance.Evaluate(launchElapsed);
                    launchElapsed += Time.deltaTime;
                }
                else
                {
                    if (boxingGlove.needTurnBack)
                    {
                        launchElapsed -= Time.deltaTime;
                        springManager.fillAmount = launchDistance.Evaluate(launchElapsed);
                    }
                    else
                    {
                        launchElapsed += Time.deltaTime;
                        springManager.fillAmount = launchDistance.Evaluate(launchTotalTime - launchElapsed);
                    }
                    boxingCollider.enabled = false;
                }

                yield return null;
            }
            springManager.fillAmount = 0f;
            boxingCollider.enabled = false;
        }
    }
}
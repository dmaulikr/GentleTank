﻿using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 用于单个对象的信息列表存储
/// </summary>
public sealed class ObjectPreferences<T>
{
    private Dictionary<string, T> preferences = new Dictionary<string, T>();        // 信息列表
    public Dictionary<string, T> Preferneces { get { return preferences; } }

    public T this[string key]                              // 信息索引器
    {
        get { return GetValue(key); }
        set { AddOrModifyValue(key, (T)value); }
    }

    /// <summary>
    /// 清除列表所有信息
    /// </summary>
    public void Clear()
    {
        preferences.Clear();
    }

    /// <summary>
    /// 移除键值
    /// </summary>
    /// <param name="key">需要移除信息的键</param>
    public void Remove(string key)
    {
        if (preferences.ContainsKey(key))
            preferences.Remove(key);
    }

    /// <summary>
    /// 是否包含该键值
    /// </summary>
    /// <param name="key">键</param>
    /// <returns>是否包含该对象键值</returns>
    public bool Contains(string key)
    {
        return preferences.ContainsKey(key);
    }

    /// <summary>
    /// 添加键值
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void AddValue(string key, T value)
    {
        if (!preferences.ContainsKey(key))
            preferences.Add(key, value);
    }

    /// <summary>
    /// 添加键值，如果已经存在，覆盖掉
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="value">对象</param>
    public void AddOrModifyValue(string key, T value)
    {
        if (preferences.ContainsKey(key))
            preferences[key] = value;
        else
            preferences.Add(key, value);
    }

    /// <summary>
    /// 获取键值，不存在返回null
    /// </summary>
    /// <param name="key">获取对象的键</param>
    /// <returns>返回对应对象，不存在为null</returns>
    public T GetValue(string key)
    {
        return preferences.ContainsKey(key) ? preferences[key] : default(T);
    }

    /// <summary>
    /// 获取键值，如果不存在，创建之
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="value">如果存在，可以无视。如果不存在，则作为初始值</param>
    /// <returns></returns>
    public T GetOrAddValue(string key, T value)
    {
        if (!preferences.ContainsKey(key))
            preferences.Add(key, value);
        return preferences[key];
    }

}

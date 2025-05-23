﻿using System;
using System.Collections.Generic;
using UnityEngine;


public static class ObjectExtension
{
    private static readonly List<Transform> CachedTransforms = new List<Transform>();

    public static T GetOrAddComponent<T>(this Component obj) where T : Component
    {
        return obj.gameObject.GetOrAddComponent<T>();
    }

    public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
    {
        T t = gameObject.GetComponent<T>();
        if (t == null)
            t = gameObject.AddComponent<T>();
        return t;
    }

    public static Component GetOrAddComponent(this Component obj, Type type)
    {
        return obj.gameObject.GetOrAddComponent(type);
    }

    public static Component GetOrAddComponent(this GameObject gameObject, Type type)
    {
        if (gameObject == null) return null;

        Component component = gameObject.GetComponent(type);
        if (component == null)
            component = gameObject.AddComponent(type);
        return component;
    }

    public static void SetParentEx(this Transform transform, Transform parent)
    {
        transform.SetParent(parent);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }

    public static void SetLayerRecursively(this GameObject gameObject, int layer)
    {
        gameObject.GetComponentsInChildren(true, CachedTransforms);
        for (int i = 0; i < CachedTransforms.Count; i++)
        {
            CachedTransforms[i].gameObject.layer = layer;
        }

        CachedTransforms.Clear();
    }

    public static void AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue value)
    {
        if (dict.ContainsKey(key))
            dict[key] = value;
        else
            dict.Add(key, value);
    }

    public static void SetActiveEx<T>(this T obj, bool value) where T : Component
    {
        if (obj)
            obj.gameObject.SetActiveEx(value);
    }

    public static void SetActiveEx(this GameObject obj, bool value)
    {
        if (obj && obj.activeSelf != value)
            obj.SetActive(value);
    }
}

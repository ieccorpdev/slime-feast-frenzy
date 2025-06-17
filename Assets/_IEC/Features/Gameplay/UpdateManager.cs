using System;
using System.Collections.Generic;
using _IEC.Script.GamePlay;
using UnityEngine;

public class UpdateManager : MonoBehaviour
{
    private static UpdateManager _instance;
    public static UpdateManager Instance => _instance;

    private readonly List<IManagedUpdate> managedUpdates = new List<IManagedUpdate>();
    private readonly List<IManagedFixedUpdate> managedFixedUpdates = new List<IManagedFixedUpdate>();
    private readonly List<IManagedLateUpdate> managedLateUpdates = new List<IManagedLateUpdate>();

    void Awake()
    {
        if (_instance == null)
            _instance = this;
        else
            Destroy(gameObject);
    }

    void Update()
    {
        for (int i = 0; i < managedUpdates.Count; i++)
            managedUpdates[i].ManagedUpdate();
    }

    void FixedUpdate()
    {
        var count = managedFixedUpdates.Count;
        for (int i = 0; i < count; i++)
            managedFixedUpdates[i].ManagedFixedUpdate();
    }

    private void LateUpdate()
    {
        int count = managedLateUpdates.Count;
        for (int i = 0; i < count; i++)
            managedLateUpdates[i].ManagedLateUpdate();
    }

    // Đăng ký và huỷ đăng ký Update
    public void RegisterUpdate(IManagedUpdate obj)
    {
        if (!managedUpdates.Contains(obj))
            managedUpdates.Add(obj);
    }

    public void UnregisterUpdate(IManagedUpdate obj)
    {
        managedUpdates.Remove(obj);
    }
    // Đăng ký và huỷ đăng ký Late Update
    public void RegisterLateUpdate(IManagedLateUpdate obj)
    {
        if (!managedLateUpdates.Contains(obj))
            managedLateUpdates.Add(obj);
    }

    public void UnregisterLateUpdate(IManagedLateUpdate obj)
    {
        managedLateUpdates.Remove(obj);
    }
    // Đăng ký và huỷ đăng ký FixedUpdate
    public void RegisterFixedUpdate(IManagedFixedUpdate obj)
    {
        if (!managedFixedUpdates.Contains(obj))
            managedFixedUpdates.Add(obj);
    }

    public void UnregisterFixedUpdate(IManagedFixedUpdate obj)
    {
        managedFixedUpdates.Remove(obj);
    }
}
using System;
using System.Collections.Generic;
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
        for (int i = 0; i < managedFixedUpdates.Count; i++)
            managedFixedUpdates[i].ManagedFixedUpdate();
    }

    private void LateUpdate()
    {
        for (int i = 0; i < managedLateUpdates.Count; i++)
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
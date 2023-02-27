using System;
using System.Collections.Generic;
using UnityEngine;

public class ServiceLocatorObject<ServiceLocatorType>
{
    private readonly Dictionary<string, ServiceLocatorType> services = new Dictionary<string, ServiceLocatorType>();

    public ServiceLocatorObject() { }

    public bool TryGetService<T>(out T service) where T : ServiceLocatorType
    {
        string key = typeof(T).Name;
        
        if (!services.ContainsKey(key))
        {
            service = default;
            return false;
        }

        service = (T)services[key];
        return true;
    }

    public T Get<T>() where T : ServiceLocatorType
    {
        string key = typeof(T).Name;
        if (!services.ContainsKey(key))
        {
            Debug.LogError($"{key} not registered with {GetType().Name}");
            throw new InvalidOperationException();
        }

        return (T)services[key];
    }

    public void Register<T>(T service) where T : ServiceLocatorType
    {
        string key = typeof(T).Name;
        if (services.ContainsKey(key))
        {
            Debug.LogError($"Attempted to register service of type {key} which is already registered with the {GetType().Name}.");
            return;
        }

        services.Add(key, service);
    }

    public void Unregister<T>() where T : ServiceLocatorType
    {
        string key = typeof(T).Name;
        if (!services.ContainsKey(key))
        {
            Debug.LogError($"Attempted to unregister service of type {key} which is not registered with the {GetType().Name}.");
            return;
        }

        services.Remove(key);
    }

    public void Exchange<T>(T service) where T : ServiceLocatorType
    {
        string oldKey = typeof(T).Name;
        if (services.ContainsKey(oldKey))
        {
            services.Remove(oldKey);
        }

        string newKey = typeof(T).Name;
        if (!services.ContainsKey(newKey))
        {
            services.Add(newKey, service);
        }
    }
}

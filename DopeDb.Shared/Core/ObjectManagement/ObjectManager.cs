using System;
using System.Collections.Generic;

namespace DopeDb.Shared.Core.ObjectManagement
{
    public class ObjectManager
    {
        private Di di;

        protected static ObjectManager instance;

        protected Dictionary<Type, object> instances;

        public static ObjectManager Instance()
        {
            if (ObjectManager.instance == null)
            {
                ObjectManager.instance = new ObjectManager();
            }
            return ObjectManager.instance;
        }

        public ObjectManager()
        {
            di = new Di(this);
            instances = new Dictionary<Type, object>();
        }

        public object Get(Type type)
        {
            if (type.IsPrimitive)
            {
                throw new ObjectManagementException($"Cannot create instances of primitive types");
            }
            var isSingleton = false;
            if (type.IsInterface)
            {
                if (instances.ContainsKey(type))
                {
                    return instances[type];
                }
                else
                {
                    throw new ObjectManagementException($"No instance defined for interface {type}");
                }
            }
            if (type.GetCustomAttributes(typeof(Singleton), false).Length > 0)
            {
                isSingleton = true;
                if (instances.ContainsKey(type))
                {
                    return instances[type];
                }
            }
            try
            {
                var instance = di.Create(type);
                if (isSingleton)
                {
                    Set(type, instance);
                }
                return instance;
            }
            catch (System.Exception e)
            {
                throw new ObjectManagementException($"Could not get object of type {type}", e);
            }
        }

        public void Set(Type type, object instance)
        {
            this.instances.Add(type, instance);
        }

        public T Get<T>()
        {
            return (T)Get(typeof(T));
        }
    }
}
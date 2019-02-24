using System.Reflection;
using System.Collections.Generic;
using System;

namespace DopeDb.Shared.Core.ObjectManagement
{
    class Di
    {

        protected ObjectManager objectManager;

        public Di(ObjectManager objectManager)
        {
            this.objectManager = objectManager;
        }

        public object Create(Type targetType)
        {
            if (targetType.IsAbstract)
            {
                throw new ObjectManagementException($"Given type {targetType} is abstract");
            }
            var constructor = targetType.GetConstructors()[0];
            var arguments = PrepareMethodArguments(constructor);
            var instance = constructor.Invoke(arguments);
            InjectProperties(instance);
            return instance;
        }

        protected object[] PrepareMethodArguments(MethodBase method)
        {
            var result = new List<object>();
            foreach (var parameter in method.GetParameters())
            {
                var targetInstance = objectManager.Get(parameter.ParameterType);
                result.Add(targetInstance);
            }
            return result.ToArray();
        }

        protected void InjectProperties(object instance)
        {
            InjectFields(instance);
            injectInjectionMethods(instance);
            var injectionCompleteCallback = instance.GetType().GetMethod("InjectionComplete", BindingFlags.NonPublic | BindingFlags.Instance);
            if (injectionCompleteCallback != null)
            {
                injectionCompleteCallback.Invoke(instance, null);
            }
        }

        protected void injectInjectionMethods(object instance)
        {
            var methodInfos = instance.GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var methodInfo in methodInfos)
            {
                if (methodInfo.Name.Length < 7 || !methodInfo.Name.StartsWith("Inject"))
                {
                    continue;
                }
                if (methodInfo.Name.StartsWith("Injected") || methodInfo.Name.StartsWith("Injection"))
                {
                    continue;
                }
                var arguments = PrepareMethodArguments(methodInfo);
                methodInfo.Invoke(instance, arguments);
            }
        }

        protected void InjectFields(object instance)
        {
            var instanceType = instance.GetType();
            var fieldInfos = instanceType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var fieldInfo in fieldInfos)
            {
                try
                {
                    var injectAttribute = fieldInfo.GetCustomAttribute(typeof(Inject));
                    if (injectAttribute == null)
                    {
                        continue;
                    }
                }
                catch (System.Exception)
                {
                    continue;
                }
                var targetInstance = this.objectManager.Get(fieldInfo.FieldType);
                fieldInfo.SetValue(instance, targetInstance);
                var injectedCallback = instanceType.GetMethod("Injected" + Util.Text.FirstCharToUpper(fieldInfo.Name), BindingFlags.NonPublic | BindingFlags.Instance);
                if (injectedCallback == null)
                {
                    continue;
                }
                injectedCallback.Invoke(instance, null);
            }
        }
    }
}
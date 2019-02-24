# ObjectManagement
Rather than creating objects you can let them be created from the object manager.
It is a singleton so you would use e.g.
```c#
using DopeDb.Shared.Configuration;
using DopeDb.Shared.Core.ObjectManagement;

var configurationManager = ObjectManager.Instance().Get<ConfigurationManager>();
```

## Dependency Injection
When retrieving an instance from the object manager, it will be created (if necessary) with all (non-primitive) arguments instantiated recursively as constructor arguments.
Note that it is not possible to create an object with primitive parameters this way.
However there are other ways to inject properties into automatically created objects:

### Method Injection
All non-public methods called `Inject*` will be handled like the constructor in that they will receive all necessary (non-primitive) arguments.
Keep in mind that injecting properties into both methods and fields requires an instance, so you will not be able to use these injected properties in your constructor. In fact, the `Inject*` method can end in anything (but the other life-cycle method names) and can handle multiple parameters.

### Field Injection
Fields annotated with the attribute `DopeDb.Shared.Core.ObjectManagement.Inject` will receive their properties injected by DI.

### Example
```c#
using DopeDb.Shared.Core.ObjectManagement;
using DopeDb.Shared.Configuration;
using DopeDb.Shared.Plugins;

class Example {
    [Inject]
    protected ConfigurationManager configurationManager;

    protected PluginManager pluginManager;

    protected void InjectPluginManager(PluginManager pluginManager) {
        this.pluginManager = pluginManager;
    }

    protected void InjectedPluginManager() {
        System.Console.WriteLine("this.configurationManager is now ready");
    }

    protected void InjectionComplete() {
        System.Console.WriteLine("All injected properties are now available");
    }
}
```

### Lifecycle
The example above illustrates the life-cycle methods `Injected<property>` and `InjectionComplete` which are called after a property has been injected and after all properties are present respectively.

## Instances
### Singletons
Classes may be annotated with the attribute `DopeDb.Shared.Core.ObjectManagement.Singleton`. This results in the object manager always returning the same instance.

### Interfaces
Interfaces can only be injected after the `ObjectManager` gets to know the instance to be returned. Similarly to singletons, you can register instances for types like
```c#
ObjectManager.Instance().Set(typeof(IExample), ObjectManager.Instance().Get<Example>());
```
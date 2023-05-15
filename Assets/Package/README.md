Dependency injection framework for Unity based on Autofac.

**This guide assumes familiarity with dependency injection and Autofac**, see [its documentation](https://docs.autofac.org/) if you don't know how it works.

# Table of Contents

[[_TOC_]]

# Installation

## 1. Package

Add this scoped registry.

```json
"scopedRegistries": [
    {
      "name": "Mr. Watts UPM Registry",
      "url": "https://gitlab.com/api/v4/projects/27157125/packages/npm/",
      "scopes": [
        "io.mrwatts"
      ]
    }
  ]
```

Add the following dependency in the **manifest.json** file in the "Packages" folder.

_"io.mrwatts.fuelinject": "1.0.0"_

The version number should be the latest version of the package (unless you want to target an older version on purpose).

## 2. NuGet Packages

**This package depends on external NuGet packages.** To use it, you must also install the [Autofac NuGet package](https://www.nuget.org/packages/Autofac) at **version ^6.4**.

It is recommended to use a tool such as [NuGetForUnity](https://github.com/GlitchEnzo/NuGetForUnity) to install this package and automatically resolve its dependencies.

## 3. Assembly References

Add Assembly reference to `mrwatts.fuelinject.runtime` to your application's assembly definitions (e.g. `Scripts.asmdef`) you want to use this package in. It is not auto-referenced.

# Setup

## 1. Application Module

Create a script containing the code for your application module. This is where you will configure your bindings, Autofac submodules, and attach existing scene objects to the container so they can be injected into other services:

```csharp
namespace Application
{
    public sealed class YourApplicationModule : MonoBehaviour, IUnityContainerModule
    {
        [SerializeField]
        private SomeExistingMonoBehaviour foo = default!;

        public void Configure(ContainerBuilder builder)
        {
            // Register your bindings, submodules, and existing objects from the scene here, for example:
            builder.RegisterInstance(foo).AsImplementedInterfaces();
            builder.RegisterType<Bar>().AsSelf().As<IBar>().SingleInstance();
        }
    }
}
```

Afterwards, add this component to a `GameObject` of your choice in your scene.

## 2. Container Module Loader

The container module loader is a shim between Autofac and Unity that handles loading all modules, building the final container, and injecting dependencies into `GameObject`s and `Component`s in (just) the active scene.

Add the `ContainerModuleLoader` Unity component to a `GameObject` of your choice in the scene. You have two choices when configuring it.

1. Enable `Automatically Add Root Game Object Modules`.
2. Configure `Startup Modules`.

> The two can be combined in theory, but this is not supported in any way.

### Automatically Add Root Game Object Modules

This will automatically scan (only) the root `GameObject`s all open scenes for components that are an (Autofac) `IModule` or an `IUnityContainerModule` and loads them into (a single) container.

**This is the recommended choice if you want to use automated tests later on**, as the utilities provided depend on it.

### Use Startup Modules

Explicitly add your module from step 1 to the list of startup modules. All modules from this list will be registered and loaded into (a single) container.

## 3. Create Your Services

After that, you can use Autofac as you would otherwise. `MonoBehaviour`s cannot have custom constructors, so they must resort to using injection by property, for example:

```cs
namespace Application
{
    public sealed class SomeMonoBehaviourDependency : MonoBehaviour
    {
        [Inject]
        private Bar Bar { get; set; } = default!;

        public void Start()
        {
            // Code that uses Bar.
        }
    }
}
```

# Service Interfaces

Fuel Inject exposes some additional interfaces you can implement that fit well with how Unity applications work. Each has its own subchapter below.

Note that you are responsible for registering (binding) these appropriately in your container module.

## `IInitializable`

Implement `IInitializable` if your service needs to do something on startup.

All initializables are executed sequentially by a `MonoBehaviour` after the container is fully built.

## `IAsyncInitializable`

Implement `IAsyncInitializable` if your service needs to do something on startup, and that task is asynchronous.

All asynchronous initializables are executed _sequentially_ (i.e. not in parallel) by a `MonoBehaviour` after the container is fully built, _after_ all synchronous initializables are done initializing.

## `ITickable`

Implement `ITickable` if your service needs to do something every frame. It is equivalent to `MonoBehaviour.Update`.

## `IFixedTickable`

Implement `ITickable` if your service needs to do something every physics system frame. It is equivalent to `MonoBehaviour.FixedUpdate`.

## `ILateTickable`

Implement `ILateTickable` if your service needs to do something every frame, after all `ITickable`s have finished executing. It is equivalent to `MonoBehaviour.LateUpdate`.

## `IDisposable`

Implement `IDisposable` if your service needs to do something as the scene is unloaded. It is equivalent to `MonoBehaviour.OnDestroy`.

## `ITerminatable`

Implement `ITerminatable` if your service needs to do something as the application exits. It is equivalent to `MonoBehaviour.OnApplicationQuit`.

# Ordering

When injecting `IEnumerable<T>` in your services, ordering is sometimes relevant. For example, you might want to have one `IInitializable` execute before another. This can be achieved by using ordering:

```cs
// Execute initializer as earlier than others.
builder.RegisterType<Foo>().As<IInitializable>().WithOrder(-1).SingleInstance();

// Execute initializer as later than others.
builder.RegisterType<Foo>().As<IInitializable>().WithOrder(1).SingleInstance();

// Execute initializer in default order (0).
builder.RegisterType<Foo>().As<IInitializable>().SingleInstance();
builder.RegisterType<Foo>().As<IInitializable>().WithOrder(0).SingleInstance();
```

If you are writing a new service that accepts this ordered collection itself, use `IOrderedEnumerable`:

```cs
public sealed class Foo
{
    private readonly IEnumerable<IBar> delegates;

    public CompositeInitializable(IOrderedEnumerable<IBar> delegates)
    {
        this.delegates = delegates;
    }
}
```

Note that execution order for multiple services that have the same ordering is unspecified.

> This is similar to [Autofac.Extras.Ordering](https://github.com/mthamil/Autofac.Extras.Ordering), but does not force you to specify the order for _all_ services, as they might be scattered across modules, where some might not care, and others might want to execute as early or late as possible.

# Runtime Injection

Since the container module loader only resolves dependencies for Unity objects in the scene and services it creates itself, dependencies are not automagically injected into prefabs or Unity objects instantiated at runtime. If you need this, you can inject one of the following dependencies in your class to do so:

-   `IInjector<Scene>` - Injects dependencies into Unity scenes by recursively looping over all `GameObject`s and their components.
-   `IInjector<GameObject>` - Injects dependencies into Unity `GameObject`s by recursively looping over their components and children.
-   `IInjector<object>` - Injects dependencies into arbitrary (even non-Unity) objects.

For example:

```cs
class Foo
{
    private readonly IInjector<GameObject> injector;

    public Foo(IInjector<GameObject> injector)
    {
        this.injector = injector;
    }

    public void Bar()
    {
        GameObject gameObject = createGameObjectSomehow();

        injector.Inject(gameObject);
    }
}
```

## Convenient Runtime Instantiation

As instantiating `GameObject`s by cloning prefabs or other `GameObject`s and injecting afterwards is a common pattern, you can inject `IGameObjectInstantiator` into your class to make this more convenient:

```cs
class Foo
{
    private readonly IGameObjectInjector instantiator;

    public Foo(IGameObjectInjector instantiator)
    {
        this.instantiator = instantiator;
    }

    public void Bar()
    {
        GameObject gameObject = instantiator.Instantiate(somePrefab);

        // gameObject, its components, and their children will have their dependencies injected.
    }
}
```

# Logging

Some basic logging is included so you can follow what is going on.

By default, the Unity console logger is used for logging through `UnityLoggerModule`. If you want to use some other logger, such as Serilog, you can overrule the necessary bindings (see also below).

You can also disable `automaticallyLoadUnityLoggerModule` on `ContainerModuleLoader` if you don't want any logger bindings to be created by default at all. In this case _you become responsible_ for getting the necessary loggers bound to the necessary interfaces.

## Kernel / Asynchronous Exceptions

`IUnityKernelLogger` handles logging for the `UnityKernel`, which is the `MonoBehaviour` that executes your initializables, disposables, and others. Because of the nature of `async` code, such as for `IAsyncInitializable`, exceptions can only propagate by returning a `Task`, which is impossible for `MonoBehaviour`s. To solve this, `UnityKernel` uses `IUnityKernelLogger` to do logging of exceptions, for both synchronous and asynchronous code.

If you want to use some other logger, such as Serilog, you can overrule create your own adapter, for example:

```cs
using System;
using UnityEngine;

/// <summary>
/// Adapter that adpts an Unity ILogger to the IUnityKernelLogger interface.
/// </summary>
public sealed class MyLoggerUnityKernelLoggerAdapter : IUnityKernelLogger
{
    private readonly IMyLogger logger;

    public UnityLoggerUnityKernelLoggerAdapter(IMyLogger logger)
    {
        this.logger = logger;
    }

    public void LogException(Exception exception)
    {
        logger.LogExceptionSomehow(exception);
    }
}
```

## Container Diagnostics

Autofac [supports outputting diagnostics](https://autofac.readthedocs.io/en/latest/advanced/debugging.html#quick-start), so you can follow what the container is doing when resolving services. This can be handy to debug loops or other tough-to-debug issues.

Fuel Inject supports the `enableContainerDiagnostics` setting on `ContainerModuleLoader`, which is a quick and easy way to log the diagnostics of the container to the Unity console.

If you want to log the diagnostics somewhere else, you can either add `ContainerDiagnosticsModule` with your own tracer yourself (and keep `enableContainerDiagnostics` disabled), or add your own build callback, as described on the documentation page above.

# Testing

Follow [the standard procedure](https://docs.unity3d.com/Packages/com.unity.test-framework@1.1) for writing tests in Unity and ensure that you can run basic unit tests before proceeding.

You can also look at [the tests in this repository](https://gitlab.com/mr-watts/internal/packages/unity-fuel-inject/-/tree/master/Assets/Tests/Behavior), which also serve as examples.

**Enable `automaticallyAddRootGameObjectModules` for your `ContainerModuleLoader`** to allow the test utilities to override and inject additional bindings without requiring changes in your application code.

Also, don't forget to add the assembly `mrwatts.fuelinject.testing` to your (test) assembly references to get access to the test utilities.

## Basic Scene Test

A basic scene test that loads a scene and verifies that the window exists looks like this:

```cs
namespace Application
{
    [TestFixture]
    internal sealed class FooWindowTests : SceneTestGroup
    {
        [UnityTest]
        public IEnumerator WindowIsPresentInScene()
        {
            yield return SetupScene("MainScene");

            var window = GameObjectFinder.Get<FooWindow>();

            Assert.NotNull(window);
        }
    }
}
```

## Overriding Container Bindings

Suppose `FooWindow` has an `IBar` dependency injected that it uses to perform some task, and you want to override `IBar` with something else (such as a mock from [FakeItEasy](https://autofac.readthedocs.io)), then you can override the binding to do so:

```cs
namespace Application
{
    [TestFixture]
    internal sealed class FooWindowTests : SceneTestGroup
    {
        [UnityTest]
        public IEnumerator WindowCallsBar()
        {
            yield return SetupScene(
                "MainScene",
                builder =>
                {
                    // window will now receive a CustomBar instead of what was originally bound to IBar.
                    builder.RegisterType<CustomBar>().As<IBar>().SingleInstance();
                }
            );

            var window = GameObjectFinder.Get<FooWindow>();

            window.DoSomething();
        }
    }
}
```

## (Not) Waiting For Initialization

**By default, `SetupScene` will wait for all `I(Async)Initializable`s to finish executing.** This is usually what you want. To turn this off, you can disable it using `SetupScene` or use the overload that takes `SceneSetupParameters`.

## Utilities

### `WaitForAsyncResult`

`WaitForAsyncResult` can be used to transform `Task`s, `ValueTask`s, and other `IAsyncResult`s from .NET into an `IEnumerable` (Unity coroutine) that works with Unity's scene tests to "await" it, for example:

```cs
namespace Application
{
    [TestFixture]
    internal sealed class FooTests
    {
        [UnityTest]
        public IEnumerator WaitsForSomethingAsync()
        {
            Task<bool> asyncTask = FooAsync();

            yield return new WaitForAsyncResult(asyncTask);

            Assert.IsTrue(asyncTask.Result);
        }
    }
}
```

### `WaitTemporarilyUntil`

`WaitTemporarilyUntil` is a variant of Unity's `WaitUntil` that supports passing a timeout, after which the waiting automatically stops:

```cs
namespace Application
{
    [TestFixture]
    internal sealed class FooTests
    {
        [UnityTest]
        public IEnumerator WaitsForSomethingAsync()
        {
            bool somethingWasDone = false;

            yield return new WaitTemporarilyUntil(() => somethingWasDone)); // Waits 10 seconds.
            yield return new WaitTemporarilyUntil(() => somethingWasDone, TimeSpan.FromSeconds(5)); // Waits 5 seconds.

            Assert.IsTrue(somethingWasDone, "Oh no, it either failed to be done, or it timed out");
        }
    }
}
```

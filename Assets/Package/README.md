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
using Autofac;
using UnityEngine;

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

```
using UnityEngine;

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
        GameObject gameObject = Instantiate(somePrefab);

        injector.Inject(gameObject);
    }
}
```

# Testing

Follow [the standard procedure](https://docs.unity3d.com/Packages/com.unity.test-framework@1.1) for writing tests in Unity and ensure that you can run basic unit tests before proceeding.

You can also look at [the tests in this repository](https://gitlab.com/mr-watts/internal/packages/unity-fuel-inject/-/tree/master/Assets/Tests/Behavior), which also serve as examples.

**Enable `automaticallyAddRootGameObjectModules` for your `ContainerModuleLoader`** to allow the test utilities to override and inject additional bindings without requiring changes in your application code.

Also, don't forget to add the assembly `mrwatts.fuelinject.testing` to your (test) assembly references to get access to the test utilities.

## Basic Scene Test

A basic scene test that loads a scene and verifies that the window exists looks like this:

```cs
using System.Collections;
using MrWatts.Internal.FuelInject.Testing;
using MrWatts.Internal.FuelInject.Testing.Utility;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

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
using System.Collections;
using Autofac;
using MrWatts.Internal.FuelInject.Testing;
using MrWatts.Internal.FuelInject.Testing.Utility;
using NUnit.Framework;
using UnityEngine.TestTools;

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
using System.Collections;
using MrWatts.Internal.FuelInject.Testing;
using MrWatts.Internal.FuelInject.Testing.Utility;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

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

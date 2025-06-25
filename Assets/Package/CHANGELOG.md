# Unreleased

-   Add built-in support for container forwarding to share services between scenes and containers.

# 2.0.2

-   Fix build errors in consuming projects around `UnitySetUp` not being found due to `SceneTestGroup` test code.

# 2.0.1

-   By default, wait for disposables to be executed when tearing down tests.
-   Allow retrieving the container in `SceneTestGroup` subclasses for convenience.
-   Fix tests not waiting for initializables with `automaticallyAddRootGameObjectModules` disabled.
-   Fix tests ignoring `containerBindingCallback` when `automaticallyAddRootGameObjectModules` was disabled.

# 2.0.0

-   Update to Autofac 8.3.0.
-   Add `PackageNuGetDependencies.csproj` to aid including projects with NuGet dependencies.

# 1.0.0

-   No changes except documentation changes.

# 0.6.1

-   Fix `IAsyncDisposable`s not being properly awaited on Android devices, resulting in pending tasks being killed.

# 0.6.0

-   `IAsyncTickable` is now supported.
-   `IAsyncDisposable` is now supported on Unity >= 2022.

# 0.5.0

-   `ITerminatable` is now available to allow subscribing to the application exiting.
-   `IGameObjectInstantiator` is now available for clients to use when wanting to clone prefabs or other `GameObject`s and immediately inject dependencies.

# 0.4.1

-   Fix warning always being shown about no `IUnityKernelLogger` being bound when not explicitly overruling it.
-   Add `WaitTemporarilyUntil` utility that automatically times out, to the test assembly to aid in writing tests.

# 0.4.0

-   Rewrite `IInjector` to not take an `IContainer`, so it matches the readme and can be used to inject into dynamically spawned game objects.
-   Container diagnostics can now be easily enabled on the `ContainerModuleLoader`. See also the new _Logging_ section in the README.
-   Exceptions from `IAsyncInitializable` are now properly logged. See also the new _Logging_ section in the README.

# 0.3.1

-   Fix Unity still validating `mrwatts.fuelinject.testing` during builds, causing failures, even when it's not referenced.

# 0.3.0

-   Support ordering by injecting `IOrderedEnumerable<T>` and using `WithOrder`.

# 0.2.1

-   Fix test assembly breaking projects due to missing NUnit (#8).
-   Make script execution order of module loader higher priority so it injects before Awake of most `MonoBehaviour`s by default.

# 0.2.0

-   Internal restructuring.
-   Add proper usage instructions to README (#6).
-   Facilitate testing by adding `SceneTestGroup` (in assembly `mrwatts.fuelinject.testing`) (#4).
-   `IInjector<object>`, `IInjector<GameObject>`, and `IInjector<Scene>` are now available for clients to use when they need to inject dependencies into objects at runtime (e.g. after the scene was already loaded) (#5).

# 0.1.1

-   Fix some meta file GUIDs conflicting with Onion.

# 0.1.0

-   Initial release.

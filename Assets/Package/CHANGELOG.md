# Next

-   Fix warning always being shown about no `IUnityKernelLogger` being bound when not explicitly overruling it.

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

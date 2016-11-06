# ASP.NET Core Modules

ASP.NET Core Modules are self-contained units of an app that you can compose into a new app. Modules include functionality for high-level features like billing, search, identity, commenting, product reviews, blogs, image galleries, etc. Modules provide services, endpoints, views and static resources as a single unit. Modules can be easily packaged, shared and acquired so that developers can take advantage of a rich ecosystem of reusable components when building applications. Modules can depend on and extend other modules to create domain specific ecosystems.

Prebuilt packages available on MyGet (https://www.myget.org/feed/Packages/aspnetcoremodules).

## Building and packaging modules

Modules are just ASP.NET Core projects that have been packaged as a NuGet package.

To use MVC from within a module package you must set the `preserveCompilationContext` setting to `true` in your `project.json` file and then modify the embedded .deps file in the module assembly as follows:

- Dissassemble the module assembly

  `ildasm /out=Module1.il Module1.dll`

- Overwrite the module .deps file with the *published* .deps file for the module. This will filter out any dependencies of type "build".
- Update the .deps file to change any library of type "project" to type "package".
- Update the compilation and runtime paths for the project libraries to the correct paths in their NuGet package (ex `Module1.dll` -> `lib/netcoreapp1.1/Module1.dll`).
- Reassemble the assembly

  `ilasm /resource=Module1.res /dll /out=Module.dll Module1.il`

Module content files can be included in the module package using the NuGet [contentFiles](http://blog.nuget.org/20160126/nuget-contentFiles-demystified.html) feature. Support for `contentFiles` is not available in .NET Core projects, so the module package must be created manually using NuGet.exe.

## Configuring shared services

Modules can optionally define a `ConfigureSharedServices` method in their startup class to share service with other modules.

## Using MVC from a module

You can use MVC as you normally would from any ASP.NET Core app.

To support link generation to endpoints in a module you need to share your module routes as a service. Setup the route sharing service by calling `services.AddMvcWithSharedRoutes()` and then share the route by calling `app.UseMvcWithSharedRoutes()`.

# App_Start Component Package

## Overview
Coordinates application startup, asset bundling, global MVC action filters, routing tables, and ASP.NET Identity / OWIN cookie authentication configuration.

## Interface
- `[BundleConfig.cs:BundleConfig]` `public class BundleConfig`
  - `public static void RegisterBundles(BundleCollection bundles)`
- `[FilterConfig.cs:FilterConfig]` `public class FilterConfig`
  - `public static void RegisterGlobalFilters(GlobalFilterCollection filters)`
- `[IdentityConfig.cs:ApplicationUserManager]` `public class ApplicationUserManager : UserManager<ApplicationUser>`
  - `public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)`
- `[IdentityConfig.cs:ApplicationSignInManager]` `public class ApplicationSignInManager : SignInManager<ApplicationUser, string>`
  - `public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)`
- `[RouteConfig.cs:RouteConfig]` `public class RouteConfig`
  - `public static void RegisterRoutes(RouteCollection routes)`
- `[Startup.Auth.cs:Startup]` `public partial class Startup`
  - `public void ConfigureAuth(IAppBuilder app)`

## Invariants
- `[IdentityConfig.cs:ApplicationUserManager.Create]` Enforces `RequireUniqueEmail = true`, password complexity (minimum length 6, digit, lowercase, uppercase), and 5 failed access attempts lockout.
- `[Startup.Auth.cs:Startup.ConfigureAuth]` Configures OWIN cookie authentication using `CookieName = "LMSPortal_Auth"`, `LoginPath = "/Account/Login"`, `SlidingExpiration = true`, and `ExpireTimeSpan = TimeSpan.FromHours(8)`.
- `[BundleConfig.cs:BundleConfig.RegisterBundles]` Bundles `bootstrap.bundle.js` (including embedded Popper.js) to prevent dropdown click delegation errors.
- `[RouteConfig.cs:RouteConfig.RegisterRoutes]` Maps custom SEO-friendly routes: `course/{slug}` to `Course/Details` and `courses` to `Course/Index`.

## Side Effects
- `[Startup.Auth.cs:Startup.ConfigureAuth]` Binds `ApplicationDbContext`, `ApplicationUserManager`, and `ApplicationSignInManager` instances into the OWIN per-request pipeline.
- `[BundleConfig.cs:BundleConfig.RegisterBundles]` Registers and minifies style and script bundles in the application `BundleTable`.

## Verification
- Run: `& "C:\Program Files\Microsoft Visual Studio\18\Community\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe" "D:\Projects\.NET\LMSPortal\LMSPortal.Tests\bin\Debug\net48\LMSPortal.Tests.dll" --Tests:InfrastructureTests`

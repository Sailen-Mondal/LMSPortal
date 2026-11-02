# Admin Subsystem

## Overview
Coordinates the administrative management subsystem of LMSPortal, providing dedicated routing, governance controllers, and administrative view models under the `/Admin` route prefix.

## Interface
- `[AdminAreaRegistration.cs:AdminAreaRegistration]` `public class AdminAreaRegistration : AreaRegistration`
  - `public override string AreaName => "Admin"`
  - `public override void RegisterArea(AreaRegistrationContext context)`
- [Controllers](./Controllers/README.md) - Administrative MVC controllers enforcing `[Authorize(Roles = "Admin")]`.
- [ViewModels](./ViewModels/README.md) - Strongly-typed view models for administrative forms and KPI metrics.

## Invariants
- `[AdminAreaRegistration.cs:RegisterArea]` Registers route `Admin_default` with pattern `Admin/{controller}/{action}/{id}`, routing to default controller `AdminDashboard` and action `Index`.
- All routes within this area namespace (`LMSPortal.Areas.Admin.Controllers`) are isolated from top-level storefront routes.

## Side Effects
- Mutates the MVC routing table by registering the `Admin` area and its associated routes during application initialization.

## Verification
- Run: `& "C:\Program Files\Microsoft Visual Studio\18\Community\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe" "D:\Projects\.NET\LMSPortal\LMSPortal.Tests\bin\Debug\net48\LMSPortal.Tests.dll" --Tests:AdminAreaRegistration`

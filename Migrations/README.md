# Migrations Component Package

## Overview
Houses Entity Framework Code First database migration configuration, schema versioning history, and automated database bootstrapping routines for LMSPortal.

## Interface
- `[Configuration.cs:Configuration]` `public sealed class LMSPortal.Migrations.Configuration : DbMigrationsConfiguration<LMSPortal.Models.ApplicationDbContext>`
- `[Configuration.cs:Configuration.Configuration]` `public Configuration()`

## Invariants
- `AutomaticMigrationsEnabled = true` is configured to enable automatic schema synchronization during development.
- `AutomaticMigrationDataLossAllowed = true` is enabled for development environments.

## Side Effects
- `[Configuration.cs:Seed]` Invokes `DatabaseSeeder.Seed(context)` which provisions default identity roles (`Admin`, `Student`), administrative user, demo student user, standard curriculum categories, full sample courses with lessons, and initial enrollments in `ApplicationDbContext`.

## Verification
- Run: `& "C:\Program Files\Microsoft Visual Studio\18\Community\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe" "D:\Projects\.NET\LMSPortal\LMSPortal.Tests\bin\Debug\net48\LMSPortal.Tests.dll" --Tests:DatabaseInitialization`

# Security Component Package

## Overview
Provides cryptographic security primitives, credential entropy generation, and security helper utilities for LMSPortal. It guarantees that generated default passwords and administrative credentials satisfy enterprise-grade password complexity.

## Interface
- `[SecurePasswordGenerator.cs:SecurePasswordGenerator]` `public static class LMSPortal.Security.SecurePasswordGenerator`
- `[SecurePasswordGenerator.cs:SecurePasswordGenerator.Generate]` `public static string Generate(int length = 16)`

## Invariants
- `[SecurePasswordGenerator.cs:SecurePasswordGenerator.Generate]` Generated password length is clamped to at least 12 characters (`if length < 12, length = 12`).
- `[SecurePasswordGenerator.cs:SecurePasswordGenerator.Generate]` Password is guaranteed to contain at least one uppercase letter, one lowercase letter, one digit, and one special character from the defined character sets.

## Side Effects
- None

## Verification
- Run: `& "C:\Program Files\Microsoft Visual Studio\18\Community\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe" "D:\Projects\.NET\LMSPortal\LMSPortal.Tests\bin\Debug\net48\LMSPortal.Tests.dll" --Tests:SecurePasswordGenerator`

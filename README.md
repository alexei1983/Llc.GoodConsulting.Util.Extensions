# Llc.GoodConsulting.Util.Extensions

A lightweight, dependency-free collection of **C# extension methods** focused on correctness, security, and modern .NET best practices.

This package provides carefully designed helpers for string comparison, spans, and common low-level operations that are easy to get wrong when re-implemented ad-hoc.

---

## ✨ Features

- 🔐 **Constant-time string & span comparisons** (timing-attack resistant)
- ⚡ **Zero-allocation `ReadOnlySpan<T>` implementations**
- 🧠 Clear, auditable code with explicit intent
- 🧩 Extension-method based API — drop-in usage
- 🚫 No external dependencies
- 🧪 Designed for security-sensitive and performance-critical code paths

---

## 📦 Installation

### .NET CLI

```powershell
dotnet add package Llc.GoodConsulting.Util.Extensions
```

### NuGet Package Manager

```powershell
Install-Package Llc.GoodConsulting.Util.Extensions
```

## 🔐 Constant-Time String Comparison

### Why this matters

Naive string comparisons (`==`, `Equals`) can leak information via timing attacks, where an attacker infers secrets one character at a time based on response duration.

This library provides constant-time comparison utilities suitable for:

- API keys
- Tokens
- Webhook secrets
- HMAC verification
- Authentication and authorization flows

### Example: Safe string comparison

```csharp
using Llc.GoodConsulting.Util.Extensions;

if (providedToken.ConstantTimeEqualsSafe(storedToken))
{
    AllowAccess();
}
```

- Resistant to timing attacks
- Handles different lengths safely
- Null-safe

### Preferred: Zero-allocation `ReadOnlySpan<char>`

```csharp
using Llc.GoodConsulting.Util.Extensions;

if (providedToken.AsSpan().ConstantTimeEquals(storedToken.AsSpan()))
{
    AllowAccess();
}
```

Benefits:

- No allocations
- Works with slices and substrings
- Ideal for high-performance paths

### Header / substring example

```csharp
using Llc.GoodConsulting.Util.Extensions;

ReadOnlySpan<char> token = authorizationHeader.AsSpan(7); // skip "Bearer "
if (token.ConstantTimeEquals(expectedToken.AsSpan()))
{
    AllowRequest();
}
```

## 🧠 Design Philosophy

This library follows a few core principles:

- Correctness over cleverness
- Security is explicit, not accidental
- Span-first APIs
- Small, composable helpers

Every method exists because it solves a real problem encountered in production code.

## 🛡️ Security Notes

These utilities mitigate *algorithm-level timing attacks* and are suitable for *application-level security*.

For raw cryptographic buffers, prefer:

```csharp
using System.Security.Cryptography;

CryptographicOperations.FixedTimeEquals(aBytes, bBytes);
```

## 🎯 Target Frameworks

- .NET (modern runtimes)
- Compatible with C# 8.0+

## 📄 License

MIT License
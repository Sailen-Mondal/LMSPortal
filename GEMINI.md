# Project Guidelines: LMS Portal

## Character Encoding & Mojibake Prevention
- Always ensure `Web.config` maintains `<globalization fileEncoding="utf-8" requestEncoding="utf-8" responseEncoding="utf-8" culture="en-US" uiCulture="en-US" />` in `<system.web>`.
- Never put raw multibyte characters or raw emojis directly in HTML attributes (like `placeholder`). Use clean descriptive text (`"Enter your password"`) or HTML entities (`&#x1F44B;`, `&bull;`).

## Authentication & Logout Flow
- Never rely on `javascript:document.getElementById('logoutForm').submit()` inside Bootstrap dropdown menus. Bootstrap dropdowns intercept link click events and cancel script submission.
- Always use native form submit buttons: `<button type="submit" class="dropdown-item">Sign Out</button>` wrapped inside `@using (Html.BeginForm("LogOff", "Account", FormMethod.Post))`.
- AccountController must support both POST and GET for `LogOff` (with `Logout` alias), call `AuthenticationManager.SignOut(...)`, abandon session, and explicitly clear the auth cookie in `Response.Cookies`.

## Bootstrap 5 Bundling in ASP.NET MVC
- Always bundle `~/Scripts/bootstrap.bundle.js` instead of `~/Scripts/bootstrap.js` in `BundleConfig.cs`.
- `bootstrap.js` does NOT include Popper.js, causing `Uncaught TypeError: Popper.createPopper is not a function`, which breaks dropdown menus (preventing users from clicking user profile menus, logout, or category dropdowns). `bootstrap.bundle.js` includes embedded Popper.

# 🎮 CS2 Auto-Accept Utility v1.0.0

![Status](https://img.shields.io/badge/Status-Stable-brightgreen)
![Platform](https://img.shields.io/badge/Platform-Windows-blue)
![License](https://img.shields.io/badge/License-MIT-orange)

A high-performance, modular utility built in **.NET 8** to automatically accept match invitations in **Counter-Strike 2**. Refactored for clean architecture, ultra-fast scanning, and organic mouse movement.

---

## 🔥 Key Enhancements

- 🏗️ **Clean Architecture**: Decoupled codebase with dedicated layers for Services (Scanning, Input), Core (Settings, Models), and UI.
- ⚡ **LockBits Scanning**: Memory-level pixel analysis that is ~10x faster than traditional methods.
- 📐 **Universal Support**: Native support for **16:9, 16:10, 4:3 (Black bars or Stretched)** and custom resolutions (e.g., **1080x1080**).
- 🖱️ **WindMouse Pathing**: Advanced human-like movement using quadratic Bezier curves, organic jitter, and acceleration/deceleration physics.
- 🖥️ **Resource Monitoring**: Real-time TUI display showing the utility's **~0.5% CPU** and **~28MB RAM** usage.

---

## 📂 Project Structure

- **`src/Core`**: Static configuration and data models.
- **`src/Infrastructure`**: Low-level Win32 API wrappers.
- **`src/Services`**: High-level logic for pixel detection and humanized input.
- **`src/Engine`**: Coordination layer for process monitoring and scan scheduling.
- **`src/UI`**: Modern console management and performance reporting.

---

## 🛠️ Usage

### Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Windows 10/11
- CS2 (Windowed or Fullscreen Borderless recommended)

### Run
```powershell
dotnet run
```

---

## ⚠️ Security Disclaimer

This project is for educational purposes only. While visual-based detection is significantly safer than memory-reading methods, always use it responsibly.

---

*Developed for the Counter-Strike community.*

# HomeSmartLink

![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet&logoColor=white)
![Blazor](https://img.shields.io/badge/Blazor-Server-512BD4?logo=blazor&logoColor=white)
![MudBlazor](https://img.shields.io/badge/MudBlazor-8.0-7e6fff)
![License](https://img.shields.io/badge/License-MIT-green)
![Platform](https://img.shields.io/badge/Platform-Windows%20%7C%20macOS%20%7C%20Linux-lightgrey)

A web dashboard for monitoring and controlling Home SmartLink compatible heating devices. Built with Blazor and MudBlazor.

## Features

- Real-time temperature monitoring for all rooms
- Heating mode control (Eco, Comfort, Program)
- Device status and diagnostics
- EcoWatt integration (French power grid status)
- Schedule programming for heating zones
- Dark/Light theme support

## Prerequisites

**Important:** This application requires an existing Home SmartLink account. You must:

1. Download the official [Home-SmartLink](https://apps.apple.com/fr/app/home-smartlink/id1123038810) iOS app
2. Create an account through the iOS app
3. Link your heating devices via Bluetooth using the iOS app (initial pairing requires Bluetooth)
4. Once devices are linked to your account, you can use this web dashboard to monitor and control them remotely

The web application communicates with the Home SmartLink cloud API. Device pairing and initial setup can only be done through the iOS app due to Bluetooth requirements.

## Supported Brands

- Texas de France
- Carrera
- Cayenne
- Zenith
- KSO

## Tech Stack

- .NET 9 / Blazor
- MudBlazor UI Components
- Clean Architecture (Domain, Application, Infrastructure, Web)

## Getting Started

```bash
# Clone the repository
git clone https://github.com/0xZunia/HomeSmartLink.git

# Navigate to the project
cd HomeSmartLink

# Restore dependencies
dotnet restore

# Run the application
dotnet run --project src/HomeSmartLink.Web/HomeSmartLink.Web
```

## Project Structure

```
src/
  HomeSmartLink.Domain/         # Domain entities and interfaces
  HomeSmartLink.Application/    # Application services and DTOs
  HomeSmartLink.Infrastructure/ # API clients and external services
  HomeSmartLink.Web/            # Blazor web application
  HomeSmartLink.Mobile/         # Mobile app (under construction)
tests/
  HomeSmartLink.Tests/          # Unit tests
```

## iOS App

An iOS version of this application is currently under construction.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Author

Reyan CARLIER - [GitHub](https://github.com/0xZunia)
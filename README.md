# 🎮 Memory Game

A classic memory card matching game built with WPF (Windows Presentation Foundation) using the MVVM (Model-View-ViewModel) architectural pattern.

![.NET Version](https://img.shields.io/badge/.NET-8.0-blue)
![License](https://img.shields.io/badge/License-MIT-green)
![Platform](https://img.shields.io/badge/Platform-Windows-lightgrey)

## 📝 Overview

Memory Game is a single-player card matching game where players need to find pairs of matching cards by flipping them over. The game features multiple categories of cards, customizable board sizes, and a timer to add challenge to the gameplay.

## ✨ Features

-   🐾 Multiple card categories (Animals, Flowers, Fruits)
-   📏 Customizable game board size (standard 4x4 or custom sizes)
-   ⏱️ Timer-based gameplay
-   📊 User statistics tracking
-   💾 Game save/load functionality
-   🔐 User authentication system
-   🎨 Modern WPF interface with smooth animations

## 📁 Project Structure

The project follows the MVVM pattern with the following structure:

-   **Models**: Contains data models like `Card`, `User`, and `Game`
-   **ViewModels**: Contains the business logic and data binding
-   **Views**: Contains the XAML files for the user interface
-   **Services**: Contains various services like `GameService`, `UserService`, and `GameSaveService`
-   **Commands**: Contains command implementations for user actions
-   **Converters**: Contains value converters for data binding
-   **Images**: Contains the game's image resources

## ⚙️ Requirements

-   .NET 8.0 SDK
-   Visual Studio 2022 (recommended) or any IDE that supports .NET development

## 🚀 Setup Instructions

1. Clone the repository
2. Open `MemoryGame.sln` in Visual Studio
3. Build and run the application

## 🎯 How to Play

1. Start the application and log in with your credentials
2. Choose a card category (Animals, Flowers, or Fruits)
3. Select your preferred board size
4. Click on cards to flip them and find matching pairs
5. Try to match all pairs before the timer runs out
6. Your statistics will be saved and can be viewed later

## 📜 Game Rules

-   The game starts with all cards face down
-   Click on a card to flip it
-   Find matching pairs by remembering card positions
-   Each successful match removes the pair from the board
-   The game ends when all pairs are matched or when the timer runs out
-   Your score is based on the time taken to complete the game

## 💻 Development

The project uses:

-   WPF for the user interface
-   MVVM pattern for clean architecture
-   XAML for UI design
-   C# for backend logic
-   .NET 8.0 as the target framework

## 🤝 Contributing

Feel free to submit issues and enhancement requests!

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

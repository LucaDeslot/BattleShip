# Projet BattleShip

## Aperçu

Ce projet BattleShip est une implémentation moderne et interactive du classique jeu de bataille navale. Il utilise les technologies Blazor WebAssembly pour le frontend et ASP.NET Core pour le backend, offrant une expérience utilisateur riche et réactive entièrement basée sur le web. Le jeu permet aux joueurs de placer stratégiquement leurs navires sur une grille et d'affronter l'ordinateur ou un autre joueur en tentant de couler les navires adverses.

## Caractéristiques Principales

- **Interface Utilisateur Intuitive** : Une interface Blazor WebAssembly qui rend le jeu accessible depuis n'importe quel navigateur moderne.
- **Communication en Temps Réel** : Utilisation de gRPC pour une communication rapide et efficace entre le client et le serveur.
- **Validation des Requêtes** : Intégration de FluentValidation pour s'assurer que toutes les requêtes envoyées au serveur sont valides.
- **Persistance des Données** : Un système de leaderboard et d'historique des parties, permettant aux joueurs de suivre leurs progrès.
- **Différents Niveaux de jeux** : Un système de niveau progressif (Augmentation de la taille de la grille au niveau 2, amélioration de l'IA au niveau 3).

## Technologies Utilisées

- **Frontend** : Blazor WebAssembly
- **Backend** : ASP.NET Core
- **Communication Client-Serveur** : gRPC avec support de gRPC-Web pour la compatibilité avec les navigateurs web.
- **Validation** : FluentValidation
- **Stockage Local** : Blazored LocalStorage pour stocker des données localement dans le navigateur de l'utilisateur.

## Configuration et Installation

1. **Prérequis** :
   - .NET 6.0 SDK ou plus récent
   - Un IDE tel que Visual Studio, VS Code ou Rider

2. **Lancement du Serveur Backend** :
   - Naviguez dans le répertoire du projet backend et exécutez `dotnet run`.

3. **Lancement de l'Application Blazor** :
   - Ouvrez le répertoire du projet frontend et exécutez `dotnet run`.

4. **Accédez à l'Application** :
   - Ouvrez votre navigateur et accédez à `https://localhost:9000` pour lancer le jeu.

## Contribution

Ce projet a été développé par :

- **ZUMSTEIN Paulin**
- **DESLOT Luca**
Nous espérons que vous apprécierez jouer à BattleShip autant que nous avons apprécié le développer !

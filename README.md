# Kitchen Chaos
<p align="center">
  <img src="https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/2275820/header.jpg?t=1725190944" />
</p>

## About this Project!
This project is the result of following Code Monkey's incredible free YouTube course series, "How to make a Complete Game in Unity" and its multiplayer follow-up. This was an amazing experience that provided deep insight into the architecture of a complete, polished game, both for single-player and multiplayer. The course focuses heavily on writing clean, scalable, and easy-to-understand code, moving beyond simple beginner tutorials to production-quality practices. Let's dive into the features of our chaotic kitchen!

## Kitchen Chaos's Features
1) Single-Player and Multiplayer Modes: Fully functional gameplay for both solo play and online multiplayer with friends.

2) Complete Gameplay Loop: From receiving orders to chopping, cooking, assembling, and delivering dishes, the game has a full gameplay loop.

3) Full Gamepad Support: Seamlessly switch between keyboard and mouse or a controller with full support for menu navigation and gameplay.

4) Online Multiplayer with Netcode for Game Objects: Built with Unity's official multiplayer solution, allowing for robust and scalable online play.

5) Lobby and Relay System: Players can easily create and join public or private game lobbies. Unity's Relay service allows for easy connection without the need for port forwarding.

6) Character Selection: A dedicated scene where players can join a lobby, see other players, and choose their character color before starting the game.

7) Synchronized Game Logic: All gameplay elements, from player movement and animations to recipe generation and cooking states, are fully synchronized across all clients in a multiplayer session.

## Welcome to Kitchen Chaos! 🍳
In Kitchen Chaos, you're a chef against the clock, either solo or with friends! Orders come in, and you have to chop, cook, assemble, and deliver the correct dishes before time runs out. Grab raw ingredients from container counters, take them to a cutting board to be sliced, or throw them on the stove to cook. But be careful! If you leave food on the stove for too long, it'll burn.

Once you have all your ingredients ready, grab a plate, assemble the dish, and run it to the delivery counter to complete the order!

## What did this project entail?
The creation of a complete game requires a few key systems:
### A Parent Engine & Language
For Kitchen Chaos, we used Unity and C#. By using a powerful engine like Unity, we can leverage its built-in physics, rendering pipeline (URP), and component-based architecture to build our game efficiently. We write all our logic in C#, focusing on clean code principles.

### Character Controller
The player needs to be able to move around the kitchen and interact with the world. We created a character controller from scratch that handles movement input, rotation, and collision detection. This ensures the player can't walk through walls or counters, and smoothly slides along them when moving diagonally.

### Input System
To handle player input for both keyboard and controllers, we used Unity's new Input System. All input logic is contained within a single GameInput class. This class uses C# events to notify other parts of the game when an action (like Interact or Pause) occurs. This decouples the input logic from the game logic, making the code cleaner and easier to maintain.

### Flexible Interaction with Interfaces & Inheritance
To allow the player to interact with many different types of counters, we used a combination of inheritance and interfaces. All counters inherit from a BaseCounter class. To handle placing objects, we defined an IKitchenObjectParent interface. The Player and the BaseCounter both implement this interface, allowing a KitchenObject to be parented to either of them without needing to know the specific type. This makes the code incredibly flexible and easy to expand.

### Data Management with Scriptable Objects
Instead of hard-coding recipes and ingredient properties, we used Scriptable Objects. These are data containers that live in our project files. We created KitchenObjectSO to define each ingredient (its prefab, icon, name) and CuttingRecipeSO / FryingRecipeSO to define how one ingredient transforms into another. This approach makes it incredibly easy to add new ingredients or recipes without changing any code.

### Multiplayer Implementation with Netcode for Game Objects
The multiplayer functionality was built using Unity's Netcode for Game Objects. This involved synchronizing all aspects of the game, including:

1) Player Movement and Animations: Ensuring all players see each other move smoothly and accurately.

2) Game State: Synchronizing the game timer, countdown, and game-over state for all players.

3) Object Spawning and Ownership: Handling the creation and destruction of networked objects, like ingredients and plates, and managing which player has control over them.

4) Lobby and Relay: Using Unity's Lobby service to allow players to create and browse for games, and the Relay service to facilitate easy peer-to-peer connections without requiring players to configure their networks.

And that's it! By combining these systems using clean code principles, we built a complete and polished game from scratch, ready for both single-player and multiplayer fun.

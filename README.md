# Steampunk UI with Photon Engine room connection
The second project of the MYG formation is to make a user login interface to replicate the example image provided with the asset pack.
As part of the complexification of the project, Photon Engine was used to join a room on login and leave it on disconnect, allowing us to see other logged in users.

> [!IMPORTANT]
> You can try the app on my itch.io page [Steampunk UI login](https://babadulnek.itch.io/flowery-journey)


### The requirements of the project
The base requirements of the project is to recreate the example provided using UIToolkit and the provided asset pack.
The UI needs to adapt to different resolutions taking the Samsung S21 as a base resolution.
An error message needs to be displayed if the login information is wrong or incomplete.
Finally unit tests have to be created and validated.
![The example to follow](/Gitassets/example.png)

<br>

## DOTween and USS animations
In order to facilitate the use of DOTween with UIToolkit elements I created an extension class to be able to use shorthand methods for VisualElements
You can see below the Tweens I've made to animate the login and disconnect.
  
> Login animation (and error message)
![The UI eases to the bottom after login in successfully](/Gitassets/connect.gif)

> Disconnect animation
![another user disappears from the list then the UI eases to the top after we press disconnect](/Gitassets/disconnect.gif)

<br>

## Photon Engine
Following the documentation, the basic functions of connecting to the server and joining a room (by using JoinOrCreateRandom) as well as leaving that room and disconnecting.


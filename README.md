Boats-Boats-Boats
=================

IMPORTANT:  This game uses XNA Framework 4.0.  In order to run the game you need to have the Microsoft XNA Framework Redistributable 4.0 Refresh.

It can be downloaded from:
  http://www.microsoft.com/en-us/download/details.aspx?id=27598

The installation file is also included in the INSTALL folder

In order to write any code on top of the project or to build the project, it is necessary to have the entire XNA Framework installed

================
SUMMARY
================

This project is a 2D arcade style shooter.  Sail your ship and survive as long as you can against endless amounts of enemy ships and boss type enemies.

The project is intended to be fun and entertaining, but also to be a learning experience for the developers and will hopefully serve as an example to others who wish to try their hand at making a game using the XNA Framework.  

We are by no means experts, but we hope that people will learn from our mistakes, or take away from our success

================
INSTALL
================
1)  Go into the INSTALL folder
2)  If you do not have the XNA Framework 4.0 Redistributable 4.0 Refresh installed, double click "xnafx40_redist.msi"
2a) Follow installation instructions
3)  Double click the "setup" file.  This checks to make sure that you have all other necessary componenets.
3a) Allow the "setup" file to install any components that you do not have
4)  Double click "PirateWars" file (the original name of the project was Pirate Wars) to install the game
5) Enjoy!

NOTE: The installation file was generated using Microsoft's ClickOnce Application Deployment Manifest.
      The game does not automatically check for updates, so in order to install the newest version, you will have to go into Control Panel and manually uninstall the application, and then reinstall it (uninstall and skip directly to step 4)
    
================
DEVELOPMENT PLANS
================
As of right now, these are the plans for the development of the game

  -Clean up UI
    *Create Instruction window
    *Add background animation to start menu
    *Create loading screen
    *Create more interesting background image for game instead of the solid color blue background
    *Make HUD more interesting
  -Create multiple game types
    *Wave Mode
      Only spawn fireboats, and have them move horizontally and vertically across the screen.
      Waves spawn faster and with more enemies as game continues
    *Boss Battle
      Fight each boss one at a time and try and defeat them as fast as you can without dying
    *King Of The Hill
      Keep the screen clear of all enemies for as long as you can
  -Create more enemy and boss types
  -Balance ship powers
  -Perfect spawning algorithm
    *Refine algorithm for how many ships spawn when
    *Refine when a boss spawns and how the game environment and player behavior changes as a result
  -Add more powerups
    *Currently only have health powerup
    *Add powerup that takes away time from ability recharge
    *Add powerup that increases the number of cannons on the player's ship
  -Generalize code
    *Make mouse and keyboard event handeling code more general so that it can be used in any project
    *Make UI elements reusable
    *Create a set of Windows Libraries that can distributed and re-used in future projects
    
      

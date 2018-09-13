# Breeze Chess

This is a Chess Playing AI I wrote my Senior Year of High School.  It's written in C#, and has several features

() UCI Compatibility
  - Allows you to play with other UCI (Universal Chess Interface) Compatible Engines
  - Allows you to use a GUI of your choice
  - Allows you to set the search depth
  
() Standalone Play
  - Type "debug", and then "play"
  - Also features a demo mode, accessible by typing "debug", and then "demo"
  
() Evaluation
  - Currently based on piece count
  - Uses Alpha-Beta Pruning to speed up the process

Future Plans
------------
In favor of increased speed and evaluation, I am abandoning this project and rewriting the engine in C++.  The new engine will be called Dolphin Chess, and will feature bitboards, location based piece weights, and (tons) more speed.

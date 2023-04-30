# Minesweeper

Start date : 29-04-2023
End date : 30-04-2023

This is the third game in my attempt to learn more about programming in unity by making 50 games. 
Minesweeper is kind of interesting in this process, because it requires interactivity between the player and the game
through clicking on GUI-esque elements (the cells) and changing the sprites of those elements accordingly. Coming
to grips with how sprite renderers work will be usefull for any future project.
A sort of interesting concept I never thought about is "how do you make sure the first click is not on a mine?". The answer,
initialize the board without any mines, and only place mines once the first click has been made, whilst making sure that
no bombs are placed on the place that you first clicked.


What can I do better?
- Most of the functionality of the game is stuffed away in a single script. This is doable for such a small project,
but kind of hard to read for anyone who isn't the author of the code. Need to find a way to keep some sort of order.
- Writing documentation and docstring at the end of the project is a pain in the ass, I should always write about the
functionality of the functions as soon as I write them.


Features I could have added with more time:
- difficulty sliders (more bombs, bigger or smaller arena)
- A timer that shows you how long you have been playing

![a screenshot of the minesweeper game](Img/minesweeper_screenshot.png)
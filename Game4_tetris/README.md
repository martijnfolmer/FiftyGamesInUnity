# Tetris

Start date : 14-05-2023
End date : 18-04-2023

This is the fourth game in my attempt to learn more about programming in unity by making 50 games. 

So, this was a bit of a mess. As tetris is just blocks moving in a grid, I didn't have to mess with kinematics
or collisions, which I thought would be simpler. However, that does mean you have to code "collision behaviour" 
and such yourself, and if you don't think about edge cases beforehand, your code is going to be a bit
of a mess, as happened with me. This would be a problem for bigger projects with more than 1 person working on it.
But this version works, and for the first 5 games my goal is to just make something that someone can look at and 
say "I recognise that". It doesn't have to be fancy, and in that regard I succeeded.

What can I do better?
- Most of the functionality of the game is stuffed away in a single script. This is doable for such a small project,
but kind of hard to read for anyone who isn't the author of the code. Need to find a way to keep some sort of order.
- Writing documentation and docstring at the end of the project is a pain in the ass, I should always write about the
functionality of the functions as soon as I write them.


Features I could have added with more time:
- Adjustable difficulty
- A highscore table
- A field where there is already a bunch of blocks, and you have to clear them
- 2 falling blocks? So you have to control 2 tetrominos at the same time? That sounds awfull and awesome.

![a screenshot of the minesweeper game](img/tetris_screenshot.png)

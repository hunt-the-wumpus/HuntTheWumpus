[![Build status](https://ci.appveyor.com/api/projects/status/n810vv9l1x0jolpk/branch/master?svg=true)](https://ci.appveyor.com/project/HuntTheWumpus/huntthewumpus/branch/master)

# HuntTheWumpus

### About project
HuntTheWumpus is remake project for N_-_N by Molten Great Cleaner Shaft team. This project write on C# with use Git for control vesrion and xUnit for UnitTesting, appveyor for Continues Integration. 

## Mehaniks

### Map
Cave are 30 rooms and bridge. Every room contains <= 3 bridges. Every bridge have 1-4 coins.

### Danger
3 danger
* Bat. 2 Bats live in 2 rooms. If your meet a bat, you and bat respawn
* Pit. Pits are 2 in rooms. If you enter room with Pit, you must complete the MiniGame for survival
* Wumpus. Main danger. If you enter room with wumpus, you must complete the MiniGame. Wumpus will run away, if you will defeat him. On medium and hard Difficulty sometimes wumpus  can walk in the Cave. To win, you must shoot an arrow to the room, where wumpus sleeps.
If you near a danger, you'll see a baner about this danger

### Player
Player can buy 2 arrows (15 coins) and a Hint (25 coins). To buy, you must complete the MiniGame. If you spent all arrows, you loose. If you buy Hint, you can see bats, pits and Wumpus Direction.


### Achivements

![Speedy](HuntTheWumpus/HuntTheWumpus/data/Achievements/MG1.png) 

Finish buy arrow mini-game in less than 7 seconds. 

![Reactive](HuntTheWumpus/HuntTheWumpus/data/Achievements/MG2.png) 

Finish pit or hint mini-game in less than 7 seconds. 

![Zinger](HuntTheWumpus/HuntTheWumpus/data/Achievements/MG3.png) 

Finish wumpus mini-game in less than 7 seconds. 

![Precise](HuntTheWumpus/HuntTheWumpus/data/Achievements/Precise.png)

Score 500 point in the mini-game on the 1 lvl by drawing only 1 figure

![Sniper](HuntTheWumpus/HuntTheWumpus/data/Achievements/Sniper.png)

Score 500 point in the mini-game on the 3 lvl by drawing only 1 

![Explorer](HuntTheWumpus/HuntTheWumpus/data/Achievements/Explorer.png)

Enter all rooms of the cave

![Hamilton's descendant](HuntTheWumpus/HuntTheWumpus/data/Achievements/Hamilton.png)

Go through all the rooms visiting each 1 time only

![Step into the Abyss](HuntTheWumpus/HuntTheWumpus/data/Achievements/Step.png)

Explore 12 rooms

![Shooter](HuntTheWumpus/HuntTheWumpus/data/Achievements/Shooter.png)

Get 2 arrows

![Hunter](HuntTheWumpus/HuntTheWumpus/data/Achievements/Hunter.png)

Get 4 arrows

![Robin Hood](HuntTheWumpus/HuntTheWumpus/data/Achievements/Robin.png)

Get 8 arrows

## Documentaion

[Object assignment](https://github.com/hunt-the-wumpus/Docs).

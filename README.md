# Overview
An implementation of the Glicko rating system to be used for smash local scenes, or any similiarly scored tournament ran on smash.gg. No other websites are currently planned to be added to input data, but such a possibility isn't out of the question. It has a web frontend for ease of use by the end user, ~~and because I didn't realize how much of a pain that is to do. Currently on hiatus due to college and other projects.~~


# Glicko
Glicko-2 is a skill rating system that accounts for player volatility. Smash is extremely volatile, upsets are common, and a bad day can lead to bad results. Being able to account for that compared to the more plain systems like ELO is imperative. Furthermore the system accounts for the greater potential volatility of players who take an extended break from smash.

It's technically less of a skill rating system, and more of a performance rating system. Because a Ganon main is gonna struggle to better player a win against a Pika.

# Database systems
This application works under 2 seperately stored, but linked data : Leagues and Events

Events hold all the set data for an event in a tournament. It has the Smash.gg UserIDs and set count.

Leagues are accurate to the name. They are seperate fields in which players are ranked agaisnt eachother. Players who participate in Tekken and Smash shouldn't have one skill rating for both, and an Alaska and Georgia local scene shouldn't have players with directly comparable skill ratings, since they never fight eachother. Basic statistic stuff.

Leagues are seperated into Timeframes. These are units of time of fixed length (as defined on the creation of the League itself) and are used to group events into large chunks of data to feed into the Glicko backend to determine Player Skill over time.

Events are NOT stored as a part of a League. They are their own entry in the database, and Leagues merely reference their ID within the timeframe they are a part of.

# TODO
This is currently VERY unfinished. Some of what's left to do is :

Add timeframes functionality to the app.

Add Ownership, Moderators, and Viewers (for private leagues) of Leagues - Will come after leagues have basic functionality and can properly seed 

Add google authentication - I really don't want to deal with my own security. This is substantially better for that reason alone.

Unify Serialization to actually be well coded. (Namely the XML stuff)

Rework literally everything to work in a queue system to prevent Rate Limiting from throwing Errors - This is last because I just want to get a functional system before I mess with that. Until then I'm still going to try to make most backend things functions to make the transition not hurt really bad.

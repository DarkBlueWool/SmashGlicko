// Glicko-2 Smash Implementation.cpp : This file contains the 'main' function. Program execution begins and ends there.
//
#include <iostream>
#include <string>
#include <map>

//I just picked this as a guess. Experimentation needed.
const float SystemConstant = 0.4;
const float PI = 3.1415926535897932384626433832795028841971693993751058209749445923078164;

class Player {
public:
    unsigned int id;
    float rating;
    float deviation;
    float volatility;
    Player() {
        id = 0;
        rating = 0;
        deviation = 350 / 173.7178;
        volatility = 0.06;
    }

    Player(unsigned int ID) {
        id = ID;
        rating = 0;
        deviation = 350 / 173.7178;
        volatility = 0.06;
    }

    std::string PrintableFormat() {
        std::string output = "\nPlayer : " + std::to_string(id);
        output += "\n   Rating : " + std::to_string(rating);
        output += "\n   Deviation : " + std::to_string(deviation);
        output += "\n   Volatility : " + std::to_string(volatility);
        return output;
    }
};

class Event {
public:
    int id;
    unsigned int MtchCount;
    unsigned int* Player1;
    unsigned int* Player2;
    //Player 1 winning games / total games
    float* Score;
    //mtchCount is the TOTAL number of Sets played, EventID is the id of the Event - use 0 for an event with no smash.gg ID (custom / concated events)
    Event(unsigned int mtchCount, int EventID) {
        id = EventID;
        MtchCount = mtchCount;
        Player1 = new unsigned int[MtchCount];
        Player2 = new unsigned int[MtchCount];
        Score = new float[MtchCount];
    }
    ~Event() {
        //This may need to be fixed, but for now it stays commented since it breaks otherwise :(
        //delete Player1;
        //delete Player2;
        //delete Score;
    }
};

//A list of Player Classes
class PlayerList {
public:
    std::string Name;
    std::map<unsigned int, Player> PlayerDict;

    PlayerList(std::string name) {
        Name = name;
    }
    PlayerList() {
        Name = "";
    }

    void AddPlayer(Player Plyr) {
        PlayerDict.insert(std::pair<unsigned int, Player>(Plyr.id, Plyr));
    }

    void AddNewPlayer(unsigned int PlayerID) {
        PlayerDict.insert(std::pair<unsigned int, Player>(PlayerID, Player(PlayerID)));
    }

    void CopyList(PlayerList Input) {
        //Loops through the content of Input
        for (std::map<unsigned int, Player>::iterator it = Input.PlayerDict.begin(); it != Input.PlayerDict.end(); ++it) {
            PlayerDict.insert(std::pair<unsigned int, Player>(it->first, it->second));
        }
    }
    void PrintList() {
        std::string output = "\n--------------- Printing PlayerList " + Name + " ---------------";
        //Loops through the content of Dictionary 
        for (std::map<unsigned int, Player>::iterator it = PlayerDict.begin(); it != PlayerDict.end(); ++it) {
            output += it->second.PrintableFormat();
        }
        printf(output.c_str());
    };
};

class EventList {
public:
    std::string Name;
    std::map<unsigned int, Event> EventDict;

    //Name can be empty and nothing will break - it's just useful for debugging purposes
    EventList(std::string name) {
        Name = name;
    }
    EventList() {
        Name = "";
    }

    void CopyList(EventList Input) {
        //Loops through the content of Input
        for (std::map<unsigned int, Event>::iterator it = Input.EventDict.begin(); it != Input.EventDict.end(); ++it) {
            EventDict.insert(std::pair<unsigned int, Event>(it->first, it->second));
        }
    }

    //Returns true if the event was added, false if it already existed
    bool AddEvent(Event Ev) {
        std::pair<std::map<unsigned int, Event>::iterator, bool> ret;
        ret = EventDict.insert(std::pair<unsigned int, Event>(Ev.id, Ev));
        return ret.second;
    }

    //Concates all events into one - mainly to be able to be processed by the Glicko-2 Implementation side of the code
    Event concate() {
        unsigned int TotalSets = 0;
        //Loops through the Events, adding the match count of each to the total match count
        for (std::map<unsigned int, Event>::iterator it = EventDict.begin(); it != EventDict.end(); ++it) {
            TotalSets += it->second.MtchCount;
        }

        Event Output = Event(TotalSets, 0);
        unsigned int i = 0;

        for (std::map<unsigned int, Event>::iterator it = EventDict.begin(); it != EventDict.end(); ++it) {
            for (unsigned int i2 = 0; i2 < it->second.MtchCount; i2++) {
                Output.Player1[i] = it->second.Player1[i2];
                Output.Player2[i] = it->second.Player2[i2];
                Output.Score[i] = it->second.Score[i2];
                i++;
            }
        }

        return Output;
    }
};

//Calculations based on Glicko Documentation
//http://www.glicko.net/glicko/glicko2.pdf
class GlickoShit {
public :
    static float GShortcut(float OpponentDeviation) {
        return 1 / sqrt(1 + 3 * (OpponentDeviation * OpponentDeviation) / (PI * PI));
    }

    static float EShortcut(float Rating, float OpponentRating, float GOut) {
        return 1 / (1 + exp(-GOut * (Rating - OpponentRating)));
    }

    static float EstVariance(int OCount, float Rating, float* ORat, float* ODev) {
        float InvV = 0;
        float GOut;
        float EOut;
        for (int i = 0; i < OCount; i++) {
            GOut = GShortcut(ODev[i]);
            EOut = EShortcut(Rating, ORat[i], GOut);
            InvV += GOut * GOut * EOut * (1 - EOut);
        }
        return 1 / InvV;
    }

    //The basic calculation of how a rating changes after a series of games / sets. This is before the volatility is calculated
    //Has EstVariance baked into it
    static float RawDeltaRating(unsigned int OCount, float Rating, float* ORat, float* ODev, float* Scores, float Variance) {
        float Output = 0;
        float GOut;
        float EOut;
        for (unsigned int i = 0; i < OCount; i++) {
            GOut = GShortcut(ODev[i]);
            EOut = EShortcut(Rating, ORat[i], GOut);
            Output += GOut * (Scores[i] - EOut);
        }
        return Output * Variance;
    }

    static float VolTempFunc(float x, float RawDelRat, float Deviation, float Variance, float a) {
        float temp = Deviation * Deviation + Variance + exp(x);
        return exp(x) * (RawDelRat * RawDelRat - temp) / (2 * temp * temp) - (x - a) / (SystemConstant * SystemConstant);
    }

    //The τ in the documention is the system constant
    static float CalcVolatility(float OldVol, float RawDelRat, float Deviation, float Variance) {
        float NewVol;
        float ConvergenceTolerance = 0.000001;

        //Get bracket for iterations
        float A = log(OldVol * OldVol);
        float B = log(RawDelRat * RawDelRat - Deviation * Deviation - Variance);
        //Iteration to get iteration bracket :pikathink:
        if (B < 0) {
            float k = 0; //effectively starts at 1 since K gets added to immediately
            while (B < 0) {
                k += 1;
                B = VolTempFunc(A - k * SystemConstant, RawDelRat, Deviation, Variance, A);
            }
        }

        //iterations time (Idk what any of this does in detail I'm just copying Glicko)
        float C;
        float FA = VolTempFunc(A, RawDelRat, Deviation, Variance, A);
        float FB = VolTempFunc(B, RawDelRat, Deviation, Variance, A);
        float FC;
        while (abs(B - A) > ConvergenceTolerance) {
            C = A + (A - B) * FA / (FB - FA);
            FC = VolTempFunc(C, RawDelRat, Deviation, Variance, A);

            if (FC * FB < 0) {
                A = B;
                FA = FB;
            }
            else {
                FA = FA / 2;
            }
            B = C;
            FB = FC;
        }

        NewVol = exp(A / 2);
        return NewVol;
    }

    static Player CalcNewInfo(Player *Plyr, Event Tourney, PlayerList PlyrLst) {
        Player Output = Player();
        Output.id = Plyr->id;

        //count number of matchs
        unsigned int MatchCount = 0;
        for (unsigned int i = 0; i < Tourney.MtchCount; i++) {
            if (Tourney.Player1[i] == Plyr->id || Tourney.Player2[i] == Plyr->id) {
                MatchCount += 1;
            }
        }


        //Set up arrays
        float* ORat = new float[MatchCount];
        float* ODev = new float[MatchCount];
        float* Scores = new float[MatchCount];
        unsigned int counter = 0;
        for (unsigned int i = 0; i < Tourney.MtchCount; i++) {
            if (Tourney.Player1[i] == Plyr->id) {
                ORat[counter] = PlyrLst.PlayerDict[Tourney.Player2[i]].rating;
                ODev[counter] = PlyrLst.PlayerDict[Tourney.Player2[i]].deviation;
                Scores[counter] = Tourney.Score[i];
                counter++;
            }
            else if (Tourney.Player2[i] == Plyr->id) {
                ORat[counter] = PlyrLst.PlayerDict[Tourney.Player1[i]].rating;
                ODev[counter] = PlyrLst.PlayerDict[Tourney.Player1[i]].deviation;
                Scores[counter] = 1 - Tourney.Score[i];
                counter++;
            }
        }

        //Start crunching numbers
        float Variance = EstVariance(counter, Plyr->rating, ORat, ODev);
        float RawDelRat = RawDeltaRating(MatchCount, Plyr->rating, ORat, ODev, Scores, Variance);
        float NewVol = CalcVolatility(Plyr->volatility, RawDelRat, Plyr->deviation, Variance);

        float NewDev = 1 / sqrt(1 / (Plyr->deviation * Plyr->deviation + NewVol * NewVol) + 1 / Variance);

        float GOut;
        float EOut;
        float TrueDelRat = 0;
        for (unsigned int i = 0; i < MatchCount; i++) {
            GOut = GShortcut(ODev[i]);
            EOut = EShortcut(Plyr->rating, ORat[i], GOut);
            TrueDelRat += GOut * (Scores[i] - EOut);
        }
        TrueDelRat = TrueDelRat * NewDev * NewDev;

        //Return the new player
        Output.deviation = NewDev;
        Output.rating = Plyr->rating + TrueDelRat;
        Output.volatility = NewVol;
        return Output;
    }
};

class TimeScale {
public:
    //The number of the timescale period
    unsigned int TimeScaleNum;
    //The Players at the start of the timescale
    PlayerList Players;
    //The events within the timescale
    EventList Events;

    PlayerList GenerateNewRatings() {
        PlayerList Output = PlayerList("New PlayerList");
        Output.CopyList(Players);
        Event TheBigEvent = Events.concate();
        for (std::map<unsigned int, Player>::iterator it = Players.PlayerDict.begin(); it != Players.PlayerDict.end(); ++it) {
            Output.PlayerDict[it->first] = GlickoShit::CalcNewInfo(&(it->second), TheBigEvent, Players);
        }
        return Output;
    }

    TimeScale(unsigned int TimeScaleNumber, PlayerList Plyrs) {
        Players = Plyrs;
        Events = EventList();
        TimeScaleNum = TimeScaleNumber;
    }
    
    TimeScale(unsigned int TimeScaleNumber) {
        Players = PlayerList();
        Events = EventList();
        TimeScaleNum = TimeScaleNumber;
    }

    TimeScale NextTimeScale() {
        return TimeScale(TimeScaleNum + 1, GenerateNewRatings());
    }

    //Adds an event to the events 
    bool AddEvent(Event Ev) {
        return Events.AddEvent(Ev);
    }
};

//testing stuff
int main()
{
    unsigned int plyrcount = 30;
    TimeScale TS = TimeScale(0);
    for (unsigned int i = 0; i < plyrcount; i++) {
        TS.Players.AddNewPlayer(i);
    }

    int randIntensity = 10;

    //Set up fake tourney results ( bigger number better player ;) )
    Event TestTourney1 = Event(plyrcount * plyrcount - plyrcount, 0);
    unsigned int counter = 0;
    for (unsigned int i = 0; i < plyrcount; i++) {
        for (unsigned int i2 = 0; i2 < plyrcount; i2++) {
            if (i != i2) {
                TestTourney1.Player1[counter] = i;
                TestTourney1.Player2[counter] = i2;
                if (i + (rand() % randIntensity) > i2 + (rand() % randIntensity)) {
                    TestTourney1.Score[counter] = 1;
                } else {
                    TestTourney1.Score[counter] = 0;
                }
                counter++;
            }
        }
    }

    //Set up fake tourney results ( bigger number better player ;) )
    Event TestTourney2 = Event(plyrcount * plyrcount - plyrcount, 1);
    counter = 0;
    for (unsigned int i = 0; i < plyrcount; i++) {
        for (unsigned int i2 = 0; i2 < plyrcount; i2++) {
            if (i != i2) {
                TestTourney2.Player1[counter] = i;
                TestTourney2.Player2[counter] = i2;
                if (i + (rand() % randIntensity) > i2 + (rand() % randIntensity)) {
                    TestTourney2.Score[counter] = 1;
                }
                else {
                    TestTourney2.Score[counter] = 0;
                }
                counter++;
            }
        }
    }

    //Set up fake tourney results ( bigger number better player ;) )
    Event TestTourney3 = Event(plyrcount * plyrcount - plyrcount, 2);
    counter = 0;
    for (unsigned int i = 0; i < plyrcount; i++) {
        for (unsigned int i2 = 0; i2 < plyrcount; i2++) {
            if (i != i2) {
                TestTourney3.Player1[counter] = i;
                TestTourney3.Player2[counter] = i2;
                if (i + (rand() % randIntensity) > i2 + (rand() % randIntensity)) {
                    TestTourney3.Score[counter] = 1;
                }
                else {
                    TestTourney3.Score[counter] = 0;
                }
                counter++;
            }
        }
    }

    TS.Events.AddEvent(TestTourney1);
    TS.Events.AddEvent(TestTourney2);
    TS.Events.AddEvent(TestTourney3);

    TimeScale TimeScaleAfter = TS.NextTimeScale();

    TimeScaleAfter.Players.PrintList();

    return 0;
}






// Run program: Ctrl + F5 or Debug > Start Without Debugging menu
// Debug program: F5 or Debug > Start Debugging menu

// Tips for Getting Started: 
//   1. Use the Solution Explorer window to add/manage files
//   2. Use the Team Explorer window to connect to source control
//   3. Use the Output window to see build output and other messages
//   4. Use the Error List window to view errors
//   5. Go to Project > Add New Item to create new code files, or Project > Add Existing Item to add existing code files to the project
//   6. In the future, to open this project again, go to File > Open > Project and select the .sln file

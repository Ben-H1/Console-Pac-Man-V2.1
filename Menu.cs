using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PacManV2._1
{
    class Menu
    {
        Utilities util = new Utilities();
        Game game = new Game();
        SoundPlayer sp;

        private bool sound = true;

        private int consoleWidth = 120; // default: 120
        private int consoleHeight = 30; // default: 30

        private ConsoleColor fgCol = ConsoleColor.White;
        private ConsoleColor bgCol = ConsoleColor.Black;

        private string highScoresPath;
        private dynamic highScores;

        private string[] mainMenuItems = { "Start game",
                                           "High scores",
                                           "Settings",
                                           "About",
                                           "Quit" };
        private string[] settingsMenuItems = { "Change board colours",
                                               "Change pellet characters",
                                               "Turn sound on or off",
                                               "Back to main menu" };
        private string[] boardColourItems = { "Change foreground colour",
                                              "Change background colour",
                                              "Back to settings" };
        private string[] foregroundColourItems = { "Blue - default",
                                                   "Dark blue",
                                                   "Cyan",
                                                   "Dark cyan",
                                                   "Gray",
                                                   "Dark gray",
                                                   "Green",
                                                   "Dark green",
                                                   "Magenta",
                                                   "Dark magenta",
                                                   "Red",
                                                   "Dark red",
                                                   "Yellow",
                                                   "Dark yellow",
                                                   "White",
                                                   "Black",
                                                   "Back" };
        private string[] backgroundColourItems = { "Black - default",
                                                   "Blue",
                                                   "Dark blue",
                                                   "Cyan",
                                                   "Dark cyan",
                                                   "Gray",
                                                   "Dark gray",
                                                   "Green",
                                                   "Dark green",
                                                   "Magenta",
                                                   "Dark magenta",
                                                   "Red",
                                                   "Dark red",
                                                   "Yellow",
                                                   "Dark yellow",
                                                   "White",
                                                   "Back" };
        private string[] pelletCharacterMenuItems = { "Change pellet character",
                                                      "Change power pellet character",
                                                      "Back to settings" };
        private string[] pelletCharacterItems = { "'.' - default",
                                                  "'-'",
                                                  "'+'",
                                                  "'■'",
                                                  "Back" };
        private string[] powerPelletCharacterItems = { "'■' - default",
                                                       "'.'",
                                                       "'-'",
                                                       "'+'",
                                                       "Back" };
        private string[] soundMenuItems = { "On - default",
                                            "Off",
                                            "Back" };

        private string[] aboutInfo = {"Console Pac Man",
                                      "Version: 2.1",
                                      "Author: Ben Hawthorn (github.com/Ben-H1)"};

        private int selectedItem = 0;

        public Menu()
        {
            sp = new SoundPlayer(util.createPath("start.wav"));
            this.highScoresPath = util.createPath("HighScores.json");
            this.highScores = util.readFile(highScoresPath);
            util.setConsoleDimensions(consoleWidth, consoleHeight);
            
            Console.CursorVisible = false;
            mainMenu();
        }

        public void mainMenu()
        {
            util.setConsoleDimensions(consoleWidth, consoleHeight);
            util.setConsoleColours(fgCol, bgCol);
            Console.Clear();
            selectedItem = 0;
            drawBorder("Pac Man");

            drawMenuItems(mainMenuItems);

            while (true) {
                ConsoleKey key = Console.ReadKey().Key;

                switch (key) {
                    case ConsoleKey.UpArrow:
                    case ConsoleKey.LeftArrow:
                        changeSelectedItem(-1, mainMenuItems.Length - 1);
                        break;

                    case ConsoleKey.DownArrow:
                    case ConsoleKey.RightArrow:
                        changeSelectedItem(1, mainMenuItems.Length - 1);
                        break;

                    case ConsoleKey.Enter:
                        switch (selectedItem) {
                            case 0:
                                sp.Play();
                                System.Threading.Thread.Sleep(500);
                                game.startGame(highScores[0].score);
                                enterHighScore(game.getScore());
                                break;
                            case 1:
                                highScoresMenu();
                                break;
                            case 2:
                                settingsMenu();
                                break;
                            case 3:
                                aboutMenu();
                                break;
                            case 4:
                                Environment.Exit(0);
                                break;
                        }
                        break;
                }

                drawMenuItems(mainMenuItems);
            }

        }

        #region "High scores"

        public void highScoresMenu()
        {
            Console.Clear();
            drawBorder("Pac Man - High scores");

            drawHighScores(highScores);

            util.setConsoleColours(bgCol, fgCol);
            string backString = "Back to main menu";
            Console.SetCursorPosition(((consoleWidth / 2) - (backString.Length / 2)) - 1, (consoleHeight / 2) + 4);
            Console.Write(backString);
            Console.SetCursorPosition(consoleWidth - 2, consoleHeight - 2);
            util.setConsoleColours(fgCol, bgCol);

            while (true) {
                ConsoleKey key = Console.ReadKey().Key;

                switch (key) {
                    case ConsoleKey.Enter:
                        mainMenu();
                        break;
                }

                Console.SetCursorPosition(consoleWidth - 2, consoleHeight - 2);
            }
        }

        public void enterHighScore(int score)
        {
            if (withinHighScores(score) >= 0) {
                util.setConsoleDimensions(consoleWidth, consoleHeight);
                util.setConsoleColours(fgCol, bgCol);
                Console.Clear();
                drawBorder("Pac Man - Enter high score");

                string scoreString = "Your score: " + score;
                Console.SetCursorPosition(((consoleWidth / 2) - (scoreString.Length / 2)), (consoleHeight / 2) - 2);
                Console.Write(scoreString);

                string enterNameString = "Enter your name: ";
                Console.SetCursorPosition(((consoleWidth / 2) - (enterNameString.Length / 2)) - 5, (consoleHeight / 2));
                Console.Write(enterNameString);

                string pressEnterString = "Press Enter when finished";
                Console.SetCursorPosition(((consoleWidth / 2) - (pressEnterString.Length / 2)), (consoleHeight / 2) + 2);
                Console.Write(pressEnterString);

                Console.SetCursorPosition(((consoleWidth / 2) - (enterNameString.Length / 2)) + 12, (consoleHeight / 2));

                Console.CursorVisible = true;
                addHighScore(Console.ReadLine(), score, withinHighScores(score));
                Console.CursorVisible = false;

                highScoresMenu();
            }

            mainMenu();
        }

        public int withinHighScores(int score)
        {
            for (int i = 0; i < highScores.Count; i++) {
                int currentHighScore = highScores[i].score;
                if (score > currentHighScore) {
                    return i;
                }
            }

            return -1;
        }

        public void addHighScore(string name, int score, int index)
        {

            for (int i = highScores.Count - 1; i > index; i--) {
                    highScores[i].name = highScores[i - 1].name;
                    highScores[i].score = highScores[i - 1].score;
            }

            highScores[index].name = name;
            highScores[index].score = score;
        }

        public void drawHighScores(dynamic highScores)
        {
            for (int i = 0; i < highScores.Count; i++) {
                string scoreString = highScores[i].name + ": " + highScores[i].score;
                Console.SetCursorPosition(((consoleWidth / 2) - (scoreString.Length / 2)) - 1, (consoleHeight / 2) - 7 + i);
                Console.Write(scoreString);
            }
        }

        public void writeScoresToFile(dynamic highScores)
        {

        }

        #endregion

        #region "Settings"

        public void settingsMenu()
        {
            Console.Clear();
            selectedItem = 0;
            drawBorder("Pac Man - Settings");

            drawMenuItems(settingsMenuItems);

            while (true) {
                ConsoleKey key = Console.ReadKey().Key;

                switch (key) {
                    case ConsoleKey.UpArrow:
                    case ConsoleKey.LeftArrow:
                        changeSelectedItem(-1, settingsMenuItems.Length - 1);
                        break;

                    case ConsoleKey.DownArrow:
                    case ConsoleKey.RightArrow:
                        changeSelectedItem(1, settingsMenuItems.Length - 1);
                        break;

                    case ConsoleKey.Enter:
                        switch (selectedItem) {
                            case 0:
                                changeBoardColoursMenu();
                                break;
                            case 1:
                                changePelletCharactersMenu();
                                break;
                            case 2:
                                soundMenu();
                                break;
                            case 3:
                                mainMenu();
                                break;
                        }
                        break;
                }

                drawMenuItems(settingsMenuItems);
            }
        }
        
        public void changeBoardColoursMenu()
        {
            Console.Clear();
            selectedItem = 0;
            drawBorder("Pac Man - Change board colours");

            drawMenuItems(boardColourItems);

            while (true) {
                ConsoleKey key = Console.ReadKey().Key;

                switch (key) {
                    case ConsoleKey.UpArrow:
                    case ConsoleKey.LeftArrow:
                        changeSelectedItem(-1, boardColourItems.Length - 1);
                        break;

                    case ConsoleKey.DownArrow:
                    case ConsoleKey.RightArrow:
                        changeSelectedItem(1, boardColourItems.Length - 1);
                        break;

                    case ConsoleKey.Enter:
                        switch (selectedItem) {
                            case 0:
                                changeForegroundColourMenu();
                                break;
                            case 1:
                                changeBackgroundColourMenu();
                                break;
                            case 2:
                               settingsMenu();
                                break;
                        }
                        break;
                }

                drawMenuItems(boardColourItems);
            }
        }

        public void changeForegroundColourMenu()
        {
            Console.Clear();
            selectedItem = 0;
            drawBorder("Pac Man - Change board foreground colour");

            drawMenuItems(foregroundColourItems);

            while (true) {
                ConsoleKey key = Console.ReadKey().Key;

                switch (key) {
                    case ConsoleKey.UpArrow:
                    case ConsoleKey.LeftArrow:
                        changeSelectedItem(-1, foregroundColourItems.Length - 1);
                        break;

                    case ConsoleKey.DownArrow:
                    case ConsoleKey.RightArrow:
                        changeSelectedItem(1, foregroundColourItems.Length - 1);
                        break;

                    case ConsoleKey.Enter:
                        switch (selectedItem) {
                            case 0:
                                game.board.setForegroundColour(ConsoleColor.Blue);
                                break;
                            case 1:
                                game.board.setForegroundColour(ConsoleColor.DarkBlue);
                                break;
                            case 2:
                                game.board.setForegroundColour(ConsoleColor.Cyan);
                                break;
                            case 3:
                                game.board.setForegroundColour(ConsoleColor.DarkCyan);
                                break;
                            case 4:
                                game.board.setForegroundColour(ConsoleColor.Gray);
                                break;
                            case 5:
                                game.board.setForegroundColour(ConsoleColor.DarkGray);
                                break;
                            case 6:
                                game.board.setForegroundColour(ConsoleColor.Green);
                                break;
                            case 7:
                                game.board.setForegroundColour(ConsoleColor.DarkGreen);
                                break;
                            case 8:
                                game.board.setForegroundColour(ConsoleColor.Magenta);
                                break;
                            case 9:
                                game.board.setForegroundColour(ConsoleColor.DarkMagenta);
                                break;
                            case 10:
                                game.board.setForegroundColour(ConsoleColor.Red);
                                break;
                            case 11:
                                game.board.setForegroundColour(ConsoleColor.DarkRed);
                                break;
                            case 12:
                                game.board.setForegroundColour(ConsoleColor.Yellow);
                                break;
                            case 13:
                                game.board.setForegroundColour(ConsoleColor.DarkYellow);
                                break;
                            case 14:
                                game.board.setForegroundColour(ConsoleColor.White);
                                break;
                            case 15:
                                game.board.setForegroundColour(ConsoleColor.Black);
                                break;
                            case 16:
                                changeBoardColoursMenu();
                                break;
                        }
                        break;
                }

                drawMenuItems(foregroundColourItems);
            }
        }

        public void changeBackgroundColourMenu()
        {
            Console.Clear();
            selectedItem = 0;
            drawBorder("Pac Man - Change board background colour");

            drawMenuItems(backgroundColourItems);

            while (true) {
                ConsoleKey key = Console.ReadKey().Key;

                switch (key) {
                    case ConsoleKey.UpArrow:
                    case ConsoleKey.LeftArrow:
                        changeSelectedItem(-1, backgroundColourItems.Length - 1);
                        break;

                    case ConsoleKey.DownArrow:
                    case ConsoleKey.RightArrow:
                        changeSelectedItem(1, backgroundColourItems.Length - 1);
                        break;

                    case ConsoleKey.Enter:
                        switch (selectedItem) {
                            case 0:
                                game.board.setBackgroundColour(ConsoleColor.Black);
                                break;
                            case 1:
                                game.board.setBackgroundColour(ConsoleColor.Blue);
                                break;
                            case 2:
                                game.board.setBackgroundColour(ConsoleColor.DarkBlue);
                                break;
                            case 3:
                                game.board.setBackgroundColour(ConsoleColor.Cyan);
                                break;
                            case 4:
                                game.board.setBackgroundColour(ConsoleColor.DarkCyan);
                                break;
                            case 5:
                                game.board.setBackgroundColour(ConsoleColor.Gray);
                                break;
                            case 6:
                                game.board.setBackgroundColour(ConsoleColor.DarkGray);
                                break;
                            case 7:
                                game.board.setBackgroundColour(ConsoleColor.Green);
                                break;
                            case 8:
                                game.board.setBackgroundColour(ConsoleColor.DarkGreen);
                                break;
                            case 9:
                                game.board.setBackgroundColour(ConsoleColor.Magenta);
                                break;
                            case 10:
                                game.board.setBackgroundColour(ConsoleColor.DarkMagenta);
                                break;
                            case 11:
                                game.board.setBackgroundColour(ConsoleColor.Red);
                                break;
                            case 12:
                                game.board.setBackgroundColour(ConsoleColor.DarkRed);
                                break;
                            case 13:
                                game.board.setBackgroundColour(ConsoleColor.Yellow);
                                break;
                            case 14:
                                game.board.setBackgroundColour(ConsoleColor.DarkYellow);
                                break;
                            case 15:
                                game.board.setBackgroundColour(ConsoleColor.White);
                                break;
                            case 16:
                                changeBoardColoursMenu();
                                break;
                        }
                        break;
                }

                drawMenuItems(backgroundColourItems);
            }
        }

        public void changePelletCharactersMenu()
        {
            Console.Clear();
            selectedItem = 0;
            drawBorder("Pac Man - Change pellet characters");

            drawMenuItems(pelletCharacterMenuItems);

            while (true) {
                ConsoleKey key = Console.ReadKey().Key;

                switch (key) {
                    case ConsoleKey.UpArrow:
                    case ConsoleKey.LeftArrow:
                        changeSelectedItem(-1, pelletCharacterMenuItems.Length - 1);
                        break;

                    case ConsoleKey.DownArrow:
                    case ConsoleKey.RightArrow:
                        changeSelectedItem(1, pelletCharacterMenuItems.Length - 1);
                        break;

                    case ConsoleKey.Enter:
                        switch (selectedItem)
                        {
                            case 0:
                                changePelletCharacterMenu();
                                break;
                            case 1:
                                changePowerPelletCharacterMenu();
                                break;
                            case 2:
                                settingsMenu();
                                break;
                        }
                        break;
                }

                drawMenuItems(pelletCharacterMenuItems);
            }
        }

        public void changePelletCharacterMenu()
        {
            Console.Clear();
            selectedItem = 0;
            drawBorder("Pac Man - Change pellet character");

            drawMenuItems(pelletCharacterItems);

            while (true) {
                ConsoleKey key = Console.ReadKey().Key;

                switch (key) {
                    case ConsoleKey.UpArrow:
                    case ConsoleKey.LeftArrow:
                        changeSelectedItem(-1, pelletCharacterItems.Length - 1);
                        break;

                    case ConsoleKey.DownArrow:
                    case ConsoleKey.RightArrow:
                        changeSelectedItem(1, pelletCharacterItems.Length - 1);
                        break;

                    case ConsoleKey.Enter:
                        switch (selectedItem)
                        {
                            case 0:
                                game.board.setPelletCharacter('.');
                                break;
                            case 1:
                                game.board.setPelletCharacter('-');
                                break;
                            case 2:
                                game.board.setPelletCharacter('+');
                                break;
                            case 3:
                                game.board.setPelletCharacter('■');
                                break;
                            case 4:
                                changePelletCharactersMenu();
                                break;
                        }
                        break;
                }

                drawMenuItems(pelletCharacterItems);
            }
        }

        public void changePowerPelletCharacterMenu()
        {
            Console.Clear();
            selectedItem = 0;
            drawBorder("Pac Man - Change power pellet character");

            drawMenuItems(powerPelletCharacterItems);

            while (true) {
                ConsoleKey key = Console.ReadKey().Key;

                switch (key) {
                    case ConsoleKey.UpArrow:
                    case ConsoleKey.LeftArrow:
                        changeSelectedItem(-1, powerPelletCharacterItems.Length - 1);
                        break;

                    case ConsoleKey.DownArrow:
                    case ConsoleKey.RightArrow:
                        changeSelectedItem(1, powerPelletCharacterItems.Length - 1);
                        break;

                    case ConsoleKey.Enter:
                        switch (selectedItem)
                        {
                            case 0:
                                game.board.setPowerPelletCharacter('■');
                                break;
                            case 1:
                                game.board.setPowerPelletCharacter('.');
                                break;
                            case 2:
                                game.board.setPowerPelletCharacter('-');
                                break;
                            case 3:
                                game.board.setPelletCharacter('+');
                                break;
                            case 4:
                                changePelletCharactersMenu();
                                break;
                        }
                        break;
                }

                drawMenuItems(powerPelletCharacterItems);
            }
        }

        public void soundMenu()
        {
            Console.Clear();
            selectedItem = 0;
            drawBorder("Pac Man - Sound");

            drawMenuItems(soundMenuItems);

            while (true) {
                ConsoleKey key = Console.ReadKey().Key;

                switch (key) {
                    case ConsoleKey.UpArrow:
                    case ConsoleKey.LeftArrow:
                        changeSelectedItem(-1, soundMenuItems.Length - 1);
                        break;

                    case ConsoleKey.DownArrow:
                    case ConsoleKey.RightArrow:
                        changeSelectedItem(1, soundMenuItems.Length - 1);
                        break;

                    case ConsoleKey.Enter:
                        switch (selectedItem)
                        {
                            case 0:
                                setSound(true);
                                break;
                            case 1:
                                setSound(false);
                                break;
                            case 2:
                                settingsMenu();
                                break;
                        }
                        break;
                }

                drawMenuItems(soundMenuItems);
            }
        }

        public void setSound(bool state)
        {
            sound = state;
            game.setSound(state);
        }

        #endregion

        #region "About"

        public void aboutMenu()
        {
            Console.Clear();
            drawBorder("Pac Man - About");

            drawAboutInfo(aboutInfo);

            util.setConsoleColours(bgCol, fgCol);
            string backString = "Back to main menu";
            Console.SetCursorPosition(((consoleWidth / 2) - (backString.Length / 2)) - 1, (consoleHeight / 2) + 2);
            Console.Write(backString);
            Console.SetCursorPosition(consoleWidth - 2, consoleHeight - 2);
            util.setConsoleColours(fgCol, bgCol);

            while (true) {
                ConsoleKey key = Console.ReadKey().Key;

                switch (key) {
                    case ConsoleKey.Enter:
                        mainMenu();
                        break;
                }

                Console.SetCursorPosition(consoleWidth - 2, consoleHeight - 2);
            }
        }

        public void drawAboutInfo(string[] aboutInfo)
        {
            for (int i = 0; i < aboutInfo.Length; i++)
            {
                Console.SetCursorPosition(((consoleWidth / 2) - (aboutInfo[i].Length / 2) - 1), (consoleHeight / 2) - (aboutInfo.Length / 2) - 1 + i);
                Console.Write(aboutInfo[i]);
            }
        }

        #endregion

        #region "Menu drawing"

        public void drawMenuItems(string[] menuItems)
        {
            for (int i = 0; i < menuItems.Length; i++) {
                Console.SetCursorPosition(((consoleWidth / 2) - ((menuItems[i].Length) / 2)) - 1, ((consoleHeight / 2) - (menuItems.Length / 2)) + i);
                if (i == selectedItem) {
                    util.setConsoleColours(bgCol, fgCol);
                    Console.Write(menuItems[i]);
                    util.setConsoleColours(fgCol, bgCol);
                } else {
                    Console.Write(menuItems[i]);
                }
            }

            Console.SetCursorPosition(consoleWidth - 2, consoleHeight - 2);
        }

        public void changeSelectedItem(int amount, int max)
        {
            if (!((selectedItem + amount < 0) || ((selectedItem + amount) > max))) {
                selectedItem += amount;
            }
        }

        public void drawBorder()
        {
            util.setConsoleColours(bgCol, fgCol);
            drawTopBorder();
            drawLeftBorder();
            drawRightBorder();
            drawBottomBorder();
            util.setConsoleColours(fgCol, bgCol);
            util.resetCursorPosition();
        }

        public void drawBorder(string title)
        {
            util.setConsoleColours(bgCol, fgCol);
            drawTopBorder();
            drawLeftBorder();
            drawRightBorder();
            drawBottomBorder();
            util.setConsoleColours(fgCol, bgCol);
            drawTitle(title);
            util.resetCursorPosition();
        }

        public void drawTitle(string title)
        {
            Console.SetCursorPosition(((consoleWidth / 2) - ((title.Length + 1) / 2)) - 1, 0);
            Console.Write(" " + title + " ");

        }

        public void drawTopBorder()
        {
            for (int i = 0; i < consoleWidth; i++) {
                Console.SetCursorPosition(i, 0);
                Console.Write(" ");
            }
        }

        public void drawLeftBorder()
        {
            for (int i = 1; i < consoleHeight - 1; i++) {
                Console.SetCursorPosition(0, i);
                Console.Write(" ");
            }
        }

        public void drawRightBorder()
        {
            for (int i = 1; i < consoleHeight - 1; i++) {
                Console.SetCursorPosition(consoleWidth - 1, i);
                Console.Write(" ");
            }
        }

        public void drawBottomBorder()
        {
            for (int i = 0; i < consoleWidth; i++) {
                Console.SetCursorPosition(i, consoleHeight - 1);
                Console.Write(" ");
            }
        }

        #endregion
    }
}

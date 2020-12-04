using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Media;

namespace PacManV2._1
{
    class Game
    {
        Utilities util = new Utilities();
        public Board board = new Board();

        SoundPlayer waka1;
        SoundPlayer waka2;
        SoundPlayer readyMusic;
        SoundPlayer lostLife;

        private bool sound = true;

        private bool wakaState;

        private ConsoleColor black = ConsoleColor.Black;
        private ConsoleColor white = ConsoleColor.White;
        private ConsoleColor pacManCol = ConsoleColor.Yellow;
        private ConsoleColor ghostCol = ConsoleColor.Red;

        private int pacManX;
        private int pacManY;
        private char currentDirection;

        private int blinkyX;
        private int blinkyY;
        private char blinkyCurrentDirection;

        private int highScore;
        private int score;
        private int winScore;
        private int lives;

        private int pelletValue = 10;
        private int powerPelletValue = 50;

        private bool gameOver;

        public Game()
        {
            waka1 = new SoundPlayer(util.createPath("waka1.wav"));
            waka2 = new SoundPlayer(util.createPath("waka2.wav"));
            readyMusic = new SoundPlayer(util.createPath("ready.wav"));
            lostLife = new SoundPlayer(util.createPath("lostLife.wav"));
        }

        public void startGame(dynamic highScore)
        {
            wakaState = true;
            pacManX = board.getStartXFromFile();
            pacManY = board.getStartYFromFile();

            blinkyX = 13;
            blinkyY = 11;

            this.highScore = highScore;
            winScore = board.getMaxScoreFromFile();
            score = 0;
            lives = 3;
            gameOver = false;

            ConsoleKey key = ConsoleKey.A;

            board.printBoard();
            printLives();
            printScore();
            printHighScore();
            drawPacMan(pacManX, pacManY);

            ready();

            while (!gameOver) {
                if (Console.KeyAvailable) {
                    key = Console.ReadKey().Key;
                }
                
                switch (key) {
                    case ConsoleKey.UpArrow:
                        currentDirection = 'U';
                        break;
                    case ConsoleKey.LeftArrow:
                        currentDirection = 'L';
                        break;
                    case ConsoleKey.RightArrow:
                        currentDirection = 'R';
                        break;
                    case ConsoleKey.DownArrow:
                        currentDirection = 'D';
                        break;
                    case ConsoleKey.Enter:
                        loseLife();
                        break;
                }

                movePacMan(currentDirection);
                moveGhost();
                checkGhostHits();
                checkGameOver();

                System.Threading.Thread.Sleep(150);
            }

            //System.Threading.Thread.Sleep(2500);
            fancyClear();
        }

        public void setSound(bool state)
        {
            sound = state;
        }

        public void playWaka()
        {
            if (sound) {
                if (wakaState) {
                    waka1.Play();
                    wakaState = false;
                } else {
                    waka2.Play();
                    wakaState = true;
                }
            }
        }

        public void fancyClear()
        {
            util.setConsoleColours(black, black);
            for (int i = 0; i < Console.WindowHeight; i++) {
                for (int j = 0; j < Console.WindowWidth; j++) {
                    Console.SetCursorPosition(j, i);
                    Console.Write(" ");
                    System.Threading.Thread.Sleep(1);
                }
            }
        }

        public void checkGhostHits()
        {
            if ((pacManX == blinkyX) && (pacManY == blinkyY)) {
                loseLife();
            }
        }

        public void checkGameOver()
        {
            if ((score >= winScore) || (lives == 0)) {
                gameOver = true;
            } else {
                gameOver = false;
            }
        }

        public void ready()
        {
            printReady();

            if (sound) {
                readyMusic.Play();
            }

            System.Threading.Thread.Sleep(4300);
            deleteReady();
        }

        public void printReady()
        {
            Console.SetCursorPosition(25, 17 + board.getHeaderSize());
            util.setConsoleColours(pacManCol, black);
            Console.Write("Ready!");
        }

        public void deleteReady()
        {
            Console.SetCursorPosition(25, 17 + board.getHeaderSize());
            util.setConsoleColours(black, board.getBackgroundColour());
            Console.Write("      ");
        }

        public void printHighScore()
        {
            string highScoreString = "High score: " + highScore;
            Console.SetCursorPosition(((board.getBoardWidth() * 2) - 1) - highScoreString.Length, 0);
            util.setConsoleColours(white, black);
            Console.Write(highScoreString);
        }

        public void printScore()
        {
            Console.SetCursorPosition(1, 0);
            util.setConsoleColours(white, black);
            Console.Write("Score: " + score);
        }

        public void loseLife()
        {
            System.Threading.Thread.Sleep(1000);

            if (sound)
            {
                lostLife.Play();
            }

            System.Threading.Thread.Sleep(2000);

            lives -= 1;

            checkGameOver();

            if (!gameOver) {
                deletePacMan(pacManX, pacManY);

                pacManX = board.getStartXFromFile();
                pacManY = board.getStartYFromFile();

                drawPacMan(pacManX, pacManY);

                updateLives();

                checkGameOver();

                if (!gameOver) {
                    printReady();
                    System.Threading.Thread.Sleep(2000);
                    deleteReady();
                }
            }
        }

        public void updateLives()
        {
            deleteLives();
            printLives();
        }

        public void deleteLives()
        {
            Console.SetCursorPosition(2, board.getBoardHeight() + board.getHeaderSize() + 1);

            for (int i = 0; i < lives; i++)
            {
                util.setConsoleColours(black, black);
                Console.Write("   ");
            }
        }

        public void printLives()
        {
            Console.SetCursorPosition(2, board.getBoardHeight() + board.getHeaderSize() + 1);

            for (int i = 0; i < lives - 1; i++)
            {
                util.setConsoleColours(black, pacManCol);
                Console.Write("  ");
                util.setConsoleColours(black, black);
                Console.Write(" ");
            }
        }

        public void moveGhost()
        {
            deleteGhost(blinkyX, blinkyY);
            drawGhost(blinkyX, blinkyY);
        }

        public void deleteGhost(int x, int y)
        {
            Console.SetCursorPosition(x * 2, y + board.getHeaderSize());
            util.setConsoleColours(black, board.getBackgroundColour());
            Console.Write("  ");
            Console.SetCursorPosition(x * 2, y + board.getHeaderSize());
        }

        public void drawGhost(int x, int y)
        {
            Console.SetCursorPosition(x * 2, y + board.getHeaderSize());
            util.setConsoleColours(black, ghostCol);
            Console.Write("  ");
            Console.SetCursorPosition(x * 2, y + board.getHeaderSize());
        }

        public void movePacMan(char direction)
        {
            switch (direction)
            {
                case 'U':
                    moveLogic(pacManX, pacManY - 1);
                    break;
                case 'L':
                    moveLogic(pacManX - 1, pacManY);
                    break;
                case 'R':
                    moveLogic(pacManX + 1, pacManY);
                    break;
                case 'D':
                    moveLogic(pacManX, pacManY + 1);
                    break;
            }
        }

        public void moveLogic(int x, int y)
        {
            if ((pacManX == board.getWrapX1() && pacManY == board.getWrapY1()) || (pacManX == board.getWrapX2() && pacManY == board.getWrapY2())) {
                wrapLogic();
                playWaka();
            } else if (canMove(x, y)) {
                deletePacMan(pacManX, pacManY);
                pacManX += x - pacManX;
                pacManY += y - pacManY;
                drawPacMan(pacManX, pacManY);
                if (board.getPellet(pacManX, pacManY)) {
                    board.setPellet(pacManX, pacManY, false);
                    score += pelletValue;
                    printScore();
                } else if (board.getPowerPellet(pacManX, pacManY)) {
                    board.setPowerPellet(pacManX, pacManY, false);
                    score += powerPelletValue;
                    printScore();
                }
                playWaka();
            } else {
                deletePacMan(pacManX, pacManY);
                drawPacMan(pacManX, pacManY);
            }
        }

        public void wrapLogic()
        {
            deletePacMan(pacManX, pacManY);

            switch (currentDirection)
            {
                case 'U':
                    if (pacManX == board.getWrapX1() && pacManY == board.getWrapY1()) {
                        pacManY = board.getWrapY2();
                    } else {
                        pacManY -= 1;
                    }
                    break;
                case 'L':
                    if (pacManX == board.getWrapX1() && pacManY == board.getWrapY1()) {
                        pacManX = board.getWrapX2();
                    } else {
                        pacManX -= 1;
                    } 
                    break;
                case 'R':
                    if (pacManX == board.getWrapX2() && pacManY == board.getWrapY2()) {
                        pacManX = board.getWrapX1();
                    } else {
                        pacManX += 1;
                    } 
                    break;
                case 'D':
                    if (pacManX == board.getWrapX2() && pacManY == board.getWrapY2()) {
                        pacManY = board.getWrapY1();
                    } else {
                        pacManY += 1;
                    }
                    break;
            }

            drawPacMan(pacManX, pacManY);
            if (board.getPellet(pacManX, pacManY))
            {
                board.setPellet(pacManX, pacManY, false);
                score += 1;
                printScore();
            }
        }

        public bool canMove(int x, int y)
        {
            if (!(board.getTile(x, y) == true)) {
                return true;
            }

            return false;
        }

        public void deletePacMan(int x, int y)
        {
            Console.SetCursorPosition(x * 2, y + board.getHeaderSize());
            util.setConsoleColours(black, board.getBackgroundColour());
            Console.Write("  ");
            Console.SetCursorPosition(x * 2, y + board.getHeaderSize());
        }

        public void drawPacMan(int x, int y)
        {
            Console.SetCursorPosition(x * 2, y + board.getHeaderSize());
            util.setConsoleColours(black, pacManCol);
            Console.Write("  ");
            Console.SetCursorPosition(x * 2, y + board.getHeaderSize());
        }

        public int getScore()
        {
            return score;
        }
    }
}

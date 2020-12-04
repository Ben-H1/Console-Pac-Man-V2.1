using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacManV2._1
{
    class Board
    {
        Utilities util = new Utilities();

        private ConsoleColor fgCol = ConsoleColor.Blue;
        private ConsoleColor bgCol = ConsoleColor.Black;
        private ConsoleColor pelletCol = ConsoleColor.Yellow;

        private string boardPath;
        private int boardWidth;
        private int boardHeight;

        private int headerSize = 2;
        private int footerSize = 3;

        private bool[,] gameBoard;
        private bool[,] pellets;
        private bool[,] powerPellets;

        private char pelletChar = '.';
        private char powerPelletChar = '■';

        private int wrapX1;
        private int wrapY1;
        private int wrapX2;
        private int wrapY2;

        public Board()
        {
            this.boardPath = util.createPath("GameBoard.json");
            gameBoard = new bool[getBoardWidthFromFile(), getBoardHeightFromFile()];
            pellets = new bool[getBoardWidthFromFile(), getBoardHeightFromFile()];
            powerPellets = new bool[getBoardWidthFromFile(), getBoardHeightFromFile()];
        }

        public bool getTile(int x, int y)
        {
            if ((x < 0 || x > boardWidth) || (y < 0 || y > boardHeight)) {
                return false;
            }

            return gameBoard[x, y];
        }

        public bool getPellet(int x, int y)
        {
            if ((x < 0 || x > boardWidth) || (y < 0 || y > boardHeight)) {
                return false;
            }

            return pellets[x, y];
        }

        public void setPellet(int x, int y, bool state)
        {
            pellets[x, y] = state;
        }

        public bool getPowerPellet(int x, int y)
        {
            if ((x < 0 || x > boardWidth) || (y < 0 || y > boardHeight)) {
                return false;
            }

            return powerPellets[x, y];
        }

        public void setPowerPellet(int x, int y, bool state)
        {
            powerPellets[x, y] = state;
        }

        public int getMaxScoreFromFile()
        {
            dynamic board = util.readFile(boardPath);
            return board[0].maxScore.maxScore;
        }

        public int getStartXFromFile()
        {
            dynamic board = util.readFile(boardPath);
            return board[0].start.startX;
        }

        public int getStartYFromFile()
        {
            dynamic board = util.readFile(boardPath);
            return board[0].start.startY;
        }

        public int getWrapX1()
        {
            return wrapX1;
        }

        public int getWrapY1()
        {
            return wrapY1;
        }

        public int getWrapX2()
        {
            return wrapX2;
        }

        public int getWrapY2()
        {
            return wrapY2;
        }

        public int getHeaderSize()
        {
            return headerSize;
        }

        public int getFooterSize()
        {
            return footerSize;
        }

        public int getBoardWidth()
        {
            return boardWidth;
        }

        public int getBoardHeight()
        {
            return boardHeight;
        }

        public ConsoleColor getForegroundColour()
        {
            return fgCol;
        }

        public ConsoleColor getBackgroundColour()
        {
            return bgCol;
        }

        public void setForegroundColour(ConsoleColor col)
        {
            fgCol = col;
        }

        public void setBackgroundColour(ConsoleColor col)
        {
            bgCol = col;
        }

        public void setPelletCharacter(char character)
        {
            pelletChar = character;
        }

        public void setPowerPelletCharacter(char character)
        {
            powerPelletChar = character;
        }

        public int getBoardWidthFromFile()
        {
            dynamic board = util.readFile(boardPath);
            return board[0].dimensions.width;
        }

        public int getBoardHeightFromFile()
        {
            dynamic board = util.readFile(boardPath);
            return board[0].dimensions.height;
        }

        public void setUpBoard()
        {
            dynamic board = util.readFile(boardPath);
            boardWidth = board[0].dimensions.width;
            boardHeight = board[0].dimensions.height;
            wrapX1 = board[0].wrap.wrapX1;
            wrapY1 = board[0].wrap.wrapY1;
            wrapX2 = board[0].wrap.wrapX2;
            wrapY2 = board[0].wrap.wrapY2;

            int i = 0;
            foreach (dynamic line in board[0].board) {
                int j = 0;
                foreach (bool tile in line) {
                    gameBoard[j, i] = tile;
                    j++;
                }
                i++;
            }

            int k = 0;
            foreach (dynamic line in board[0].pellets)
            {
                int j = 0;
                foreach (bool tile in line)
                {
                    pellets[j, k] = tile;
                    j++;
                }
                k++;
            }

            int n = 0;
            foreach (dynamic line in board[0].powerPellets)
            {
                int j = 0;
                foreach (bool tile in line)
                {
                    powerPellets[j, n] = tile;
                    j++;
                }
                n++;
            }
        }

        public void printBoard()
        {
            setUpBoard();
            Console.Clear();
            util.setConsoleDimensions(boardWidth * 2, boardHeight + headerSize + footerSize);

            for (int i = 0; i < headerSize; i++) {
                Console.WriteLine();
            }

            for (int i = 0; i < boardHeight; i++) {
                for (int j = 0; j < boardWidth; j++) {
                    if (gameBoard[j, i]) {
                        util.setConsoleColours(bgCol, fgCol);
                        Console.Write("  ");
                    } else if (pellets[j, i]) {
                        util.setConsoleColours(pelletCol, bgCol);
                        Console.Write(pelletChar + " ");
                    } else if (powerPellets[j, i]) {
                        util.setConsoleColours(pelletCol, bgCol);
                        Console.Write(powerPelletChar + " ");
                    } else {
                        util.setConsoleColours(fgCol, bgCol);
                        Console.Write("  ");
                    }
                }
                Console.WriteLine();
            }

            for (int i = 0; i < footerSize; i++) {
                Console.WriteLine();
            }

            util.resetCursorPosition();
        }
    }
}

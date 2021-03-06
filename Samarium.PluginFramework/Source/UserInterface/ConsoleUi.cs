﻿
using System;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace Samarium.PluginFramework.UI {
    public static class ConsoleUI {
        public static int ConsoleWidth {
            get {
                return Console.WindowWidth;
            }
        }

        public static int ConsoleHeight {
            get {
                return Console.WindowHeight;
            }
        }

        internal static int GetConsoleCentre(string text) {
            return ConsoleWidth / 2 - text.Length / 2;
        }

        public static void SetCursorPosition(Coord coords) {
            Console.SetCursorPosition(coords.x, coords.y);
        }

        public static Coord GetCoordinates(int x, int y) {
            return new Coord() { x = x, y = y };
        }

        public static void PrintLeft(string text, int yCoord) {
            SetCursorPosition(GetCoordinates(0, yCoord));
            Console.Write(text);
        }

        public static void PrintCentre(string text, int yCoord) {
            SetCursorPosition(GetCoordinates(GetConsoleCentre(text), yCoord));
            Console.Write(text);
        }

        public static void PrintCentre(string text, int maxXCoord, int yCoord, ConsoleColor textBackground = ConsoleColor.Black, ConsoleColor textForeground = ConsoleColor.White) {
            ConsoleColor backgroundColor = Console.BackgroundColor;
            ConsoleColor foregroundColor = Console.ForegroundColor;
            Console.BackgroundColor = textBackground;
            Console.ForegroundColor = textForeground;
            if (!text.Contains("\n")) {
                Print(maxXCoord / 2 - text.Length / 2, yCoord, text);
            } else {
                string str1 = text;
                char[] chArray = new char[1] { '\n' };
                foreach (string str2 in str1.Split(chArray))
                    Print(maxXCoord / 2 - str2.Length / 2, yCoord++, text);
            }
            Console.BackgroundColor = backgroundColor;
            Console.ForegroundColor = textForeground;
        }

        public static void PrintCentre(string text, int maxXCoord, int yCoord, bool clearArea, ConsoleColor textBackground = ConsoleColor.Black, ConsoleColor textForeground = ConsoleColor.White) {
            ConsoleColor backgroundColor = Console.BackgroundColor;
            ConsoleColor foregroundColor = Console.ForegroundColor;
            Console.BackgroundColor = textBackground;
            Console.ForegroundColor = textForeground;
            if (!text.Contains("\n")) {
                if (clearArea) {
                    StringBuilder stringBuilder = new StringBuilder().Append(' ', text.Length);
                    Print(maxXCoord / 2 - text.Length / 2, yCoord, stringBuilder.ToString());
                }
                Print(maxXCoord / 2 - text.Length / 2, yCoord, text);
            } else {
                string str1 = text;
                char[] chArray = new char[1] { '\n' };
                foreach (string str2 in str1.Split(chArray)) {
                    if (clearArea) {
                        StringBuilder stringBuilder = new StringBuilder().Append(' ', str2.Length);
                        Print(maxXCoord / 2 - str2.Length / 2, yCoord, stringBuilder.ToString());
                    }
                    Print(maxXCoord / 2 - str2.Length / 2, ++yCoord, str2.Replace("\n", ""));
                }
            }
            Console.BackgroundColor = backgroundColor;
            Console.ForegroundColor = textForeground;
        }

        public static void PrintRight(string text, int yCoord, int offset = 0) {
            SetCursorPosition(GetCoordinates(ConsoleWidth - text.Length - offset, yCoord));
            Console.Write(text);
        }

        public static void PrintCentre(string text) {
            var splitString = text.Split('\r', '\n');
            var divident = ConsoleHeight / 2 - splitString.Length / 2;
            for (var i = 0; i < splitString.Length; i++) {
                SetCursorPosition(GetCoordinates(GetConsoleCentre(splitString[i]), divident + i));
                Console.Write(text);
            }
        }

        public static void CinematicPrint(string text, Coord coords, int timeDelayInMs = 50) {
            SetCursorPosition(coords);
            foreach (char ch in text.ToCharArray()) {
                Console.Write(ch);
                Thread.Sleep(timeDelayInMs);
            }
        }

        public static void Print(int x, int y, string text) {
            if (!text.Contains("\n")) {
                SetCursorPosition(new Coord() {
                    x = x,
                    y = y
                });
                Console.Write(text);
            } else {
                string str1 = text;
                char[] chArray = new char[1] { '\n' };
                foreach (string str2 in str1.Split(chArray)) {
                    SetCursorPosition(GetCoordinates(x, y++));
                    Console.Write(str2);
                }
            }
        }

        public static void PrintFullScreenBorder(BorderType borderType = BorderType.DoubleBorder, string title = "", string version = "") {
            Coord coords;
            for (int index1 = 0; index1 < ConsoleHeight; ++index1) {
                if (index1 == 0 || index1 == ConsoleHeight - 1) {
                    for (int index2 = 0; index2 < ConsoleWidth; ++index2) {
                        coords = new Coord {
                            x = index2,
                            y = index1,
                        };
                        SetCursorPosition(coords);
                        Console.Write(GetBorderChar(borderType, CharType.Horizontal));
                    }
                    if (index1 == 0) {
                        coords = new Coord {
                            x = 0,
                            y = index1
                        };
                        SetCursorPosition(coords);
                        Console.Write(GetBorderChar(borderType, CharType.TopLeftCorner));
                        coords = new Coord {
                            x = ConsoleWidth - 1,
                            y = index1
                        };
                        SetCursorPosition(coords);
                        Console.Write(GetBorderChar(borderType, CharType.TopRightCorner));
                    } else if (index1 == ConsoleHeight - 1) {
                        coords = new Coord {
                            x = 0,
                            y = index1
                        };
                        SetCursorPosition(coords);
                        Console.Write(GetBorderChar(borderType, CharType.BottomLeftCorner));
                        coords = new Coord {
                            x = ConsoleWidth - 1,
                            y = index1
                        };
                        SetCursorPosition(coords);
                        Console.Write(GetBorderChar(borderType, CharType.BottomRightCorner));
                    }
                } else {
                    coords = new Coord {
                        x = 0,
                        y = index1
                    };
                    SetCursorPosition(coords);
                    Console.Write(GetBorderChar(borderType, CharType.Vertical));
                    coords = new Coord {
                        x = ConsoleWidth - 1,
                        y = index1
                    };
                    SetCursorPosition(coords);
                    Console.Write(GetBorderChar(borderType, CharType.Vertical));
                }
            }
            if (!string.IsNullOrEmpty(title))
                PrintCentre(title, 0);
            if (string.IsNullOrEmpty(version))
                return;
            PrintRight(version, 0, 3);
        }

        public static void PrintTopTitle(string title, string version = "") {
            if (!string.IsNullOrEmpty(title))
                PrintCentre(title, 0);
            if (!string.IsNullOrEmpty(version))
                PrintRight(version, 0, 3);
        }

        public static void PrintBox(uint startX, uint startY, uint width, uint height, BorderType borderType = BorderType.DoubleBorder, CharType topLeftCorner = CharType.TopLeftCorner, CharType topRightCorner = CharType.TopRightCorner, CharType bottomLeftCorner = CharType.BottomLeftCorner, CharType bottomRightCorner = CharType.BottomRightCorner, string title = "") {
            for (int index = 0; (long)index < (long)height; ++index) {
                SetCursorPosition(GetCoordinates((int)startX, index + (int)startY));
                Console.Write(GetBorderChar(borderType, CharType.Vertical));
                SetCursorPosition(GetCoordinates((int)startX + (int)width, index + (int)startY));
                Console.Write(GetBorderChar(borderType, CharType.Vertical));
            }
            for (int index = 0; (long)index < (long)width; ++index) {
                SetCursorPosition(GetCoordinates((int)startX + index, (int)startY));
                Console.Write(GetBorderChar(borderType, CharType.Horizontal));
                SetCursorPosition(GetCoordinates((int)startX + index, (int)startY + (int)height - 1));
                Console.Write(GetBorderChar(borderType, CharType.Horizontal));
            }
            SetCursorPosition(GetCoordinates((int)startX, (int)startY));
            Console.Write(GetBorderChar(borderType, topLeftCorner));
            SetCursorPosition(GetCoordinates((int)startX, (int)startY + (int)height - 1));
            Console.Write(GetBorderChar(borderType, bottomLeftCorner));
            SetCursorPosition(GetCoordinates((int)startX + (int)width, (int)startY));
            Console.Write(GetBorderChar(borderType, topRightCorner));
            SetCursorPosition(GetCoordinates((int)startX + (int)width, (int)startY + (int)height - 1));
            Console.Write(GetBorderChar(borderType, bottomRightCorner));
            if (string.IsNullOrEmpty(title))
                return;
            PrintCentre(title, (int)width, (int)startY, ConsoleColor.Black, ConsoleColor.White);
        }

        public static char GetBorderChar(BorderType borderType, CharType charType) {
            switch (charType) {
                case CharType.TopLeftCorner:
                    switch (borderType) {
                        case BorderType.SingleThinBorder:
                            return '┌';
                        case BorderType.SingleThickBorder:
                            return '┏';
                        case BorderType.SingleToDoubleBorder:
                            return '╒';
                        case BorderType.DoubleBorder:
                            return '╔';
                        case BorderType.DoubleToSingleBorder:
                            return '╓';
                        default:
                            return '╔';
                    }
                case CharType.Horizontal:
                    switch (borderType) {
                        case BorderType.SingleThinBorder:
                            return '─';
                        case BorderType.SingleThickBorder:
                            return '━';
                        case BorderType.SingleToDoubleBorder:
                            return '═';
                        case BorderType.DoubleBorder:
                            return '═';
                        case BorderType.DoubleToSingleBorder:
                            return '─';
                        default:
                            return '═';
                    }
                case CharType.TopRightCorner:
                    switch (borderType) {
                        case BorderType.SingleThinBorder:
                            return '┐';
                        case BorderType.SingleThickBorder:
                            return '┓';
                        case BorderType.SingleToDoubleBorder:
                            return '╕';
                        case BorderType.DoubleBorder:
                            return '╗';
                        case BorderType.DoubleToSingleBorder:
                            return '╖';
                        default:
                            return '╗';
                    }
                case CharType.LeftT:
                    switch (borderType) {
                        case BorderType.SingleThinBorder:
                            return '├';
                        case BorderType.SingleThickBorder:
                            return '┣';
                        case BorderType.SingleToDoubleBorder:
                            return '╞';
                        case BorderType.DoubleBorder:
                            return '╠';
                        case BorderType.DoubleToSingleBorder:
                            return '╟';
                        default:
                            return '╠';
                    }
                case CharType.CentreCross:
                    switch (borderType) {
                        case BorderType.SingleThinBorder:
                            return '┼';
                        case BorderType.SingleThickBorder:
                            return '╋';
                        case BorderType.SingleToDoubleBorder:
                            return '╪';
                        case BorderType.DoubleBorder:
                            return '╬';
                        case BorderType.DoubleToSingleBorder:
                            return '╫';
                        default:
                            return '╬';
                    }
                case CharType.RightT:
                    switch (borderType) {
                        case BorderType.SingleThinBorder:
                            return '┤';
                        case BorderType.SingleThickBorder:
                            return '┫';
                        case BorderType.SingleToDoubleBorder:
                            return '╡';
                        case BorderType.DoubleBorder:
                            return '╣';
                        case BorderType.DoubleToSingleBorder:
                            return '╢';
                        default:
                            return '╣';
                    }
                case CharType.Vertical:
                    switch (borderType) {
                        case BorderType.SingleThinBorder:
                            return '│';
                        case BorderType.SingleThickBorder:
                            return '┃';
                        case BorderType.SingleToDoubleBorder:
                            return '│';
                        case BorderType.DoubleBorder:
                            return '║';
                        case BorderType.DoubleToSingleBorder:
                            return '║';
                        default:
                            return '║';
                    }
                case CharType.BottomLeftCorner:
                    switch (borderType) {
                        case BorderType.SingleThinBorder:
                            return '└';
                        case BorderType.SingleThickBorder:
                            return '┗';
                        case BorderType.SingleToDoubleBorder:
                            return '╘';
                        case BorderType.DoubleBorder:
                            return '╚';
                        case BorderType.DoubleToSingleBorder:
                            return '╙';
                        default:
                            return '╚';
                    }
                case CharType.BottomRightCorner:
                    switch (borderType) {
                        case BorderType.SingleThinBorder:
                            return '┘';
                        case BorderType.SingleThickBorder:
                            return '┛';
                        case BorderType.SingleToDoubleBorder:
                            return '╛';
                        case BorderType.DoubleBorder:
                            return '╝';
                        case BorderType.DoubleToSingleBorder:
                            return '╜';
                        default:
                            return '╝';
                    }
                case CharType.BottomT:
                    switch (borderType) {
                        case BorderType.SingleThinBorder:
                            return '┴';
                        case BorderType.SingleThickBorder:
                            return '┻';
                        case BorderType.SingleToDoubleBorder:
                            return '╧';
                        case BorderType.DoubleBorder:
                            return '╩';
                        case BorderType.DoubleToSingleBorder:
                            return '╨';
                        default:
                            return '╩';
                    }
                case CharType.TopT:
                    switch (borderType) {
                        case BorderType.SingleThinBorder:
                            return '┬';
                        case BorderType.SingleThickBorder:
                            return '┳';
                        case BorderType.SingleToDoubleBorder:
                            return '╤';
                        case BorderType.DoubleBorder:
                            return '╦';
                        case BorderType.DoubleToSingleBorder:
                            return '╥';
                        default:
                            return '╦';
                    }
                default:
                    return ' ';
            }
        }

        public static void Clear(uint yCoord) {
            StringBuilder stringBuilder = new StringBuilder().Append(' ', ConsoleWidth - 1);
            Print(0, (int)yCoord, stringBuilder.ToString());
        }

        public static void Clear(uint xCoordStart, uint xCoordEnd, uint yCoordStart, uint yCoordEnd) {
            for (uint index1 = yCoordStart; index1 < yCoordEnd; ++index1) {
                for (uint index2 = xCoordStart; index2 < xCoordEnd; ++index2)
                    Print((int)index2, (int)index1, " ");
            }
        }

        public static void PrintProgressBar(ProgressStyle style, uint width, int xCoord, uint yCoord, int value, bool indeterminate = false) {
            try {
                if (value < 0 || (long)value >= (long)width)
                    return;
                Coord coordinates = GetCoordinates(Console.CursorLeft, Console.CursorTop);
                int num = (int)width - 2 - value;
                StringBuilder stringBuilder = new StringBuilder().Append('[').Append((char)style, value);
                if (num > 0)
                    stringBuilder.Append(' ', (int)width - 2 - value);
                stringBuilder.Append(']');
                if (xCoord < 0)
                    PrintCentre(stringBuilder.ToString(), (int)yCoord);
                else
                    Print(xCoord, (int)yCoord, stringBuilder.ToString());
                SetCursorPosition(coordinates);
            } catch {
            }
        }

        internal static void OnPropertyChanged(string propertyName, object caller = null) {
            caller.GetType().GetProperty(propertyName).GetValue(caller);
            Debug.WriteLine("A property has changed!\n\tInformation:\n\t\tProperty name: {propertyName}\n\t\tProperty value (data): {propValue}\n\t\tCaller: {nameof(caller)}");
        }
    }
}

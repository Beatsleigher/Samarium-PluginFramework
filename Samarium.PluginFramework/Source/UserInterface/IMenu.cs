
using System;
using System.Collections.Generic;

namespace Samarium.PluginFramework.UI.Menus {
    public interface IMenu {
        string Title { get; set; }

        string[] MenuItems { get; set; }

        int SelectedMenuIndex { get; set; }

        string[] LeftDescription { get; set; }

        string[] RightDescription { get; set; }

        string BottomText { get; set; }

        ConsoleKey NextKey { get; set; }

        ConsoleKey PreviousKey { get; set; }

        ConsoleKey SelectionKey { get; set; }

        Dictionary<ConsoleKey, Action> ActionKeys { get; set; }

        void PrintMenu();

        void PrintMenuItems();

        void PrintMenuTitle();

        void PrintLeftDescription();

        void PrintRightDescription();

        void PrintBottomText();

        int HandOverToUser();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MazeGenerator;
using Menu;
using ProfileClasses;
namespace Examv2
{
    class Program
    {
        static void Main(string[] args)
        {
            Model datas = new Model();
            Profile CurrentProfile = new Profile();
            _Maze maze = new _Maze();

            _Menu startMenu = new _Menu();
            startMenu.AddMenuIndex(0, "Create new profile");
            startMenu.AddMenuIndex(1, "Already have one");
            startMenu.AddMenuIndex(2, "Exit");

            do
            {
                startMenu.Start();
                if (startMenu.Choice == 2)
                    return;
                switch(startMenu.Choice)
                {
                    case 0:
                        Profile player = new Profile();

                        Console.WriteLine("Enter name");
                        player.Name = Console.ReadLine();

                        //Add to file
                        datas.AddProfile(player);
                        break;
                    case 1:
                        datas.ShowAll();
                        Console.WriteLine("Enter name");
                        string PlayerName = Console.ReadLine();

                        CurrentProfile = datas.FindProfile(PlayerName);
                        break;

                }
            } while (startMenu.AllowContinue());
            

             #region Menu
            _Menu gameMenu = new _Menu();
           
            gameMenu.AddMenuIndex(1, "Play");
            gameMenu.AddMenuIndex(2, "Maze Settings");
            _Menu subgameMenu = new _Menu();
            subgameMenu.AddMenuIndex(1, "Set Maze Size");
            subgameMenu.AddMenuIndex(2, "Spawn Frogs");
            subgameMenu.AddMenuIndex(3, "Back");

            gameMenu.AddMenuIndex(3, "View Players Profiles");
            //search by name???
            gameMenu.AddMenuIndex(4, "Edit Profile Name");
            gameMenu.AddMenuIndex(5, "Change Profile");
            gameMenu.AddMenuIndex(6, "High Scores");
            gameMenu.AddMenuIndex(7, "Exit");
            #endregion

            do
            {
                gameMenu.Start();
                if (gameMenu.Choice == 7)
                { break; }
                switch (gameMenu.Choice)
                {
                    case 1:
                        Console.Clear();
                        Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);
                        maze.Generate();
                        Console.SetCursorPosition(0, Console.LargestWindowHeight - 1);
                        Console.ReadKey();
                        break;
                    
                    case 2:
                        do
                        {
                            subgameMenu.Start();

                            if (subgameMenu.Choice == 3)
                                break;

                            switch (subgameMenu.Choice)
                            {
                                case 1:
                                    //check for values
                                    int Height, Width;
                                    Console.WriteLine("Enter Height");
                                    Height = Convert.ToInt32(Console.ReadLine());

                                    Console.WriteLine("Enter Width");
                                    Width = Convert.ToInt32(Console.ReadLine());
                                    maze.size = new IntVector2(Height, Width);
                                    break;
                                case 2:
                                    Console.WriteLine("1 to spawn frog in the maze");
                                    int frog = Convert.ToInt32(Console.ReadLine());
                                    if (frog == 1)
                                        maze.Frogs = true;
                                    break;
                            }
                        }
                        while (subgameMenu.AllowContinue());
                        break;
                    case 3:
                        datas.ShowAll();
                        break;
                    case 4:
                        Console.WriteLine("Enter new name");
                        string newname = Console.ReadLine();

                        datas.EditProfile(CurrentProfile.Name, newname);

                        break;
                    case 5:
                        datas.ShowAll();
                        Console.WriteLine("Enter new profile name");
                        string PlayerName = Console.ReadLine();

                        CurrentProfile = datas.FindProfile(PlayerName);
                        break;
                    case 6:
                        /*
                         * find all
                         * sort 
                         */
                        break;
                }


            } while (gameMenu.AllowContinue());

            //    Console.Clear();
            //    ////Console.SetWindowSize(170, 170);
            //    Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);
            //    _Maze m = new _Maze();
            //    m.Generate();
            //    ////Console.SetWindowSize(189, 189);
            //    Console.ReadKey();
            //    ////System.Threading.Thread.Sleep(5000);
            //    Console.SetCursorPosition(0, Console.LargestWindowHeight - 1);
            //}
        }
    }
}

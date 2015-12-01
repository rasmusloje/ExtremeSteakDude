﻿using ExtremeSteakDude.Commands;
using ExtremeSteakDude.Model;
using ExtremeSteakDude.Serialization;
using GalaSoft.MvvmLight;
using System.Windows.Media.Imaging;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using System;
using ExtremeSteakDude.Levels;
using ExtremeSteakDude.View;
using System.Media;
using System.Windows.Media;
using System.Collections;
using System.Windows.Controls;

namespace ExtremeSteakDude.ViewModel
{

    public class MainViewModel : ViewModelBase
    {

        /// <summary>
        /// The <see cref="WelcomeTitle" /> property's name.
        /// </summary>
        public const string WelcomeTitlePropertyName = "WelcomeTitle";

        public ObservableCollection<Model.HighScores> highScores { get; set; }

        private MovementController mc;
        public ObservableCollection<Player> players { get; set; }
        public Player player;
        private BitmapImage map;
        public ICommand KeyDownCommand { get; }
        public ICommand KeyUpCommand { get; }

        public ICommand ContinueCommand { get; }
        public ICommand NewGameCommand { get; }

        public ICommand HighScoreCommand { get; }
        public ICommand ExitCommand { get; }

        public ICommand SaveHighScoreCommand { get; }
        public ICommand SavePlayerCommand { get; }

        public ICommand MainMenuCommand { get; }

        public ICommand NewPlayerCommandLVL1 { get; }
        public ICommand NewPlayerCommandLVL2 { get; }
        public ICommand LoadPlayerCommand { get; }

        private string _welcomeTitle = string.Empty;
        private bool canJump = true;

        public string name {get; set;}

        /// <summary>
        /// Gets the WelcomeTitle property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        /// 

       


        public string WelcomeTitle
        {
            get
            {
                return _welcomeTitle;
            }
            set
            {
                _welcomeTitle = value;
            }
        }

        public MainViewModel() {
            name = "Erir";

            XML xml = new XML();
            players = new ObservableCollection<Player>();
            player = xml.Player;
            players.Add(player);
            
            
            KeyDownCommand = new RelayCommand<KeyEventArgs>(KeyDown);
            KeyUpCommand = new RelayCommand<KeyEventArgs>(KeyUp);

            
            highScores = new ObservableCollection<Model.HighScores>();
            highScores.Add(xml.HighScores);
            mc = new MovementController(players, highScores);
            NewGameCommand = new RelayCommand(NewGame);
            ContinueCommand = new RelayCommand(Continue);
            HighScoreCommand = new RelayCommand(HighScore);
            ExitCommand = new RelayCommand(Exit);
            SaveHighScoreCommand = new RelayCommand<String>(SaveHighScore);
            MainMenuCommand = new RelayCommand(MainMenuC);
            SavePlayerCommand = new RelayCommand(SavePlayer);
            NewPlayerCommandLVL1 = new RelayCommand(NewPlayerLVL1);
            NewPlayerCommandLVL2 = new RelayCommand(NewPlayerLVL2);
        }
        private void MainMenuC()
        {
            MainMenuCommand Command = new MainMenuCommand();
            Command.Execute();

        }
        private void NewGame()
        {
            NewGameCommand Command = new NewGameCommand();
            Command.Execute();
        }

        private void Continue()
        {
            ContinueCommand Command = new ContinueCommand(players, new XML());
            Command.Execute();
        }

        private void HighScore()
        {
            HighScoreCommand Command = new HighScoreCommand();
            Command.Execute();
        }

        private void Exit()
        {
            ExitCommand Command = new ExitCommand();
            Command.Execute();
        }

        private void NewPlayerLVL1()
        {
            NewPlayerCommandLVL1 Command = new NewPlayerCommandLVL1(players);
            Command.Execute();
        }

        private void NewPlayerLVL2()
        {
            NewPlayerCommandLVL2 Command = new NewPlayerCommandLVL2(players);
            Command.Execute();
        }
        private void SavePlayer()
        {
            SavePlayerCommand Command = new SavePlayerCommand(players, new XML());
            Command.Execute();
        }

        private void SaveHighScore(string obj)
        {
            SaveHighScoreCommand Command = new SaveHighScoreCommand(highScores, new XML(), obj, highScores[0].Score);
            Command.Execute();
            NewGame();
        }

        private void KeyDown(KeyEventArgs e)
        {
            
            switch (e.Key)
            {
                case Key.Left:
                    mc.moveLeft = true;
                    //BitmapImage bm1 = new BitmapImage(new Uri(player.MeatboyImageInvert, UriKind.RelativeOrAbsolute));
                    players[0].meatboyImage = player.MeatboyImageLeft;
                    break;
                case Key.Right:
                    mc.moveRight = true;
                    players[0].meatboyImage = player.MeatboyImageRight;
                    break;
                case Key.Space:
                    if (canJump)
                    {
                        mc.jump = true;
                        canJump = false;
                    }
                    
                    // For jump animation
                    //BitmapImage bm = new BitmapImage(new Uri(player.MeatboyImageJump, UriKind.RelativeOrAbsolute));
                    players[0].meatboyImage = player.MeatboyImageJump;
                    break;
                case Key.Z:
                    mc.isUndoMode = !mc.isUndoMode;
                    break;
                case Key.Escape:
                    // Save the game
                    // Save player position, timer, level, momemtum
                    SavePlayer();

                    // Go to main menu
                    //mc.isUndoMode = true;
                    var main = App.Current.MainWindow as MainWindow;
                    MainMenu mm = new MainMenu();
                    main.Content = mm;
                    break;
            }
        }

        private void KeyUp(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left:
                    mc.moveLeft = false;
                    players[0].meatboyImage = player.MeatboyImageLeft;
                    break;
                case Key.Right:
                    mc.moveRight = false;
                    players[0].meatboyImage = player.MeatboyImageRight;
                    break;
                case Key.Space:
                    mc.jump = false;
                    canJump = true;
                    // For jump animation
                    players[0].meatboyImage = player.MeatboyImageFront;
                    break;
            }
        }
        



    }
}
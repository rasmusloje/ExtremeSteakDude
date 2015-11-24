﻿using ExtremeSteakDude.Model;
using ExtremeSteakDude.Serialization;
using GalaSoft.MvvmLight;
using System;

namespace ExtremeSteakDude.ViewModel
{

    public class MainViewModel : ViewModelBase
    {

        /// <summary>
        /// The <see cref="WelcomeTitle" /> property's name.
        /// </summary>
        public const string WelcomeTitlePropertyName = "WelcomeTitle";

        public HighScores highScores { get; set; }

        private string _welcomeTitle = string.Empty;

        /// <summary>
        /// Gets the WelcomeTitle property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
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
            



            XML xml = new XML();
            highScores = xml.HighScores;
        }
    }
}
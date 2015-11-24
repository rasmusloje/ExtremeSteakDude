﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ExtremeSteakDude.Model;
using ExtremeSteakDude.Serialization;

namespace ExtremeSteakDude.Commands
{
    class SaveHighScoreCommand : IUndoRedoCommand
    {
        private XML xML;
        private String name;
        private int score;
        private ObservableCollection<HighScores> highScores;
        private int v;


        public SaveHighScoreCommand(ObservableCollection<HighScores> highScores, XML xML, string name, int v)
        {
            this.highScores = highScores;
            this.xML = xML;
            this.name = name;
            this.v = v;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute()
        {
            highScores[0].Name = name;
            highScores[0].Score = score;
            xML.HighScores = highScores[0];
        }

        public void Undo()
        {
            throw new NotImplementedException();
        }
    }
}

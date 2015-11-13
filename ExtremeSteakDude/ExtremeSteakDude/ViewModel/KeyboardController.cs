﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ExtremeSteakDude.ViewModel
{
    class KeyboardController
    {
        MovementController mc = new MovementController();

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left: mc.moveLeft = true; break;
                case Key.Right: mc.moveRight = true; break;
                case Key.Up: mc.jump = true; break;
            }
        }

        private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left: mc.moveLeft = false; break;
                case Key.Right: mc.moveRight = false; break;
                case Key.Up: mc.jump = false; break;
            }
        }
    }
}
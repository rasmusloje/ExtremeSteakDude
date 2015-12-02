﻿using ExtremeSteakDude.Commands;
using ExtremeSteakDude.Levels;
using ExtremeSteakDude.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace ExtremeSteakDude.ViewModel
{
    class MovementController : IDisposable
    {
        public bool moveRight { get; set; }
        public bool moveLeft { get; set; }
        public bool jump { get; set; }
        public bool isUndoMode { get; set; }
        public bool pause { get; set; }
        public bool unpause { get; set; }
        public bool first { get; set; } = true;

        public ObservableCollection<Player> p;
        private Timer moveTimer;
        private UndoRedoController urc;
        private System.Diagnostics.Stopwatch timer;

        private int movespeed = 5;
        private int moveacc = 2;
        private int fallspeed = 5;
        private int gravity = 1;
        private int jumpheight = 17;
        private int tick = 12;

        private MapNew currentlvl;
        private CollisionDetector coll;
        private TimeSpan offset;
        private CDC cdc;
        private SoundController sc;
        private ObservableCollection<Model.HighScores> highScores;
        
        private MainViewModel mwm;


        public MovementController(ObservableCollection<Player> p, ObservableCollection<Model.HighScores> highScores, MainViewModel main)
        {
            if (Player.level == Player.levelenum.one)
            {
                currentlvl = new lvl2();
                Console.WriteLine("LEVEL ONE IN MC");
            }

            if (Player.level == Player.levelenum.two)
            {
                currentlvl = new lvl2();
                Console.WriteLine("LEVEL TWO IN MC");
            }


            this.p = p;
            p[0].alive = true;
            p[0].won = false;
            urc = new UndoRedoController();
            timer = new System.Diagnostics.Stopwatch();
            moveTimer = new Timer(x => Move(), null, 0, tick);
            sc = new SoundController();
            //coll = new CollisionDetector(this, currentlvl);
            cdc = new CDC(this, currentlvl);
            mwm = main;
            this.highScores = highScores;
        }

        private void Move()
        {
            if (first && !jump && !moveRight && !moveLeft)
            {
                return;
            }
            if (first)
            {
                offset.Subtract(offset);
                timer.Restart();
                first = false;
            }
            if (isUndoMode)
            {
                timer.Stop();
                if (moveLeft)
                {
                    urc.Undo();

                }
                else if (moveRight)
                {
                    urc.Redo();
                }
            }
            else if(pause)
            {
                timer.Stop();
                if (unpause)
                {
                    pause = false;
                    unpause = false;
                    timer.Start();
            }

            }

            else
            {


                if (timer.IsRunning == false)
                {
                    offset = p[0].timeSpan;
                    timer.Restart();
                }

                if (moveLeft && !moveRight && !p[0].onWallLeft && !( p[0].onWallRight && p[0].inAir) && p[0].vx > -movespeed)
                {
                    MoveLeft();

                }
                else if (moveRight && !moveLeft && !p[0].onWallRight && !( p[0].onWallLeft && p[0].inAir) && p[0].vx < movespeed)
                {
                    MoveRight();
                }
                else
                {
                    if (p[0].vx > 0)
                    {
                        if (p[0].onWallRight) { p[0].vx = 0; }
                        else if (p[0].vx > moveacc + 1 && !p[0].onWallRight)
                        {
                            p[0].vx = p[0].vx - moveacc;
                        }
                        else
                        {
                            p[0].vx = 0;
                        }
                    }
                    else if (p[0].vx < -moveacc)
                    {
                        if (p[0].onWallLeft) { p[0].vx = 0; }
                        else { p[0].vx = p[0].vx + moveacc; }
                    }
                    else
                    {
                        p[0].vx = 0;
                    }
                }
                if (p[0].top)
                {
                    if (p[0].vy > 0)
                        p[0].vy = 0;
                }
                if (p[0].inAir && (p[0].onWallLeft || p[0].onWallRight))
                {
                    if (jump)
                    {
                        Jump();
                    }
                    else if (p[0].vy < fallspeed / 3)
                    {
                        if (15 - p[0].vy >= gravity)
                        {
                            p[0].vy = p[0].vy + gravity;
                        }
                        else
                        {
                            p[0].vy = fallspeed / 3;
                        }
                    }
                }
                else if (p[0].inAir)
                {
                    if (p[0].vy < fallspeed)
                    {
                        if (15 - p[0].vy >= gravity)
                        {
                            p[0].vy = p[0].vy + gravity;
                        }
                        else
                        {
                            p[0].vy = fallspeed;
                        }
                    }
                }
                else if (jump)
                {
                    Jump();
                }
                else
                {
                    p[0].vy = 0;
                }
                cdc.check();
                CheckWinDeath();
                urc.AddAndExecute(new MomentumCommand(p, p[0].vx, p[0].vy, timer.Elapsed + offset));
                // coll.CheckForCollision();
            }
        }
        private void Jump()
        {
            sc.playJumpSound();
            if (p[0].onWallRight)
            {
                p[0].vx = -jumpheight;
                p[0].vy = -jumpheight;
            }
            else if (p[0].onWallLeft)
            {
                p[0].vx = jumpheight;
                p[0].vy = -jumpheight;
            }
            else
            {
                p[0].vy = -jumpheight;
            }
            jump = false;

        }

        private void MoveRight()
        {
                if (10 - p[0].vx <= moveacc)
                {
                    p[0].vx = p[0].vx + moveacc;
                }
                else
                {
                    p[0].vx = movespeed;
                }
        }
        private void MoveLeft()
        {
                if (-10 - p[0].vx >= -moveacc)
                {
                    p[0].vx = p[0].vx - moveacc;
                }
                else
                {
                    p[0].vx = -movespeed;
                }
        }

        //check for death/win. Set new window accordingly
        public void CheckWinDeath()
        {
            /* Toggle Code (Rasmus/Martin ask before you do dumb things)
            if (p[0].won)
            {
                if(TimeSpan.Compare(timer.Elapsed, highScores[0].getCurrentLvlHs()) == -1)
                {
                    
                    //mwm.Win();
                    /*var hswin = App.Current.MainWindow as MainWindow;
                    View.NewHighscore newhs = new View.NewHighscore();
                    hswin.Content = newhs;
                }
            }
            if (!p[0].alive)
            {
                sc.playDeathSound();
                Console.WriteLine("DEAD DEAD DEAD DEAD DEAD DEAD");
                p[0].alive = true;
                //mwm.Death();
            }
            
            var main = App.Current.MainWindow as MainWindow;
            View.LevelSelect ls = new View.LevelSelect();
            main.Content = ls;
            // MAYBE DO A COMMAND HERE
            */
        }
        public void Dispose()
        {
            moveTimer.Dispose();
        }

    }
}

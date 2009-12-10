using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hazard
{
    public class GameState
    {
        int point = 0;
        bool placing = true;
        Enum currentAction = Actions.finishBettingFirst;

        public enum Actions { passWin, passLose, roll, rollAgain, bet, finishBettingFirst };

        public GameState()
        {

        }

        public Enum getAction()
        {
            return currentAction;
        }

        public String getMessage()
        {
            String message = null;

            if ( point == 0 )
                message = "New Thrower";
            else 
                message = "Throw a " + point.ToString();

            return message;
        }

        public void startBetting()
        {
            placing = true;
        }
        public void doneBetting()
        {
            placing = false;
        }

        public bool canRoll()
        {
            return !placing;
        }

        public Enum manualWin()
        {
            point = 0;
            placing = true;
            currentAction = Actions.passWin;
            return Actions.passWin;
        }

        public Enum manualLose()
        {
            point = 0;
            placing = true;
            currentAction = Actions.passLose;
            return Actions.passLose;
        }

        public Hazard.GameState.Actions newRoll(int roll)
        {
            Actions ret;

            if (false) // should be if(placing). Hacked to ignore bet-placing turns.
            {   // Bets are being placed. Roll was in error.
                ret = Actions.finishBettingFirst;
            }
            else if (point == 0)
            { //This is the come-out roll
                if (roll == 7 || roll == 11)
                { // Rolled a natural, Pass Line Wins
                    ret = Actions.passWin;
                }
                else if (roll == 2 || roll == 3 || roll == 12)
                { // Crapped out, Pass Line Loses
                    ret = Actions.passLose;
                }
                else
                {   // Point established.
                    // Opportunity to bet.
                    point = roll;
                    startBetting();
                    ret = Actions.bet;
                }
            }
            else
            {
                // Point has been established and betting is done.

                if (roll == 7)
                { // Pass Line Loses. Turn over.
                    point = 0;
                    ret = Actions.passLose;
                }
                else if (roll == point)
                { // Made point! Pass Line Wins. Turn over.
                    point = 0;
                    ret = Actions.passWin;
                }
                else
                { // hasn't won or lost yet. Give another opportunity to bet.
                    placing = true;
                    ret = Actions.bet;
                }
            }
            currentAction = ret;
            return ret;
        }
    }
}

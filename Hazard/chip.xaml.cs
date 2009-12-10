using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Surface;
using Microsoft.Surface.Presentation;
using Microsoft.Surface.Presentation.Controls;
using Microsoft.Surface.Presentation.Manipulations;

namespace Hazard
{
    /// <summary>
    /// Interaction logic for chip.xaml
    /// </summary>
    public partial class chip : SurfaceUserControl
    {
        double value = 10;
        double cutval = 10;

        // It's a state machine!
        // 0: normal. Display total value.
        // 1: knob action detected. Display value being selected by virtual knob.
        //  : When the user lets go...
        //  :   if cut value = full value, nothing changed, so revert to 0
        //  :   otherwise, it remains in state 1, and can be further adjusted.
        //  : Moving the chip will "cut" the stack, leaving a new chip of the remaining value beneath it.
        // :  
        int state = 0;

        SurfaceWindow1 window;

        public chip(SurfaceWindow1 parentWindow, double startValue)
        {
            InitializeComponent();
            InitializeManipulationProcessor();
            window = parentWindow;
            value = startValue;
            setCut(value);

        }

        public void payout(int factor)
        {
            // You won!
            value = value * factor;
            
            // Add some neat animation here

            resetCut();
        }
        private Affine2DManipulationProcessor manipulationProcessor;

        private void InitializeManipulationProcessor()
        {
            manipulationProcessor = new Affine2DManipulationProcessor(Affine2DManipulations.Rotate, DialGrid, new Point(50, 50));
            manipulationProcessor.Affine2DManipulationDelta += OnManipulationDelta;

        }

        private void OnManipulationDelta(object sender, Affine2DOperationDeltaEventArgs e)
        {
            DialRotateTransform.Angle = DialRotateTransform.Angle + e.RotationDelta;
            
            setCut( cutval + Math.Round(e.RotationDelta / 10) );

        }

        protected override void OnContactDown(ContactEventArgs e)
        {
            if (true)
            {
                // Seeing if the touch is within the desired area for rotate control. 
                // If it is, do the rotate stuff.
                // if not, ignore it so the scatterview element handles it instead.
                // The desired area for rotation is a ring between two radii
                Point touchpoint = e.GetPosition(Ellipse);
                double tX = touchpoint.X - 50;
                double tY = touchpoint.Y - 50;
                double distance = Math.Sqrt(tX * tX + tY * tY);

                if (distance > 20 && distance < 40)
                {   // Apply rotate, not move
                    base.OnContactDown(e);

                    e.Contact.Capture(this);

                    manipulationProcessor.BeginTrack(e.Contact);

                    //Mark as handled
                    e.Handled = true;

                    //Make rotation indicator appear
                    GlowRing.Visibility = Visibility.Visible;
                }
                else
                {
                    if (state == 1) //if we are cutting the stack
                        cutStack();

                }
                
            }
        }

        protected override void OnContactUp(ContactEventArgs e)
        {
            base.OnContactUp(e);

            //Make rotation indicator hide, because it can't be rotated without a contact
            if (ContactsCaptured.Count <= 1)
                GlowRing.Visibility = Visibility.Hidden;

            //Check to make sure we're not formatting to look like a cut if we're not
            //This shouldn't be necessary but is a catch just in case my logic isn't flawless
            if (cutval == value)
                cutting(false);

        }

        // Run when the containing (custom) ScatterViewItem fires OnContactUp
        // Runs when the container was moved, but not rotated, and user just let go
        // Purpose: to find if chip was dragged onto another chip, which requires a merge
        //          to determine if the chip is in play, making a bet
        public void OnLetGo()
        {
            chip merger = touchingOther();
            if (merger != null)
            {
                //merge with the stack
                //BigStatusLabel.Content = "M";
                value = value + merger.value;
                resetCut();
                window.removeChip(merger);
            }

            // After every move, update if in a betting position.
            isBetting();
        }

        // checks all the chips in play to see if it's near enough to this one.
        // If there is a chip close enough, it returns a reference to that chip.
        // If there is not, the return is null.
        private chip touchingOther()
        {
            chip returnChip = null;

            double thisX = getCenter().X;
            double thisY = getCenter().Y;

            foreach (ScatterViewItemChip item in window.MainScatterView.Items)
            {
                if (isNearEnough(thisX, item.ActualCenter.X) && isNearEnough(thisY, item.ActualCenter.Y))
                {
                    chip c = (chip)item.Content;
                    if (c != this)
                        returnChip = c;
                }
            }
            
            return returnChip;
        }

        private bool isNearEnough(double v1, double v2)
        {
            if (Math.Abs(v1 - v2) <= 10)
                return true;
            else
                return false;
        }

        protected override void OnContactTapGesture(ContactEventArgs e)
        {
            base.OnContactTapGesture(e);
            resetCut();
        }

        private void cutting(bool cutting)
        {
            if (cutting)
            {
                BigStatusLabel.Foreground = Brushes.Cyan;
                state = 1;
            }
            else
            {
                BigStatusLabel.Foreground = Brushes.AntiqueWhite;
                state = 0;
            }
        }

        private void setCut(double newcutval)
        {
            if (newcutval >= value)
            {
                cutval = value;
                cutting(false);
            }
            else if (newcutval < 1)
            {
                cutval = 1;
                cutting(true);
            }
            else
            {
                cutval = newcutval;
                cutting(true);
            }


            BigStatusLabel.Content = cutval;
        }

        private void resetCut()
        {
            cutting(false);
            setCut(value);
        }

        private void cutStack()
        {
            //Create the new chip stack beneath me
            window.addChip(getCenter(), value-cutval);
            //Update my new total value
            value = cutval;
            //No longer in cut state, update style.
            cutting(false);
        }

        public Point getCenter()
        {
            return ((ScatterViewItem)Parent).ActualCenter;
        }

        // Checks if this chip is in a betting position
        // automatically sets visual cues
        // Warning: Concept of "in position" is local to the chip, not the table.
        public bool isBetting()
        {
            bool betting =  getCenter().X > 212 &&
                            getCenter().X < 812 &&
                            getCenter().Y > 309 &&
                            getCenter().Y < 459;

            if (betting)
                Ellipse.Stroke = Brushes.Red;
            else
                Ellipse.Stroke = Brushes.Black;

            return betting;
        }
    }
}

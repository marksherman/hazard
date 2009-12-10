using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using Microsoft.Surface;
using Microsoft.Surface.Presentation;
using Microsoft.Surface.Presentation.Controls;

namespace Hazard
{
    /// <summary>
    /// Interaction logic for SurfaceWindow1.xaml
    /// </summary>
    public partial class SurfaceWindow1 : SurfaceWindow
    {
        GameState gs;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SurfaceWindow1()
        {
            InitializeComponent();

            // Add handlers for Application activation events
            AddActivationHandlers();

            //Control Panel needs to know where this is
            Controls.setParentWindow(this);

            // GameState tracks the actual gameplay
            gs = new GameState();
            
            addChip(new Point(500, 800), 10);
            addChip(new Point(1000,500), 30);
            
        }
      
        private void addChip()
        {
            addChip(new Point(0, 0), 10);
        }

        public void addChip(Point center, double value)
        {
            chip chip1 = new chip(this, value);
            ScatterViewItemChip item1 = new ScatterViewItemChip();
            item1.Content = chip1;
            item1.CanScale = false;
            item1.CanRotate = false;
            item1.Clip = new EllipseGeometry(new Point(50, 50), 52, 52);
            Color bgcolor = new Color();
            bgcolor.A = 0;
            item1.Background = new SolidColorBrush(bgcolor);
            item1.ZIndex = 0;

            //Just for now, if the point = 0,0 don't bother setting it
            if (!center.Equals(new Point(0, 0)))
                item1.Center = center;

            MainScatterView.Items.Add(item1);
        }

        public void removeChip(chip todel)
        {
            MainScatterView.Items.Remove(todel.Parent);
        }

        public void triggerRoll(int value)
        {
            Hazard.GameState.Actions act = gs.newRoll(value);

            if (act == GameState.Actions.passLose)
                passLineLose();
            else if (act == GameState.Actions.passWin)
                passLineWin();

            PassLine.Label.Content = gs.getMessage();
        }

        public void passLineWin()
        {
            foreach ( ScatterViewItemChip item in MainScatterView.Items )
            {
                chip c = ((chip)item.Content);

                if (c.isBetting())
                {
                    c.payout(2);
                }
            }
        }

        public void passLineLose()
        {
            ArrayList toDelete = new ArrayList();

            foreach (ScatterViewItemChip item in MainScatterView.Items)
            {
                chip c = ((chip)item.Content);

                if (c.isBetting())
                {
                    toDelete.Add(item);    
                }
            }
            foreach (ScatterViewItemChip item in toDelete)
            {
                //Probably want to add some cute disappear animation
                MainScatterView.Items.Remove(item);
            }
        }

        /// <summary>
        /// Occurs when the window is about to close. 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            // Remove handlers for Application activation events
            RemoveActivationHandlers();
        }

        /// <summary>
        /// Adds handlers for Application activation events.
        /// </summary>
        private void AddActivationHandlers()
        {
            // Subscribe to surface application activation events
            ApplicationLauncher.ApplicationActivated += OnApplicationActivated;
            ApplicationLauncher.ApplicationPreviewed += OnApplicationPreviewed;
            ApplicationLauncher.ApplicationDeactivated += OnApplicationDeactivated;
        }

        /// <summary>
        /// Removes handlers for Application activation events.
        /// </summary>
        private void RemoveActivationHandlers()
        {
            // Unsubscribe from surface application activation events
            ApplicationLauncher.ApplicationActivated -= OnApplicationActivated;
            ApplicationLauncher.ApplicationPreviewed -= OnApplicationPreviewed;
            ApplicationLauncher.ApplicationDeactivated -= OnApplicationDeactivated;
        }

        /// <summary>
        /// This is called when application has been activated.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnApplicationActivated(object sender, EventArgs e)
        {
            //TODO: enable audio, animations here
        }

        /// <summary>
        /// This is called when application is in preview mode.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnApplicationPreviewed(object sender, EventArgs e)
        {
            //TODO: Disable audio here if it is enabled

            //TODO: optionally enable animations here
        }

        /// <summary>
        ///  This is called when application has been deactivated.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnApplicationDeactivated(object sender, EventArgs e)
        {
            //TODO: disable audio, animations here
        }
    }
}
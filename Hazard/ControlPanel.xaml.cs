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

namespace Hazard
{
    /// <summary>
    /// Interaction logic for ControlPanel.xaml
    /// </summary>
    public partial class ControlPanel : SurfaceUserControl
    {
        SurfaceWindow1 window;
        int input = 0;

        public ControlPanel()
        {
            InitializeComponent();

            DieButton1.Click += new RoutedEventHandler(DieButton1_Click);
            DieButton2.Click += new RoutedEventHandler(DieButton2_Click);
            DieButton3.Click += new RoutedEventHandler(DieButton3_Click);
            DieButton4.Click += new RoutedEventHandler(DieButton4_Click);
            DieButton5.Click += new RoutedEventHandler(DieButton5_Click);
            DieButton6.Click += new RoutedEventHandler(DieButton6_Click);

            Die2Button1.Click += new RoutedEventHandler(Die2Button1_Click);
            Die2Button2.Click += new RoutedEventHandler(Die2Button2_Click);
            Die2Button3.Click += new RoutedEventHandler(Die2Button3_Click);
            Die2Button4.Click += new RoutedEventHandler(Die2Button4_Click);
            Die2Button5.Click += new RoutedEventHandler(Die2Button5_Click);
            Die2Button6.Click += new RoutedEventHandler(Die2Button6_Click);

            PassButton.Click += new RoutedEventHandler(PassButton_Click);
            FailButton.Click += new RoutedEventHandler(FailButton_Click);

        }

        public void setParentWindow(SurfaceWindow1 w)
        {
            window = w;
        }

        private void DieButton1_Click(object sender, RoutedEventArgs e)
        {
            window.addChip(new Point(200, 200), 10);
        }
        private void DieButton2_Click(object sender, RoutedEventArgs e)
        {
            window.addChip(new Point(300, 200), 20);
        }
        private void DieButton3_Click(object sender, RoutedEventArgs e)
        {
            window.addChip(new Point(400, 200), 30);
        }
        private void DieButton4_Click(object sender, RoutedEventArgs e)
        {
            window.addChip(new Point(500, 200), 40);
        }
        private void DieButton5_Click(object sender, RoutedEventArgs e)
        {
            window.addChip(new Point(600, 200), 50);
        }
        private void DieButton6_Click(object sender, RoutedEventArgs e)
        {
            //addToInput(6);
        }

        private void Die2Button1_Click(object sender, RoutedEventArgs e)
        {
            addToInput(1);
        }
        private void Die2Button2_Click(object sender, RoutedEventArgs e)
        {
            addToInput(2);
        }
        private void Die2Button3_Click(object sender, RoutedEventArgs e)
        {
            addToInput(3);
        }
        private void Die2Button4_Click(object sender, RoutedEventArgs e)
        {
            addToInput(4);
        }
        private void Die2Button5_Click(object sender, RoutedEventArgs e)
        {
            addToInput(5);
        }
        private void Die2Button6_Click(object sender, RoutedEventArgs e)
        {
            addToInput(6);
        }

        private void PassButton_Click(object sender, RoutedEventArgs e)
        {
            window.passLineWin();
            
        }
        private void FailButton_Click(object sender, RoutedEventArgs e)
        {
            window.passLineLose();
        }

        private void addToInput(int val)
        {
            
            if (input == 0)
            {
                input = input + val;
                Status.Content = input;
            }
            else
            {
                // Execute Die roll on val + input
                input = input + val;
                window.triggerRoll(input);
                Status.Content = "Rolled " + input.ToString();
                input = 0;
            }

        }

    }
}

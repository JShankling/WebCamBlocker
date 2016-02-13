using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Webcam_Blocker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DriverClass Webcam;
        DriverClass Microphone;
        private bool ignoreEvent = false;

        public MainWindow()
        {
            InitializeComponent();
            MaxHeight = 500;
            MinHeight = 500;
            MaxWidth = 800;
            MinWidth = 800;
            Webcam = new DriverClass(DriverClass.Type.Image);
            Microphone = new DriverClass(DriverClass.Type.Audio);
            getWebcamStatus();
            getMicrophoneStatus();
        }

        private void getMicrophoneStatus()
        {
            Microphone.CheckStatus();
            if (Microphone.getStatus() == DriverClass.Status.ENABLE)
            {
                microphoneSlider.Value = 0;
                showMicrophoneEnableLabels();
            }
            else
            {
                microphoneSlider.Value = 1;
                showMicrophoneDisableLabels();
            }
        }

        private void getWebcamStatus()
        {
            Webcam.CheckStatus();
            if (Webcam.getStatus() == DriverClass.Status.ENABLE)
            {
                webcamSlider.Value = 0;
                showWebcamEnableLabels();
            }
            else
            {
                webcamSlider.Value = 1;
                showWebcamDisableLabels();
            }
        }

        private bool EnableMicrophone()
        {
            if (Microphone.Enable())
            {
                showMicrophoneEnableLabels();
                return true;
            }
            else { MessageBox.Show("Unable to Enable The Microphone"); }
            return false;
        }

        private bool DisableMicrophone()
        {
            if (Microphone.Disable())
            {
                showMicrophoneDisableLabels();
                return true;
            }
            else { MessageBox.Show("Unable to Enable The Microphone"); }
            return false;
        }

        private bool EnableWebcam()
        {
            if (Webcam.Enable())
            {
                showWebcamEnableLabels();
                return true;
            }
            else { MessageBox.Show("Unable to Enable The Webcam"); }
            return false;
        }

        private bool DisableWebcam()
        {
            if (Webcam.Disable())
            {
                showWebcamDisableLabels();
                return true;
            }
            else { MessageBox.Show("Unable to Disable The Webcam"); }
            return false;
        }

        private void showWebcamEnableLabels()
        {
            lblWebcamProtection.Content = "Your Webcam Is Not Protected";
            lblWebcamBlock.Content = "No Connections Are Blocked";
            lblWebcamProtection.Foreground = new SolidColorBrush(Colors.Red);
        }

        private void showWebcamDisableLabels()
        {
            lblWebcamProtection.Content = "Your Webcam Is Protected";
            lblWebcamBlock.Content = "All Connections Are Blocked";
            lblWebcamProtection.Foreground = new SolidColorBrush(Colors.Green);
        }

        private void showMicrophoneEnableLabels()
        {
            lblMicrophoneProtection.Content = "Your Microphone Is Not Protected";
            lblMicrophoneProtection.Foreground = new SolidColorBrush(Colors.Red);
            lblMicrophoneBlock.Content = "No Connections Are Blocked";
        }

        private void showMicrophoneDisableLabels()
        {
            lblMicrophoneProtection.Content = "Your Microphone Is Protected";
            lblMicrophoneProtection.Foreground = new SolidColorBrush(Colors.Green);
            lblMicrophoneBlock.Content = "All Connections Are Blocked";
        }

        private void webcamSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (ignoreEvent == true)
            {
                ignoreEvent = false;
                return;
            }

            var slider = sender as Slider;
            double value = slider.Value;
            if (slider.Value == 1)
            {
                if (!DisableWebcam())
                {
                    ignoreEvent = true;
                    slider.Value = 0;
                }
            }
            else {
                if (!EnableWebcam())
                {
                    ignoreEvent = true;
                    slider.Value = 1;
                }

            }
        }

        private void microphoneSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (ignoreEvent == true)
            {
                ignoreEvent = false;
                return;
            }

            var slider = sender as Slider;
            double value = slider.Value;
            if (slider.Value == 1)
            {
                if (!DisableMicrophone())
                {
                    ignoreEvent = true;
                    slider.Value = 0;
                }
            }
            else {
                if (!EnableMicrophone())
                {
                    ignoreEvent = true;
                    slider.Value = 1;
                }

            }

        }

    }
}

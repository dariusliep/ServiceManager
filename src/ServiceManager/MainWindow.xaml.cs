using System;
using System.Collections.Generic;
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
using ServicesOperations;
using System.Collections.Concurrent;
using System.Threading;
using System.Timers;

namespace ServiceManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ConcurrentQueue<Message> _queue = new ConcurrentQueue<Message>();
        private readonly AutoResetEvent _signal = new AutoResetEvent(false);
        private volatile bool _proceedConsumer = true;
        private System.Timers.Timer _serviceReadingTimer;

        //private Thread _servicesReadingThread = null;
        private Thread _servicesConsumingThread = null;

        public MainWindow()
        {
            InitializeComponent();

            _servicesConsumingThread = new Thread(new ThreadStart(ConsumeServicesList));
            _servicesConsumingThread.Start();

            _serviceReadingTimer = new System.Timers.Timer(1000);
            _serviceReadingTimer.Elapsed += new ElapsedEventHandler(ReadServicesList);
            _serviceReadingTimer.Enabled = true;

            //_servicesReadingThread = new Thread(new ThreadStart(ReadServicesList));
            //_servicesReadingThread.Start();
        }

        void ReadServicesList(object source, ElapsedEventArgs e)
        {
            ServicesReading reading = new ServicesReading();
            Message message = new Message();
            message.Services = reading.ReadAllServices();
            _queue.Enqueue(message);
            _signal.Set();
        }

        void ConsumeServicesList()
        {
            while(_proceedConsumer)
            {
                _signal.WaitOne();

                Message receivedMessage = null;
                while(_queue.TryDequeue(out receivedMessage))
                {
                    lvServicesListView.ItemsSource = receivedMessage.Services;
                }
            }
        }
    }
}

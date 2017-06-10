﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FIXApplication;
using System.Diagnostics;
using System.Windows;
using UIDemo.ViewModel;

namespace UIDemo
{
    public class AppContext
    {
        QFApp _qfapp = null;

        public string ConfigFile { get; set; }

        public void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            Trace.WriteLine("Uncaught exception:");
            Trace.WriteLine(e.Exception.ToString());
        }

        public void Application_Startup(
            object sender,
            StartupEventArgs e,
            ICustomFixStrategy strategy)
        {
            Trace.WriteLine("Application started.");

            // FIX app settings and related
            QuickFix.SessionSettings settings = new QuickFix.SessionSettings(this.ConfigFile);
            strategy.SessionSettings = settings;

            // FIX application setup
            QuickFix.IMessageStoreFactory storeFactory = new QuickFix.FileStoreFactory(settings);
            QuickFix.ILogFactory logFactory = new QuickFix.FileLogFactory(settings);
            _qfapp = new QFApp(settings, strategy);

            QuickFix.IInitiator initiator = new QuickFix.Transport.SocketInitiator(_qfapp, storeFactory, settings, logFactory);
            _qfapp.Initiator = initiator;


            // Window creation and context assignment
            UIDemo.MainWindow mainWindow = new UIDemo.MainWindow();
            mainWindow.DataContext = new MainWindowViewModel();

            mainWindow.MessageView.DataContext = new MessageViewModel(_qfapp);
            mainWindow.ExecutionView.DataContext = new ExecutionViewModel(_qfapp);
            mainWindow.ConnectionView.DataContext = new ConnectionViewModel(_qfapp);
            mainWindow.OrderView.DataContext = new OrderViewModel(_qfapp, strategy);
            mainWindow.NewsSenderView.DataContext = new NewsSenderViewModel(_qfapp);

            // Set the main UI dispatcher
            SmartDispatcher.SetDispatcher(mainWindow.Dispatcher);

            mainWindow.Show();
        }

        public void Application_Exit(object sender, ExitEventArgs e)
        {
            try
            {
                Trace.WriteLine("Application exit.");
                _qfapp.Stop();
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }
    }
}

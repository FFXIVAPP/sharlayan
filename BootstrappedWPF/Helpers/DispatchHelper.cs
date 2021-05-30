namespace BootstrappedWPF.Helpers {
    using System;
    using System.Threading;
    using System.Windows;
    using System.Windows.Threading;

    public static class DispatcherHelper {
        public static void Invoke(Action action, DispatcherPriority dispatcherPriority = DispatcherPriority.Background) {
            Application.Current.Dispatcher.BeginInvoke(dispatcherPriority, new ThreadStart(action));
        }
    }
}
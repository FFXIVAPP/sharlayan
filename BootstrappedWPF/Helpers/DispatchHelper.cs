// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DispatchHelper.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   DispatchHelper.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

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
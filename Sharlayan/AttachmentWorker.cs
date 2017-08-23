// Sharlayan ~ AttachmentWorker.cs
// 
// Copyright © 2007 - 2017 Ryan Wilson - All Rights Reserved
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Diagnostics;
using System.Linq;
using System.Timers;
using NLog;
using Sharlayan.Models;

namespace Sharlayan
{
    internal class AttachmentWorker : IDisposable
    {
        #region Logger

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        #endregion

        public AttachmentWorker()
        {
            _scanTimer = new Timer(1000);
            _scanTimer.Elapsed += ScanTimerElapsed;
        }

        #region Implementation of IDisposable

        public void Dispose()
        {
            _scanTimer.Elapsed -= ScanTimerElapsed;
        }

        #endregion

        /// <summary>
        /// </summary>
        /// <param name="sender"> </param>
        /// <param name="e"> </param>
        private void ScanTimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (_isScanning || !MemoryHandler.Instance.IsAttached)
            {
                return;
            }
            _isScanning = true;

            Func<bool> scanner = delegate
            {
                var processes = Process.GetProcesses();
                if (!processes.Any(process => process.Id == _processModel.ProcessID && process.ProcessName == _processModel.ProcessName))
                {
                    MemoryHandler.Instance.IsAttached = false;
                    MemoryHandler.Instance.UnsetProcess();
                }
                _isScanning = false;
                return true;
            };
            scanner.BeginInvoke(delegate { }, scanner);
        }

        #region Declarations

        private ProcessModel _processModel;
        private readonly Timer _scanTimer;
        private bool _isScanning;

        #endregion

        #region Timer Controls

        /// <summary>
        /// </summary>
        public void StartScanning(ProcessModel processModel)
        {
            _processModel = processModel;
            _scanTimer.Enabled = true;
        }

        /// <summary>
        /// </summary>
        public void StopScanning()
        {
            _scanTimer.Enabled = false;
        }

        #endregion
    }
}

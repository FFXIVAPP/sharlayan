// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClearingArrayPool.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   ClearingArrayPool.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan {
    using System.Buffers;

    public class ClearingArrayPool<T> : ArrayPool<T> {
        private ArrayPool<T> _arrayPool = Shared;

        public override T[] Rent(int minimumLength) {
            return this._arrayPool.Rent(minimumLength);
        }

        public void Return(T[] array) {
            this.Return(array, true);
        }

        public override void Return(T[] array, bool clearArray = false) {
            this._arrayPool.Return(array, clearArray);
        }
    }
}
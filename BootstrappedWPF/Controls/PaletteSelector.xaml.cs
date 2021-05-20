// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PaletteSelector.xaml.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   PaletteSelector.xaml.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BootstrappedWPF.Controls {
    using System.Windows.Controls;

    using BootstrappedWPF.ViewModels;

    /// <summary>
    /// Interaction logic for PaletteSelector.xaml
    /// </summary>
    public partial class PaletteSelector : UserControl {
        public PaletteSelector() {
            this.InitializeComponent();

            this.DataContext = new PaletteSelectorViewModel();
        }
    }
}
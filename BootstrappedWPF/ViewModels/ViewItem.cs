// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewItem.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   ViewItem.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BootstrappedWPF.ViewModels {
    using System;
    using System.Windows;
    using System.Windows.Controls;

    public class ViewItem : PropertyChangedBase {
        private readonly Type _contentType;

        private readonly object? _dataContext;

        private ScrollBarVisibility _horizontalScrollBarVisibilityRequirement;

        private Thickness _marginRequirement = new(16);

        private ScrollBarVisibility _verticalScrollBarVisibilityRequirement = ScrollBarVisibility.Auto;

        public ViewItem(string name, Type contentType, object? dataContext = null) {
            this.Name = name;
            this._contentType = contentType;
            this._dataContext = dataContext;
            this.Content = this.CreateContent();
        }

        public object? Content { get; }

        public ScrollBarVisibility HorizontalScrollBarVisibilityRequirement {
            get => this._horizontalScrollBarVisibilityRequirement;
            set => this.SetProperty(ref this._horizontalScrollBarVisibilityRequirement, value);
        }

        public Thickness MarginRequirement {
            get => this._marginRequirement;
            set => this.SetProperty(ref this._marginRequirement, value);
        }

        public string Name { get; }

        public ScrollBarVisibility VerticalScrollBarVisibilityRequirement {
            get => this._verticalScrollBarVisibilityRequirement;
            set => this.SetProperty(ref this._verticalScrollBarVisibilityRequirement, value);
        }

        private object? CreateContent() {
            object? content = Activator.CreateInstance(this._contentType);
            if (this._dataContext != null && content is FrameworkElement element) {
                element.DataContext = this._dataContext;
            }

            return content;
        }
    }
}
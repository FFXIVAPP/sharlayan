namespace BootstrappedWPF.ViewModels {
    using System;
    using System.Collections.Generic;
    using System.Windows.Input;

    using BootstrappedWPF.Properties;
    using BootstrappedWPF.Utilities;

    using MaterialDesignColors;

    using MaterialDesignThemes.Wpf;

    public class PaletteSelectorViewModel : PropertyChangedBase {
        private static Lazy<PaletteSelectorViewModel> _instance = new Lazy<PaletteSelectorViewModel>(() => new PaletteSelectorViewModel());

        private bool _isDarkTheme;

        public PaletteSelectorViewModel() {
            PaletteHelper paletteHelper = new PaletteHelper();
            ITheme theme = paletteHelper.GetTheme();

            this.IsDarkTheme = theme.GetBaseTheme() == BaseTheme.Dark;

            if (paletteHelper.GetThemeManager() is { } themeManager) {
                themeManager.ThemeChanged += (_, e) => {
                    this.IsDarkTheme = e.NewTheme?.GetBaseTheme() == BaseTheme.Dark;
                    Settings.Default.DarkMode = this.IsDarkTheme;
                };
            }
        }

        public static PaletteSelectorViewModel Instance => _instance.Value;

        public ICommand ApplyAccentCommand { get; } = new DelegatedCommand(o => ApplyAccent((Swatch) o));

        public ICommand ApplyPrimaryCommand { get; } = new DelegatedCommand(o => ApplyPrimary((Swatch) o));

        public bool IsDarkTheme {
            get => this._isDarkTheme;
            set {
                if (this.SetProperty(ref this._isDarkTheme, value)) {
                    ThemeUtilities.ModifyTheme(
                        theme => theme.SetBaseTheme(
                            value
                                ? Theme.Dark
                                : Theme.Light));
                }
            }
        }

        public IEnumerable<Swatch> Swatches { get; } = new SwatchesProvider().Swatches;

        private static void ApplyAccent(Swatch swatch) {
            if (swatch is { AccentExemplarHue: not null, }) {
                Settings.Default.UserThemeAccent = swatch.Name;
                ThemeUtilities.ModifyTheme(theme => theme.SetSecondaryColor(swatch.AccentExemplarHue.Color));
            }
        }

        private static void ApplyPrimary(Swatch swatch) {
            Settings.Default.UserThemePrimary = swatch.Name;
            ThemeUtilities.ModifyTheme(theme => theme.SetPrimaryColor(swatch.ExemplarHue.Color));
        }
    }
}
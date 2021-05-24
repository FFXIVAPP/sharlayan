namespace BootstrappedWPF.Utilities {
    using System;

    using MaterialDesignThemes.Wpf;

    public static class ThemeUtilities {
        public static void ModifyTheme(Action<ITheme> modificationAction) {
            PaletteHelper paletteHelper = new PaletteHelper();
            ITheme theme = paletteHelper.GetTheme();
            modificationAction?.Invoke(theme);
            paletteHelper.SetTheme(theme);
        }
    }
}
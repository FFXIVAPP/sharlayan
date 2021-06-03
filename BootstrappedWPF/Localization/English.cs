namespace BootstrappedWPF.Localization {
    using System.Windows;

    public class English {
        private static readonly ResourceDictionary _translations = new ResourceDictionary();

        public static ResourceDictionary Translations() {
            _translations.Clear();

            _translations.Add("MainWindow_HomeButtonToolTip", "Home");
            _translations.Add("MainWindow_SettingsButtonToolTip", "Settings");
            _translations.Add("MainWindow_ChatLogButtonToolTip", "ChatLog");
            _translations.Add("MainWindow_DebugButtonToolTip", "Debug");
            _translations.Add("MainWindow_AboutButtonToolTip", "About");
            _translations.Add("HomeTabItem_WelcomeText", "Welcome to Sharlayan's WPF Example");
            _translations.Add("AboutTabItem_GetInTouchText", "Get In Touch");
            _translations.Add("AboutTabItem_GetInTouchExtendedText", "Say hello, make a feature request, or raise a bug through one of these channels:");
            _translations.Add("AboutTabItem_OpenSourceText", "Open Source");
            _translations.Add("AboutTabItem_OpenSourceExtendedText", "This project is completely open source. If you like it and want to say thanks you could hit the GitHub Star button, tweet or post about it, or tell your mum about it!");
            _translations.Add("AboutTabItem_DonationText", "Feel like you want to make a donation?  It would be gratefully received.  Click the button to donate via GitHub sponsors.");
            _translations.Add("Palette_PrimaryMidText", "Primary - Mid");
            _translations.Add("Palette_LightText", "Light");
            _translations.Add("Palette_MidText", "Mid");
            _translations.Add("Palette_DarkText", "Dark");
            _translations.Add("Palette_AccentText", "Accent");
            _translations.Add("PaletteSelector_DescriptionText", "This is your current palette.");
            _translations.Add("PaletteSelector_LightText", "Light");
            _translations.Add("PaletteSelector_DarkText", "Dark");
            _translations.Add("PaletteSelector_PrimaryText", "Primary");
            _translations.Add("PaletteSelector_AccentText", "Accent");
            _translations.Add("SettingsTabItem_SharlayanTabHeaderText", "Sharlayan");
            _translations.Add("SettingsTabItem_ChatCodesTabHeaderText", "Chat Codes");
            _translations.Add("SettingsTabItem_ThemeTabHeaderText", "Theme");
            _translations.Add("UserSettings_DataSettingsText", "Data Settings");
            _translations.Add("UserSettings_UseCachedDataText", "Use locally cached JSON for Sharlayan resources");
            _translations.Add("UserSettings_ResetSettingsText", "Reset Settings");
            _translations.Add("UserSettings_MemoryTimingsText", "Memory Timings");
            _translations.Add("UserSettings_AdjustmentText", "Adjust these timings to increase or decrease data latency. Faster timings may increase CPU usage.");
            _translations.Add("UserSettings_ActionsHelperText", "Actions");
            _translations.Add("UserSettings_ActorHelperText", "Actor");
            _translations.Add("UserSettings_ChatLogHelperText", "ChatLog");
            _translations.Add("UserSettings_CurrentPlayerHelperText", "Current Player");
            _translations.Add("UserSettings_InventoryHelperText", "Inventory");
            _translations.Add("UserSettings_JobResourcesHelperText", "Job Resources");
            _translations.Add("UserSettings_PartyMembersHelperText", "Party Members");
            _translations.Add("UserSettings_TargetHelperText", "Target");

            return _translations;
        }
    }
}
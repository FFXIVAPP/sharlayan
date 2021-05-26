namespace BootstrappedWPF.Localization {
    using System.Windows;

    public class German {
        private static readonly ResourceDictionary _translations = new ResourceDictionary();

        public static ResourceDictionary Translations() {
            _translations.Clear();

            _translations.Add("MainWindow_HomeButtonToolTip", "Zuhause");
            _translations.Add("MainWindow_SettingsButtonToolTip", "die Einstellungen");
            _translations.Add("MainWindow_ChatLogButtonToolTip", "Chat Protokoll");
            _translations.Add("MainWindow_DebugButtonToolTip", "Debuggen");
            _translations.Add("MainWindow_AboutButtonToolTip", "Über");
            _translations.Add("HomeTabItem_WelcomeText", "Willkommen zu Sharlayans WPF-Beispiel");
            _translations.Add("AboutTabItem_GetInTouchText", "In Kontakt kommen");
            _translations.Add("AboutTabItem_GetInTouchExtendedText", "Sagen Sie Hallo, stellen Sie eine Funktionsanfrage oder melden Sie einen Fehler über einen dieser Kanäle:");
            _translations.Add("AboutTabItem_OpenSourceText", "Open Source");
            _translations.Add("AboutTabItem_OpenSourceExtendedText", "Dieses Projekt ist komplett Open Source. Wenn es dir gefällt und du dich bedanken möchtest, kannst du auf den GitHub Star-Button klicken, darüber twittern oder posten oder deiner Mutter davon erzählen!");
            _translations.Add("AboutTabItem_DonationText", "Möchten Sie eine Spende machen? Es würde dankbar aufgenommen werden. Klicken Sie auf die Schaltfläche, um über GitHub-Sponsoren zu spenden.");
            _translations.Add("Palette_PrimaryMidText", "Grundschule - Mitte");
            _translations.Add("Palette_LightText", "Licht");
            _translations.Add("Palette_MidText", "Mitte");
            _translations.Add("Palette_DarkText", "Dunkel");
            _translations.Add("Palette_AccentText", "Akzent");
            _translations.Add("PaletteSelector_DescriptionText", "Dies ist Ihre aktuelle Palette.");
            _translations.Add("PaletteSelector_LightText", "Licht");
            _translations.Add("PaletteSelector_DarkText", "Dunkel");
            _translations.Add("PaletteSelector_PrimaryText", "Primär");
            _translations.Add("PaletteSelector_AccentText", "Akzent");
            _translations.Add("SettingsTabItem_SharlayanTabHeaderText", "Sharlayan");
            _translations.Add("SettingsTabItem_ChatCodesTabHeaderText", "Chat-Codes");
            _translations.Add("SettingsTabItem_ThemeTabHeaderText", "Thema");
            _translations.Add("UserSettings_DataSettingsText", "Dateneinstellungen");
            _translations.Add("UserSettings_UseCachedDataText", "Verwenden Sie lokal zwischengespeicherten JSON für Sharlayan-Ressourcen");
            _translations.Add("UserSettings_ResetSettingsText", "Einstellungen zurücksetzen");
            _translations.Add("UserSettings_MemoryTimingsText", "Speicherzeiten");
            _translations.Add("UserSettings_AdjustmentText", "Passen Sie diese Timings an, um die Datenlatenz zu erhöhen oder zu verringern. Schnellere Timings können die CPU-Auslastung erhöhen.");
            _translations.Add("UserSettings_ActionsHelperText", "Aktionen");
            _translations.Add("UserSettings_ActorHelperText", "Darsteller");
            _translations.Add("UserSettings_ChatLogHelperText", "Chat Protokoll");
            _translations.Add("UserSettings_CurrentPlayerHelperText", "Aktueller Spieler");
            _translations.Add("UserSettings_InventoryHelperText", "Inventar");
            _translations.Add("UserSettings_JobResourcesHelperText", "Jobressourcen");
            _translations.Add("UserSettings_PartyMembersHelperText", "Parteimitglieder");
            _translations.Add("UserSettings_TargetHelperText", "Ziel");

            return _translations;
        }
    }
}
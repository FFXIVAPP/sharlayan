namespace BootstrappedWPF.Localization {
    using System.Windows;

    public class French {
        private static readonly ResourceDictionary _translations = new ResourceDictionary();

        public static ResourceDictionary Translations() {
            _translations.Clear();

            _translations.Add("MainWindow_HomeButtonToolTip", "Domicile");
            _translations.Add("MainWindow_SettingsButtonToolTip", "Paramètres");
            _translations.Add("MainWindow_ChatLogButtonToolTip", "ChatLog");
            _translations.Add("MainWindow_DebugButtonToolTip", "Déboguer");
            _translations.Add("MainWindow_AboutButtonToolTip", "Quelque");
            _translations.Add("HomeTabItem_WelcomeText", "Bienvenue dans l'exemple WPF de Sharlayan");
            _translations.Add("AboutTabItem_GetInTouchText", "Entrer en contact");
            _translations.Add("AboutTabItem_GetInTouchExtendedText", "Dites bonjour, faites une demande de fonctionnalité ou soulevez un bogue via l'un de ces canaux:");
            _translations.Add("AboutTabItem_OpenSourceText", "Open source");
            _translations.Add("AboutTabItem_OpenSourceExtendedText", "Ce projet est entièrement open source. Si vous l'aimez et que vous voulez dire merci, vous pouvez appuyer sur le bouton GitHub Star, tweeter ou publier à ce sujet, ou en parler à votre mère!");
            _translations.Add("AboutTabItem_DonationText", "Envie de faire un don? Il serait reçu avec gratitude. Cliquez sur le bouton pour faire un don via les sponsors GitHub.");
            _translations.Add("Palette_PrimaryMidText", "Primaire - Moyen");
            _translations.Add("Palette_LightText", "Lumière");
            _translations.Add("Palette_MidText", "Milieu");
            _translations.Add("Palette_DarkText", "Sombre");
            _translations.Add("Palette_AccentText", "Accent");
            _translations.Add("PaletteSelector_DescriptionText", "Ceci est votre palette actuelle.");
            _translations.Add("PaletteSelector_LightText", "Lumière");
            _translations.Add("PaletteSelector_DarkText", "Sombre");
            _translations.Add("PaletteSelector_PrimaryText", "Primaire");
            _translations.Add("PaletteSelector_AccentText", "Accent");
            _translations.Add("SettingsTabItem_SharlayanTabHeaderText", "Sharlayan");
            _translations.Add("SettingsTabItem_ChatCodesTabHeaderText", "Codes de chat");
            _translations.Add("SettingsTabItem_ThemeTabHeaderText", "Thème");
            _translations.Add("UserSettings_DataSettingsText", "Paramètres de données");
            _translations.Add("UserSettings_UseCachedDataText", "Utiliser le JSON mis en cache localement pour les ressources Sharlayan");
            _translations.Add("UserSettings_ResetSettingsText", "Réinitialiser les options");
            _translations.Add("UserSettings_MemoryTimingsText", "Timings de la mémoire");
            _translations.Add("UserSettings_AdjustmentText", "Ajustez ces horaires pour augmenter ou réduire la latence des données. Des synchronisations plus rapides peuvent augmenter l'utilisation du processeur.");
            _translations.Add("UserSettings_ActionsHelperText", "Actions");
            _translations.Add("UserSettings_ActorHelperText", "Acteur");
            _translations.Add("UserSettings_ChatLogHelperText", "ChatLog");
            _translations.Add("UserSettings_CurrentPlayerHelperText", "Joueur actuel");
            _translations.Add("UserSettings_InventoryHelperText", "Inventaire");
            _translations.Add("UserSettings_JobResourcesHelperText", "Ressources d'emploi");
            _translations.Add("UserSettings_PartyMembersHelperText", "Membres du parti");
            _translations.Add("UserSettings_TargetHelperText", "Cible");

            return _translations;
        }
    }
}
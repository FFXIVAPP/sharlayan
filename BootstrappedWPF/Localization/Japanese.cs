namespace BootstrappedWPF.Localization {
    using System.Windows;

    public class Japanese {
        private static readonly ResourceDictionary _translations = new ResourceDictionary();

        public static ResourceDictionary Translations() {
            _translations.Clear();

            _translations.Add("MainWindow_HomeButtonToolTip", "ホームホーム");
            _translations.Add("MainWindow_SettingsButtonToolTip", "設定");
            _translations.Add("MainWindow_ChatLogButtonToolTip", "ChatLog");
            _translations.Add("MainWindow_DebugButtonToolTip", "デバッグ");
            _translations.Add("HomeTabItem_WelcomeText", "SharlayanのWPFの例へようこそ");
            _translations.Add("AboutTabItem_GetInTouchText", "連絡する");
            _translations.Add("AboutTabItem_GetInTouchExtendedText", "次のいずれかのチャネルを通じて、挨拶するか、機能をリクエストするか、バグを報告してください。");
            _translations.Add("AboutTabItem_OpenSourceText", "オープンソース");
            _translations.Add("AboutTabItem_OpenSourceExtendedText", "このプロジェクトは完全にオープンソースです。 気に入って感謝の気持ちを伝えたい場合は、GitHub Starボタンを押すか、ツイートまたは投稿するか、お母さんに伝えてください。");
            _translations.Add("AboutTabItem_DonationText", "寄付したい気がしますか？ ありがたいです。 ボタンをクリックして、GitHubスポンサー経由で寄付します。");
            _translations.Add("Palette_PrimaryMidText", "プライマリ-ミッド");
            _translations.Add("Palette_LightText", "光");
            _translations.Add("Palette_MidText", "ミッド");
            _translations.Add("Palette_DarkText", "闇");
            _translations.Add("Palette_AccentText", "アクセント");
            _translations.Add("PaletteSelector_DescriptionText", "これが現在のパレットです。");
            _translations.Add("PaletteSelector_LightText", "光");
            _translations.Add("PaletteSelector_DarkText", "闇");
            _translations.Add("PaletteSelector_PrimaryText", "プライマリ");
            _translations.Add("PaletteSelector_AccentText", "アクセント");
            _translations.Add("SettingsTabItem_SharlayanTabHeaderText", "Sharlayan");
            _translations.Add("SettingsTabItem_ChatCodesTabHeaderText", "チャットコード");
            _translations.Add("SettingsTabItem_ThemeTabHeaderText", "テーマ");
            _translations.Add("UserSettings_DataSettingsText", "データ設定");
            _translations.Add("UserSettings_UseCachedDataText", "SharlayanリソースにローカルにキャッシュされたJSONを使用する");
            _translations.Add("UserSettings_ResetSettingsText", "設定をリセット");
            _translations.Add("UserSettings_MemoryTimingsText", "メモリのタイミング");
            _translations.Add("UserSettings_AdjustmentText", "これらのタイミングを調整して、データの待ち時間を増減します。 タイミングが速いと、CPU使用率が高くなる可能性があります。");
            _translations.Add("UserSettings_ActionsHelperText", "行動");
            _translations.Add("UserSettings_ActorHelperText", "俳優");
            _translations.Add("UserSettings_ChatLogHelperText", "ChatLog");
            _translations.Add("UserSettings_CurrentPlayerHelperText", "現在のプレーヤー");
            _translations.Add("UserSettings_InventoryHelperText", "在庫");
            _translations.Add("UserSettings_JobResourcesHelperText", "求人情報");
            _translations.Add("UserSettings_PartyMembersHelperText", "党員");
            _translations.Add("UserSettings_TargetHelperText", "目標");

            return _translations;
        }
    }
}
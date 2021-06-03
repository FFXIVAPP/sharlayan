namespace BootstrappedWPF.Localization {
    using System.Windows;

    public class Korean {
        private static readonly ResourceDictionary _translations = new ResourceDictionary();

        public static ResourceDictionary Translations() {
            _translations.Clear();

            _translations.Add("MainWindow_HomeButtonToolTip", "집");
            _translations.Add("MainWindow_SettingsButtonToolTip", "설정");
            _translations.Add("MainWindow_ChatLogButtonToolTip", "ChatLog");
            _translations.Add("MainWindow_DebugButtonToolTip", "디버그");
            _translations.Add("MainWindow_AboutButtonToolTip", "약간");
            _translations.Add("HomeTabItem_WelcomeText", "Sharlayan의 WPF 예제에 오신 것을 환영합니다.");
            _translations.Add("AboutTabItem_GetInTouchText", "연락하기");
            _translations.Add("AboutTabItem_GetInTouchExtendedText", "다음 채널 중 하나를 통해 인사, 기능 요청 또는 버그 제기 :");
            _translations.Add("AboutTabItem_OpenSourceText", "오픈 소스");
            _translations.Add("AboutTabItem_OpenSourceExtendedText", "이 프로젝트는 완전히 오픈 소스입니다. 당신이 그것을 좋아하고 감사의 말을 전하고 싶다면 GitHub Star 버튼을 누르거나, 그것에 대해 트윗하거나 게시하거나, 엄마에게 그것에 대해 말할 수 있습니다!");
            _translations.Add("AboutTabItem_DonationText", "기부하고 싶으신가요? 고맙게 받아 들일 것입니다. 버튼을 클릭하여 GitHub 스폰서를 통해 기부하세요.");
            _translations.Add("Palette_PrimaryMidText", "기본-중간");
            _translations.Add("Palette_LightText", "빛");
            _translations.Add("Palette_MidText", "중간");
            _translations.Add("Palette_DarkText", "어두운");
            _translations.Add("Palette_AccentText", "악센트");
            _translations.Add("PaletteSelector_DescriptionText", "이것이 현재 팔레트입니다.");
            _translations.Add("PaletteSelector_LightText", "빛");
            _translations.Add("PaletteSelector_DarkText", "어두운");
            _translations.Add("PaletteSelector_PrimaryText", "일 순위");
            _translations.Add("PaletteSelector_AccentText", "악센트");
            _translations.Add("SettingsTabItem_SharlayanTabHeaderText", "Sharlayan");
            _translations.Add("SettingsTabItem_ChatCodesTabHeaderText", "채팅 코드");
            _translations.Add("SettingsTabItem_ThemeTabHeaderText", "테마");
            _translations.Add("UserSettings_DataSettingsText", "데이터 설정");
            _translations.Add("UserSettings_UseCachedDataText", "Sharlayan 리소스에 로컬로 캐시 된 JSON 사용");
            _translations.Add("UserSettings_ResetSettingsText", "설정 재설정");
            _translations.Add("UserSettings_MemoryTimingsText", "메모리 타이밍");
            _translations.Add("UserSettings_AdjustmentText", "이러한 타이밍을 조정하여 데이터 대기 시간을 늘리거나 줄입니다. 타이밍이 빠를수록 CPU 사용량이 늘어날 수 있습니다.");
            _translations.Add("UserSettings_ActionsHelperText", "행위");
            _translations.Add("UserSettings_ActorHelperText", "배우");
            _translations.Add("UserSettings_ChatLogHelperText", "ChatLog");
            _translations.Add("UserSettings_CurrentPlayerHelperText", "현재 플레이어");
            _translations.Add("UserSettings_InventoryHelperText", "목록");
            _translations.Add("UserSettings_JobResourcesHelperText", "직업 자원");
            _translations.Add("UserSettings_PartyMembersHelperText", "파티원");
            _translations.Add("UserSettings_TargetHelperText", "표적");

            return _translations;
        }
    }
}
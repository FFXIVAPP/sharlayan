namespace BootstrappedWPF.Helpers {
    using System.Collections;
    using System.Globalization;
    using System.Linq;
    using System.Windows;

    using BootstrappedWPF.Localization;
    using BootstrappedWPF.ViewModels;

    public static class LocaleHelper {
        public static void UpdateLocale(CultureInfo cultureInfo) {
            string culture = cultureInfo.TwoLetterISOLanguageName;
            ResourceDictionary dictionary;
            switch (culture) {
                case "fr":
                    dictionary = French.Translations();
                    break;
                case "ja":
                    dictionary = Japanese.Translations();
                    break;
                case "de":
                    dictionary = German.Translations();
                    break;
                case "zh":
                    dictionary = Chinese.Translations();
                    break;
                case "ko":
                    dictionary = Korean.Translations();
                    break;
                default:
                    dictionary = English.Translations();
                    break;
            }

            AppViewModel.Instance.Locale = dictionary.Cast<DictionaryEntry>().ToDictionary(item => (string) item.Key, item => (string) item.Value);
        }
    }
}
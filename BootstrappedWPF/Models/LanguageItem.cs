namespace BootstrappedWPF.Models {
    using System.Globalization;

    public class LanguageItem {
        public CultureInfo CultureInfo { get; set; }
        public string ImageURI { get; set; }
        public string Language { get; set; }
        public string Title { get; set; }
    }
}
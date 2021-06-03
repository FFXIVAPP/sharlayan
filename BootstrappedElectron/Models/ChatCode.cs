namespace BootstrappedElectron.Models {
    public class ChatCode {
        public ChatCode(string code, string color, string description) {
            this.Code = code;
            this.Color = color;
            this.Description = description;
        }

        public string Code { get; }
        public string Color { get; set; }
        public string Description { get; set; }
    }
}
namespace BootstrappedWPF.Models {
    public class ChatCode : PropertyChangedBase {
        private string _color;

        private string _description;

        public ChatCode(string code) {
            this.Code = code;
            this.Color = "FFFFFF";
            this.Description = "Unknown";
        }

        public ChatCode(string code, string color, string description) {
            this.Code = code;
            this.Color = color;
            this.Description = description;
        }

        public string Code { get; }

        public string Color {
            get => this._color;
            set => this.SetProperty(ref this._color, value);
        }

        public string Description {
            get => this._description;
            set => this.SetProperty(ref this._description, value);
        }
    }
}
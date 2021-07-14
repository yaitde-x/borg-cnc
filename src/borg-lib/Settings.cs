
namespace Yuni.Settings {
    public class ApplicationSettings {

        public ApplicationSettings(string yuniRoot) {
            YuniRoot = yuniRoot;
        }
        
        public string YuniRoot { get; private set;}
    }
}
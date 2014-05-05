using System.Drawing;

namespace ElfTools {
    public class ButtonImages {
        private readonly Bitmap _selectedImage;
        private readonly Bitmap _unselectedImage;


        public ButtonImages(Bitmap unselected, Bitmap selected) {
            _unselectedImage = unselected;
            _selectedImage = selected;
        }


        public Bitmap GetImage(bool selected) {
            return selected ? _selectedImage : _unselectedImage;
        }
    }
}
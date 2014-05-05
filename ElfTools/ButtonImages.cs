using System.Drawing;

namespace ElfTools {
    public class ButtonImages {
        public Bitmap SelectedImage = null;
        public Bitmap UnselectedImage = null;

        public ButtonImages() {}


        public ButtonImages(Bitmap unselected, Bitmap selected) {
            UnselectedImage = unselected;
            SelectedImage = selected;
        }


        public Bitmap GetImage(bool selected) {
            return selected ? SelectedImage : UnselectedImage;
        }
    }
}
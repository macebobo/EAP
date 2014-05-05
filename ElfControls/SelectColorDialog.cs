using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ElfControls {
    [ToolboxBitmap(@"C:\Users\roba\Documents\Visual Studio 2012\Projects\Editable Adjustable Preview\ElfControls\Resources\SelectColorDialog.bmp")]
    public partial class SelectColorDialog : Component {
        #region [ Public Variables ]

        /// <summary>
        ///     Gets or sets the color name selected by the user.
        /// </summary>
        [Description("Gets or sets the color name selected by the user.")]
        public string ColorName { get; set; }

        /// <summary>
        ///     Gets or sets the color selected by the user.
        /// </summary>
        [Description("Gets or sets the color selected by the user.")]
        public Color Color { get; set; }

        /// <summary>
        ///     List of customized colors.
        /// </summary>
        [Description("List of customized colors.")]
        public Palette CustomColors { get; set; }

        /// <summary>
        ///     Custom image to use for the OK button on the Select Color dialog.
        /// </summary>
        [Description("Custom image to use for the OK button on the Select Color dialog.")]
        public Image OKButton_Image { get; set; }

        /// <summary>
        ///     Custom image to use for the Cancel button on the Select Color dialog.
        /// </summary>
        [Description("Custom image to use for the Cancel button on the Select Color dialog.")]
        public Image CancelButton_Image { get; set; }

        /// <summary>
        ///     Show the palette of colors next to the color picker.
        /// </summary>
        [Description("Show the palette of colors next to the color picker.")]
        public bool ShowPalette { get; set; }

        /// <summary>
        ///     Title for the dialog.
        /// </summary>
        [Description("Title for the dialog.")]
        public string Title { get; set; }

        #endregion [ Public Variables ]

        #region [ Constructors ]

        public SelectColorDialog() {
            InitializeComponent();
        }


        public SelectColorDialog(IContainer container) {
            container.Add(this);

            InitializeComponent();
        }

        #endregion [ Constructors ]

        #region [ Methods ]

        /// <summary>
        ///     Runs a common dialog box with a default owner.
        /// </summary>
        /// <returns>
        ///     System.Windows.Forms.DialogResult.OK if the user clicks OK in the dialog box; otherwise,
        ///     System.Windows.Forms.DialogResult.Cancel.
        /// </returns>
        public DialogResult ShowDialog() {
            var Result = DialogResult.Cancel;

            var frmDialog = new SelectColorForm();
            if ((Title ?? string.Empty).Length > 0) {
                frmDialog.Text = Title;
            }
            frmDialog.OKButton_Image = OKButton_Image;
            frmDialog.CancelButton_Image = CancelButton_Image;
            frmDialog.ShowPalette = ShowPalette;
            frmDialog.CustomColors = CustomColors;
            frmDialog.Color = Color;
            if (ColorName != null) {
                frmDialog.ColorName = ColorName;
            }

            Result = frmDialog.ShowDialog();
            if (Result != DialogResult.Cancel) {
                Color = frmDialog.Color;
                ColorName = frmDialog.ColorName;
            }

            frmDialog.Dispose();
            frmDialog = null;

            return Result;
        }


        /// <summary>
        ///     Runs a common dialog box with the specified owner.
        /// </summary>
        /// <param name="owner">
        ///     Any object that implements System.Windows.Forms.IWin32Window that represents the top-level window
        ///     that will own the modal dialog box.
        /// </param>
        /// <returns>
        ///     System.Windows.Forms.DialogResult.OK if the user clicks OK in the dialog box; otherwise,
        ///     System.Windows.Forms.DialogResult.Cancel.
        /// </returns>
        public DialogResult ShowDialog(IWin32Window owner) {
            var Result = DialogResult.Cancel;

            var frmDialog = new SelectColorForm();
            if (Title.Length > 0) {
                frmDialog.Text = Title;
            }
            frmDialog.Color = Color;
            frmDialog.ColorName = ColorName;
            frmDialog.OKButton_Image = OKButton_Image;
            frmDialog.CancelButton_Image = CancelButton_Image;

            Result = frmDialog.ShowDialog(owner);
            if (Result != DialogResult.Cancel) {
                Color = frmDialog.Color;
                ColorName = frmDialog.ColorName;
            }

            frmDialog.Dispose();
            frmDialog = null;

            return Result;
        }

        #endregion [ Methods ]
    }
}
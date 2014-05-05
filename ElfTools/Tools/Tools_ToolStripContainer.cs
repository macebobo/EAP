using System.Windows.Forms;

using ElfCore.Util;

using ElfRes = ElfTools.Properties.Resources;

namespace ElfTools.Tools {
    public partial class Tools_ToolStripContainer : ToolStripForm {
        #region [ Properties ]

        public ToolStrip GenericToolStrip {
            get { return Generic_ToolStrip; }
        }

        #endregion [ Properties ]

        #region [ Constructors ]

        public Tools_ToolStripContainer() {
            InitializeComponent();

            //Ellipse_NoFill.Image = ImageHandler.AddAnnotation(ElfRes.fill, Annotation.Delete);
            //Polygon_NoFill.Image = ImageHandler.AddAnnotation(ElfRes.fill, Annotation.Delete);
            //Rectangle_NoFill.Image = ImageHandler.AddAnnotation(ElfRes.fill, Annotation.Delete);

            //Ellipse_Fill.Image = ImageHandler.AddAnnotation(ElfRes.fill, Annotation.Check);
            //Polygon_Fill.Image = ImageHandler.AddAnnotation(ElfRes.fill, Annotation.Check);
            //Rectangle_Fill.Image = ImageHandler.AddAnnotation(ElfRes.fill, Annotation.Check);

            Crop_cmdClear.Image = ImageHandler.AddAnnotation(ElfRes.crop_small, Annotation.Delete);
            Crop_cmdExecute.Image = ImageHandler.AddAnnotation(ElfRes.crop_small, Annotation.Play);

            Mask_cmdRemove.Image = ImageHandler.AddAnnotation(ElfRes.mask, Annotation.Delete);
            Mask_cmdSelectAll.Image = ImageHandler.AddAnnotation(ElfRes.mask, Annotation.Star);
            Mask_cmdLoad.Image = ImageHandler.AddAnnotation(ElfRes.mask, Annotation.Open);
            Mask_cmdSave.Image = ImageHandler.AddAnnotation(ElfRes.mask, Annotation.Save);


            MoveChannel_cmdExecute.Image = ImageHandler.AddAnnotation(ElfRes.move_channel, Annotation.Play);
        }

        #endregion [ Constructors ]

        #region [ Methods ]

        //public override void AddCustomControls()
        //{
        //	TrackBar tb = new TrackBar();
        //	tb.AutoSize = false;
        //	tb.Size = new System.Drawing.Size(60, 20);
        //	tb.Maximum = 3;
        //	tb.LargeChange = 1;
        //	tb.TickStyle = TickStyle.None;

        //	ToolStripControlHost item = new ToolStripControlHost(tb);
        //	item.AutoSize = true;

        //	Spray_ToolStrip.Items.Insert(Spray_ToolStrip.Items.Count - 1, item);
        //}

        public override ToolStrip GetToolStrip(int ownerTool) {
            switch (ownerTool) {
                case (int) ToolID.Crop:
                    //return Crop_ToolStrip;
                    return null;

                case (int) ToolID.Ellipse:
                    return Ellipse_ToolStrip;

                case (int) ToolID.Mask_Ellipse:
                case (int) ToolID.Mask_Freehand:
                case (int) ToolID.Mask_Paint:
                case (int) ToolID.Mask_Rectangle:
                case (int) ToolID.Mask_Lasso:
                    return Mask_ToolStrip;

                case (int) ToolID.Erase:
                    return Eraser_ToolStrip;

                case (int) ToolID.Fill:
                    return null;

                case (int) ToolID.Icicles:
                    return Icicles_ToolStrip;

                case (int) ToolID.ImageStamp:
                    return ImageStamp_ToolStrip;

                case (int) ToolID.Line:
                    return Line_ToolStrip;

                case (int) ToolID.MegaTree:
                    return MegaTree_ToolStrip;

                case (int) ToolID.MoveChannel:
                    return MoveChannel_ToolStrip;

                case (int) ToolID.MultiChannelLine:
                    return MultiChannelLine_ToolStrip;

                case (int) ToolID.Paint:
                    return Paint_ToolStrip;

                case (int) ToolID.Polygon:
                    return Polygon_ToolStrip;

                case (int) ToolID.Rectangle:
                    return Rectangle_ToolStrip;

                case (int) ToolID.SingingFace:
                    return SingingFace_ToolStrip;

                case (int) ToolID.Spray:
                    return Spray_ToolStrip;

                case (int) ToolID.Text:
                    return Text_ToolStrip;

                case (int) ToolID.Zoom:
                    return Zoom_ToolStrip;

                default:
                    return null;
            }
        }

        #endregion [ Methods ]
    }
}
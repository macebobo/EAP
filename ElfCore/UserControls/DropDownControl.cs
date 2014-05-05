using System;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Windows.Forms.VisualStyles;

namespace Unity3.Controls
{
    public partial class DropDownControl : UserControl
	{
		#region [ Enums ]

		public enum eDockSide
        {
            Left,
            Right
        }

        public enum eDropState
        {
            Closed,
            Closing,
            Dropping,
            Dropped
        }

		#endregion [ Enums ]

		#region [ Private Variables ]

		private Rectangle _anchorClientBounds;
		private Size _anchorSize = new Size(121, 21);
		bool _closedWhileInControl;
		private bool _designView = true;
		private eDockSide _dockSide;
		DropDownContainer _dropContainer;
        Control _dropDownItem;
		private eDropState _dropState;
		protected bool mousePressed;
		private Size _storedSize;
		private string _text;

		#endregion [ Private Variables ]

		#region [ Properties ]

		public Rectangle AnchorClientBounds
		{
			get { return _anchorClientBounds; }
		}

		public Size AnchorSize
		{
			get { return _anchorSize; }
			set
			{
				_anchorSize = value;
				this.Invalidate();
			}
		}

		protected virtual bool CanDrop
		{
			get
			{
				if (_dropContainer != null)
					return false;

				if (_dropContainer == null && _closedWhileInControl)
				{
					_closedWhileInControl = false;
					return false;
				}

				return !_closedWhileInControl;
			}
		}

		[DefaultValue(false)]
		protected bool DesignView
		{
			get { return _designView; }
			set
			{
				if (_designView == value)
					return;

				_designView = value;
				if (_designView)
				{
					this.Size = _storedSize;
				}
				else
				{
					_storedSize = this.Size;
					this.Size = _anchorSize;
				}

			}
		}

		public eDockSide DockSide
		{
			get { return _dockSide; }
			set { _dockSide = value; }
		}

		protected eDropState DropState
        {
            get { return _dropState; }
        }

        public override string Text
        {
            get {return _text;}
            set 
            {
               _text = value;
               this.Invalidate();
            }
        }

		#endregion [ Properties ]

		#region [ Constructors ]

		public DropDownControl()
        {
            InitializeComponent();
            this._storedSize = this.Size;
            this.BackColor = Color.White;
            this.Text = this.Name;
        }

		#endregion [ Constructors ]

		#region [ Methods ]

		public void CloseDropDown()
		{
			if (_dropContainer != null)
			{
				_dropState = eDropState.Closing;
				_dropContainer.Freeze = false;
				_dropContainer.Close();
			}
		}

		public void FreezeDropDown(bool remainVisible)
		{
			if (_dropContainer != null)
			{
				_dropContainer.Freeze = true;
				if (!remainVisible)
					_dropContainer.Visible = false;
			}
		}

		protected virtual Rectangle GetDropDownBounds()
		{
			Size inflatedDropSize = new Size(_dropDownItem.Width + 2, _dropDownItem.Height + 2);
			Rectangle screenBounds = _dockSide == eDockSide.Left ?
				new Rectangle(this.Parent.PointToScreen(new Point(this.Bounds.X, this.Bounds.Bottom)), inflatedDropSize)
				: new Rectangle(this.Parent.PointToScreen(new Point(this.Bounds.Right - _dropDownItem.Width, this.Bounds.Bottom)), inflatedDropSize);
			Rectangle workingArea = Screen.GetWorkingArea(screenBounds);
			//make sure we're completely in the top-left working area
			if (screenBounds.X < workingArea.X)
				screenBounds.X = workingArea.X;
			if (screenBounds.Y < workingArea.Y)
				screenBounds.Y = workingArea.Y;

			//make sure we're not extended past the working area's right /bottom edge
			if (screenBounds.Right > workingArea.Right && workingArea.Width > screenBounds.Width)
				screenBounds.X = workingArea.Right - screenBounds.Width;
			if (screenBounds.Bottom > workingArea.Bottom && workingArea.Height > screenBounds.Height)
				screenBounds.Y = workingArea.Bottom - screenBounds.Height;

			return screenBounds;
		}

		private ComboBoxState GetState()
		{
			if (mousePressed || _dropContainer != null)
				return ComboBoxState.Pressed;
			else
				return ComboBoxState.Normal;
		}

		public void InitializeDropDown(Control dropDownItem)
        {
            if (_dropDownItem != null)
                throw new Exception("The drop down item has already been implemented!");
            _designView = false;
            _dropState = eDropState.Closed;
            this.Size = _anchorSize;
            this._anchorClientBounds = new Rectangle(2, 2, _anchorSize.Width - 21, _anchorSize.Height - 4);
            //removes the dropDown item from the controls list so it 
            //won't be seen until the drop-down window is active
            if (this.Controls.Contains(dropDownItem))
                this.Controls.Remove(dropDownItem);
            _dropDownItem = dropDownItem;
        }

		protected void OpenDropDown()
		{
			if (_dropDownItem == null)
				throw new NotImplementedException("The drop down item has not been initialized!  Use the InitializeDropDown() method to do so.");

			if (!CanDrop)
				return;

			_dropContainer = new DropDownContainer(_dropDownItem);
			_dropContainer.Bounds = GetDropDownBounds();
			_dropContainer.DropStateChange += new DropDownContainer.DropWindowArgs(dropContainer_DropStateChange);
			_dropContainer.FormClosed += new FormClosedEventHandler(dropContainer_Closed);
			this.ParentForm.Move += new EventHandler(ParentForm_Move);
			_dropState = eDropState.Dropping;
			_dropContainer.Show(this);
			_dropState = eDropState.Dropped;
			this.Invalidate();
		}

		public void UnFreezeDropDown()
		{
			if (_dropContainer != null)
			{
				_dropContainer.Freeze = false;
				if (!_dropContainer.Visible)
					_dropContainer.Visible = true;
			}
		}

		#endregion [ Methods ]

		#region [ Events ]

		#region [ Event Handlers ]
		
		public event EventHandler PropertyChanged;

		#endregion [ Event Handlers ]

		#region [ Event Triggers ]

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			mousePressed = true;
			OpenDropDown();
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);
			mousePressed = false;
			this.Invalidate();
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			//Check if VisualStyles are supported...
			//Thanks to codeproject member: Mathiyazhagan for catching this. :)
			if (ComboBoxRenderer.IsSupported)
			{
				ComboBoxRenderer.DrawTextBox(e.Graphics, new Rectangle(new Point(0, 0), _anchorSize), GetState());
				ComboBoxRenderer.DrawDropDownButton(e.Graphics, new Rectangle(_anchorSize.Width - 19, 2, 18, _anchorSize.Height - 4), GetState());
			}
			else
			{
				ControlPaint.DrawComboButton(e.Graphics, new Rectangle(
					_anchorSize.Width - 19, 2, 18, _anchorSize.Height - 4),
					(this.Enabled) ? ButtonState.Normal : ButtonState.Inactive);
			}

			using (Brush b = new SolidBrush(this.BackColor))
			{
				e.Graphics.FillRectangle(b, this.AnchorClientBounds);
			}

			TextRenderer.DrawText(e.Graphics, _text, this.Font, this.AnchorClientBounds, this.ForeColor, TextFormatFlags.WordEllipsis);
		}

		protected void OnPropertyChanged()
		{
			if (PropertyChanged != null)
				PropertyChanged(null, null);
		}

		protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (_designView)
                _storedSize = this.Size;
            _anchorSize.Width = this.Width;
            if (!_designView)
            {
                _anchorSize.Height = this.Height;
                this._anchorClientBounds = new Rectangle(2, 2, _anchorSize.Width - 21, _anchorSize.Height - 4);
            }
        }
		        
		#endregion [ Event Triggers ]

		#region [ Event Delegates ]

		void dropContainer_Closed(object sender, FormClosedEventArgs e)
		{
			if (!_dropContainer.IsDisposed)
			{
				_dropContainer.DropStateChange -= dropContainer_DropStateChange;
				_dropContainer.FormClosed -= dropContainer_Closed;
				this.ParentForm.Move -= ParentForm_Move;
				_dropContainer.Dispose();
			}
			_dropContainer = null;
			_closedWhileInControl = (this.RectangleToScreen(this.ClientRectangle).Contains(Cursor.Position));
			_dropState = eDropState.Closed;
			this.Invalidate();
		}

		void dropContainer_DropStateChange(DropDownControl.eDropState state)
		{
			_dropState = state;
		}

		void ParentForm_Move(object sender, EventArgs e)
		{
			_dropContainer.Bounds = GetDropDownBounds();
		}

		#endregion [ Event Delegates ]

		#endregion [ Events ]

		#region [ Class DropDownContainer ]

		internal sealed class DropDownContainer : Form, IMessageFilter
        {
            public bool Freeze;

            public DropDownContainer(Control dropDownItem)
            {
                this.FormBorderStyle = FormBorderStyle.None;
                dropDownItem.Location = new Point(1, 1);
                this.Controls.Add(dropDownItem);
                this.StartPosition = FormStartPosition.Manual;
                this.ShowInTaskbar = false;
                Application.AddMessageFilter(this);
            }

            public bool PreFilterMessage(ref Message m)
            {
                if (!Freeze && this.Visible && (Form.ActiveForm == null || !Form.ActiveForm.Equals(this)))
                {
                    OnDropStateChange(eDropState.Closing);
                    this.Close();
                }


                return false;
            }

            public delegate void DropWindowArgs(eDropState state);
            public event DropWindowArgs DropStateChange;
            
			private void OnDropStateChange(eDropState state)
            {
                if (DropStateChange != null)
                    DropStateChange(state);
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);
                e.Graphics.DrawRectangle(Pens.Gray, new Rectangle(0,0,this.ClientSize.Width - 1, this.ClientSize.Height - 1));
            }

            protected override void OnClosing(CancelEventArgs e)
            {
                Application.RemoveMessageFilter(this);
                this.Controls.RemoveAt(0); //prevent the control from being disposed
                base.OnClosing(e);
            }
		}

		#endregion [ Class DropDownContainer ]
	}
}

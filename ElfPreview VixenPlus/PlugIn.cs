using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Xml;

using ElfCore.Channels;
using ElfCore.Controllers;
using ElfCore.Forms;
using ElfCore.Profiles;
using ElfCore.Util;

using ElfPreview.Annotations;

using Vixen = VixenPlus;

namespace ElfPreview {
    /// <summary>
    ///     Elf Preview Output PlugIn For Vixen+
    /// </summary>
    [UsedImplicitly]
    public class PlugIn : Vixen.IEventDrivenOutputPlugIn {
        #region [ Private Variables ]

        private readonly List<Vixen.Channel> _channels;
        private Vixen.IExecutable _executable;
        private PreviewDialog _previewDialog;
        private string _profileFileName = string.Empty;
        private XmlNode _setupNode;
        private int _startChannel; // index of first channel of data range sent

        #endregion [ Private Variables ]

        #region [ Properties ]

        /// <summary>
        ///     Name of the PlugIn Author(s)
        /// </summary>
        [UsedImplicitly]
        public string Author {
            get { return "Rob Anderson"; }
        }

        /// <summary>
        ///     Description of the PlugIn
        /// </summary>
        [UsedImplicitly]
        public string Description {
            get { return "Elf Preview Output PlugIn for Vixen+"; }
        }

        /// <summary>
        ///     Does not use any hardware
        /// </summary>
        public string HardwareMap {
            get { return null; }
        }

        /// <summary>
        ///     Public Name of the PlugIn
        /// </summary>
        public string Name {
            get { return "Elf Preview"; }
        }

        #endregion [ Properties ]

        #region [ Constructors ]

        public PlugIn() {
            _channels = new List<Vixen.Channel>();
        }

        #endregion [ Constructors ]

        #region [ Methods ]

        /// <summary>
        ///     Called when the plug-in needs to update the hardware state during execution
        /// </summary>
        /// <param name="channelValues">Event values in channel order, 1 byte per channel.</param>
        public void Event(byte[] channelValues) {
            if (_previewDialog == null || _previewDialog.Disposing || _previewDialog.IsDisposed) {
                return;
            }

            _previewDialog.UpdateWith(channelValues, _executable.Name);
        }


        /// <summary>
        ///     Called anytime Vixen needs to make sure the plug-in's setup or other initialization is up to date.
        ///     Initialize is called before the plug-in is setup, before sequence execution, and other times.
        ///     It's called from multiple places at any time, therefore the plug-in can make no assumptions
        ///     about the state of the program or sequence due to a call to Initialize.
        /// </summary>
        /// <param name="executableObject">
        ///     An object which provides methods and fields which represent the executable object
        ///     (typically a sequence) which is executing.
        /// </param>
        /// <param name="setupData">
        ///     A SetupData reference that provides some plug-incentric convenience methods. It can be safely
        ///     ignored.
        /// </param>
        /// <param name="setupNode">An XmlNode representing the root of the plug-in's setup data in the sequence.</param>
        public void Initialize(Vixen.IExecutable executableObject, Vixen.SetupData setupData, XmlNode setupNode) {
            _executable = executableObject;
            _channels.Clear();
            _channels.AddRange(executableObject.FullChannels);

            _setupNode = setupNode;

            try {
                _profileFileName = ((Vixen.Profile) executableObject).FileName ?? string.Empty;
            }
            catch {
                try {
                    _profileFileName = ((Vixen.EventSequence) executableObject).Profile.FileName ?? string.Empty;
                    if (_profileFileName.Length == 0) {
                        _profileFileName = ((Vixen.EventSequence) executableObject).Profile.Name + ".pro";
                    }
                }
                    // ReSharper disable EmptyGeneralCatchClause
                catch {}
                // ReSharper restore EmptyGeneralCatchClause
            }

            // They specified in 1-base, we use 0-base
            if (_setupNode.Attributes != null) {
                _startChannel = Convert.ToInt32(_setupNode.Attributes["from"].Value) - 1;
            }
        }


        /// <summary>
        ///     Called when the user has requested to setup the plug-in instance. (Select COM Ports, etc)
        /// </summary>
        public Control Setup() {
            var mySettings = Settings.Instance;
            mySettings.Style = Settings.SettingsStyle.Xml;

            var myWorkshop = Workshop.Instance;

            try {
                if (_channels.Count == 0) {
                    MessageBox.Show(@"The Profile you are trying to edit has no channels.", @"Elf Preview", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return null;
                }

                myWorkshop.RunMode = RunMode.PlugIn;
                myWorkshop.Initialize();

                // Gather up all the Vixen channel data and send it along to the Load method, in case this is a brand
                // new profile and will need this.
                var rawChannelList =
                    _channels.Select(
                        vChannel =>
                            new RawChannel
                            {Name = vChannel.Name, SequencerColor = vChannel.Color, Enabled = vChannel.Enabled, ID = vChannel.OutputChannel}).ToList();

                var loadingProfile = new BaseProfile(typeof (ElfProfiles.Vixen.VixenPlus));
                loadingProfile.Load(_setupNode, rawChannelList);
                loadingProfile.Name = _executable.Name;
                loadingProfile.Filename = _profileFileName;
                myWorkshop.ProfileController.Add(loadingProfile);

                using (var editor = new Editor()) {
                    editor.ShowDialog();
                }

                myWorkshop.ProfileController.Remove(loadingProfile);
            }
            catch (Exception ex) {
                Workshop.CrashLog(ex);
            }
            finally {
                if (mySettings != null) {
                    mySettings.Save();
                }
            }

            return null;
        }


        public void GetSetup() {
        }


        public void CloseSetup() {
        }


        public bool SupportsLiveSetup() {
            return false;
        }


        public bool SettingsValid() {
            return true;
        }


        /// <summary>
        ///     Called when a sequence is executed. MDI forms are now handled differently.
        /// </summary>
        public void Startup() {
            if ((_channels == null) || (_channels.Count == 0)) {
                return;
            }

            var systemInterface = (Vixen.ISystem) Vixen.Interfaces.Available["ISystem"];
            var ci = typeof (PreviewDialog).GetConstructor(new[] {typeof (XmlNode), typeof (List<Vixen.Channel>), typeof (int)});
            _previewDialog = (PreviewDialog) systemInterface.InstantiateForm(ci, _setupNode, _channels, _startChannel);
        }


        /// <summary>
        ///     Called when execution is stopped or the plug-in instance is no longer going to be referenced.
        /// </summary>
        public void Shutdown() {
            if (_previewDialog != null) {
                if (_previewDialog.InvokeRequired) {
                    _previewDialog.Invoke((MethodInvoker) delegate {
                        _previewDialog.Close();
                        _previewDialog.Dispose();
                    });
                }
                else {
                    _previewDialog.Close();
                    _previewDialog.Dispose();
                }
                _previewDialog = null;
            }
            _channels.Clear();
            _setupNode = null;
        }


        public override string ToString() {
            return Name;
        }

        #endregion [ Methods ]
    }
}

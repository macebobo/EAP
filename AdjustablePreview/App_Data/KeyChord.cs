using System.Windows.Forms;

namespace ElfCore
{
	public class KeyChord
	{
		public Keys Start;
		public Keys End;

		public void Clear()
		{
			Start = Keys.None;
			End = Keys.None;
		}
	}
}

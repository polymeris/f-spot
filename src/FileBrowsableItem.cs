/*
 * FileBrowsableItem.cs
 *
 * Author(s):
 *	Larry Ewing  (lewing@novell.com)
 *	Stephane Delcroix  (stephane@delcroix.org)
 *
 * This is free software. See COPYING for details
 */


using System;
using System.IO;
using System.Collections;
using System.Xml;

using Hyena;
using FSpot.Utils;

namespace FSpot {
	public class FileBrowsableItem : IBrowsableItem, IDisposable
	{
		ImageFile img;
		bool attempted;

		public FileBrowsableItem (SafeUri uri)
		{
			DefaultVersion = new FileBrowsableItemVersion () {
                Uri = uri
            };
		}

		protected ImageFile Image {
			get {
				if (!attempted) {
					img = ImageFile.Create (DefaultVersion.Uri);
					attempted = true;
				}

				return img;
			}
		}

		public Tag [] Tags {
			get {
				return null;
			}
		}

		public DateTime Time {
			get {
				return Image.Date;
			}
		}

		public IBrowsableItemVersion DefaultVersion { get; private set; }

		public string Description {
			get {
				try {
					if (Image != null)
						return Image.Description;

				} catch (System.Exception e) {
					Log.Exception (e);
				}

				return null;
			}
		}

		public string Name {
			get {
				return Path.GetFileName (Image.Uri.AbsolutePath);
			}
		}

		public uint Rating {
			get {
				return 0; //FIXME ndMaxxer: correct?
			}
		}

		public void Dispose ()
		{
			img.Dispose ();
			GC.SuppressFinalize (this);
		}

		private class FileBrowsableItemVersion : IBrowsableItemVersion {
			public string Name { get { return String.Empty; } }
			public bool IsProtected { get { return true; } }

			public SafeUri BaseUri { get { return Uri.GetBaseUri (); } }
			public string Filename { get { return Uri.GetFilename (); } }
			public SafeUri Uri { get; set; }
		}
	}
}

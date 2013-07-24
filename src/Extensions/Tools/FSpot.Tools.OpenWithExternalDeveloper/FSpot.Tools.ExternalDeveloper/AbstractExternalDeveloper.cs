//  OpenWithExternalDeveloper.cs
//
//  Author:
//       Camilo Polymeris <cpolymeris@gmail.com>
//
//  Copyright (c) 2013 Camilo Polymeris
// 
//  Contains code from: AbstractDevelopInUFRaw.cs
//  Author:
//   	Stephane Delcroix <stephane@delcroix.org>
//   	Ruben Vermeersch <ruben@savanne.be>
//
//  Copyright (C) 2007-2010 Novell, Inc.
//  Copyright (C) 2007-2009 Stephane Delcroix
//  Copyright (C) 2008-2010 Ruben Vermeersch
//
//  Permission is hereby granted, free of charge, to any person obtaining
//  a copy of this software and associated documentation files (the
//  "Software"), to deal in the Software without restriction, including
//  without limitation the rights to use, copy, modify, merge, publish,
//  distribute, sublicense, and/or sell copies of the Software, and to
//  permit persons to whom the Software is furnished to do so, subject to
//  the following conditions:
//
//  The above copyright notice and this permission notice shall be
//  included in all copies or substantial portions of the Software.
//
//  THE SOFTWARE IS PROVIDED AS IS, WITHOUT WARRANTY OF ANY KIND,
//  EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
//  MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
//  NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
//  LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
//  OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
//  WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
//
using System;
using System.IO;
using FSpot.Imaging;
using Hyena;
using Mono.Unix;

namespace FSpot.Tools.ExternalDeveloper
{
	/// <summary>
	/// Base class for external developer handlers. Uses the Development class to pass development
	/// parameters to the derived classes' implamentation of CallExternalDeveloper.
	/// Includes a file watcher to update the Photo with a new version when changes are registered.
	/// </summary>
	public abstract class AbstractExternalDeveloper
	{
		/// Simple class to pass around and store development parameters
		/// In the future might include JPEG parameters (e.g. quality)
		protected class Development
		{
			public Development(Photo p)
			{
				photo = p;
				if (!ImageFile.IsRaw(Raw.Uri))
					throw new InvalidDataException();

				version = GetVersionName(photo);
				developed = GetUriForVersionName(photo, version);
			}

			public void StartWatching()
			{
				watcher = new FileSystemWatcher();
				watcher.Path = Path.GetDirectoryName(developed.LocalPath); 
				watcher.Filter = Path.GetFileName(developed.LocalPath);
				watcher.Changed += new FileSystemEventHandler(
					(sender, e) => this.OnDevelopedChanged(false));
				watcher.Created += new FileSystemEventHandler(
					(sender, e) => this.OnDevelopedChanged(true));
				Log.Information(String.Format(
					"Watching developments to file '{0}'",
					developed.LocalPath));
				watcher.EnableRaisingEvents = true;
			}

			public void StopWatching()
			{
				Log.Information(String.Format(
					"Stop watching developments to file '{0}'",
					developed.LocalPath));
				watcher = null;
			}

			protected void OnDevelopedChanged(bool created)
			{
				SafeUri uri = new SafeUri(developed);

				Log.Information(String.Format(
					"File '{0}' has been created or changed, adding version to database",
					uri));

				if (created)
				{
					string path = SafeUri.FilenameToUri(Path.GetDirectoryName(uri.AbsolutePath));
					string filename = Path.GetFileName(uri.AbsolutePath);
					photo.AddVersion(new SafeUri(path), filename, version, true);
				}
				photo.Changes.DataChanged = true;
				App.Instance.Database.Photos.Commit(photo);
			}

			public PhotoVersion Raw
			{
				get
				{
					return photo.GetVersion(Photo.OriginalVersionId) as PhotoVersion;
				}
			}

			public Hyena.SafeUri RawUri
			{
				get
				{
					return Raw.Uri;
				}
			}

			public System.Uri DevelopedUri
			{
				get
				{
					return developed;
				}
			}

			private Photo photo;
			private string version; //< Name of the version of this photo the dev is assigned to
			private System.Uri developed; //< Path of output jpeg
			private FileSystemWatcher watcher;
		}

		/// <summary>
		/// Derived classes should reimplement this for program-specific functionality.
		/// Remember to call d.StartWatching & d.StopWatching on program start and end, resp.
		/// </summary>
		protected abstract void CallExternalDeveloper(Development d);

		public void Run(object o, EventArgs e, Photo p)
		{
			try
			{
				Development d = new Development(p);
				CallExternalDeveloper(d);
			} catch (InvalidDataException)
			{
				Log.Warning("The original version of this image is not a (supported) RAW file");
				return;
			}
		}

		protected static string GetVersionName(Photo p)
		{
			return GetVersionName(p, 1);
		}
		
		private static string GetVersionName(Photo p, int i)
		{
			string name = Catalog.GetPluralString("External development",
			                                      "External development #{0}", i);
			name = String.Format(name, i);
			if (p.VersionNameExists(name))
				return GetVersionName(p, i + 1);
			return name;
		}
		
		protected static System.Uri GetUriForVersionName(Photo p, string version_name)
		{
			string name_without_ext = System.IO.Path.GetFileNameWithoutExtension(p.Name);
			return new System.Uri(System.IO.Path.Combine(DirectoryPath(p), name_without_ext
				+ " (" + version_name + ")" + ".jpg"));
		}
		
		protected static string DirectoryPath(Photo p)
		{
				return Path.GetDirectoryName(p.VersionUri(Photo.OriginalVersionId));
		}

		public int JpegQuality { get; set; }
	}	
}


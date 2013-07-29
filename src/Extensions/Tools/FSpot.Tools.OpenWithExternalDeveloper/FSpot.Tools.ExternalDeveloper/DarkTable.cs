//  DarkTable.cs
//
//  Author:
//       Camilo Polymeris <cpolymeris@gmail.com>
//
//  Copyright (c) 2013 Camilo Polymeris
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
using Hyena;

namespace FSpot.Tools.ExternalDeveloper
{

	public class DarkTable : AbstractExternalDeveloper
	{
		protected override void CallExternalDeveloper (Development d)
		{
			string executable = "darktable";
			string args = String.Format ("-d memory '{0}'", d.RawUri.LocalPath);
			d.StartWatching ();
			Log.Information (String.Format ("Calling Darktable: {0} {1}", executable, args));
			System.Diagnostics.Process dt = System.Diagnostics.Process.Start (executable, args);
			dt.EnableRaisingEvents = true;
			dt.Exited += (sender, o) => DoDevelop(d);
		}


		void DoDevelop(Development d)
		{
			// After user edits...
			// ...actually generate, JPEG using darktable-cli. Make sure sidecar is present!
			string executable = "darktable-cli";
			string args = String.Format ("'{0}' '{1}'",
			                             d.RawUri.LocalPath, d.DevelopedUri.LocalPath);
			System.Diagnostics.Process dt_cli = System.Diagnostics.Process.Start (executable, args);
			dt_cli.Exited += (sender, e) => d.StopWatching();
		}
	}
}

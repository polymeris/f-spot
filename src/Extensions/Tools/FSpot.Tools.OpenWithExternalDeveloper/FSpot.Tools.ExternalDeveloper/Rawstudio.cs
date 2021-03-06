//  Rawstudio.cs
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

	public class Rawstudio : AbstractExternalDeveloper
	{

		private static bool? supported = null;

		protected override void CallExternalDeveloper (Development d)
		{
			string executable = "rawstudio";
			string args = String.Format ("--output='{1}' '{0}'",
			                             d.RawUri.LocalPath, d.DevelopedUri.LocalPath);
			d.StartWatching ();
			Log.Information (String.Format ("Calling Rawstudio: {0} {1}", executable, args));
			System.Diagnostics.Process rs = System.Diagnostics.Process.Start (executable, args);
			rs.EnableRaisingEvents = true;
			rs.Exited += (sender, e) => d.StopWatching();
		}

		public static bool SupportedVersionIsInstalled()
		{
			if (supported != null)
				return (bool)supported;
			// TODO: if my patch doesn't get accepted by version 2.1, this has to be rewritten
			string executable = "rawstudio";
			string args = "--version";
			System.Diagnostics.Process rs = System.Diagnostics.Process.Start (executable, args);
			if (!rs.WaitForExit(2000)) //< that should be more than enough time, I hope
			{
				rs.Kill();
				supported = false;
			}
			else
				supported = rs.ExitCode == 0;
			return (bool)supported;
		}
	}

}

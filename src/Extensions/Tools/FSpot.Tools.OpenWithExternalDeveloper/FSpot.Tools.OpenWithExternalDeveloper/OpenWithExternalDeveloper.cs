//  OpenWithExternalDeveloper.cs
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
using FSpot.Extensions;

namespace FSpot.Tools.OpenWithExternalDeveloper
{
	public class ExternalDeveloperFactory
	{
		public const string PREFERENCES_EXTENSION =
			Preferences.APP_FSPOT + "extension/openwithexternaldeveloper/";
		public const string DEVELOPER_KEY = PREFERENCES_EXTENSION + "developer";

		public static void SetPreferredDeveloper(string name)
		{
			Preferences.Set (ExternalDeveloperFactory.DEVELOPER_KEY, name);
		}

		public static AbstractExternalDeveloper Get()
		{
			switch (Preferences.Get<string>(DEVELOPER_KEY))
			{
				default:
					return new UFRaw ();
			}
		}
	}

	public class ExternalDeveloperCommand : ICommand
	{
		public void Run (object o, EventArgs e)
		{
			foreach (Photo p in App.Instance.Organizer.SelectedPhotos ())
				try
				{
					ExternalDeveloperFactory.Get().Run(o, e, p);
				}
				catch (OperationCanceledException)
				{
					return;
				}
		}
    	}

	public class ConfigureExternalDeveloperCommand : ICommand
	{
		public ConfigureExternalDeveloperCommand ()
		{
			dialog = new ExternalDeveloperConfigurationDialog ();
		}

		public void Run (object o, EventArgs e)
		{
			dialog.Show ();
		}

		protected ExternalDeveloperConfigurationDialog dialog;
	}
}


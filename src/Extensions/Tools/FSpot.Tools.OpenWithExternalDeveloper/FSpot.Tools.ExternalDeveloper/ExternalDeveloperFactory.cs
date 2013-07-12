//  ExternalDeveloperFactory.cs
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
using System.Collections.Generic;
using System.Linq;
using GLib;

namespace FSpot.Tools.ExternalDeveloper
{
	/// <summary>
	/// Produces <see cref="AbstractExternalDeveloper"/>s depending on the user's preferences.
	/// </summary>
	public class ExternalDeveloperFactory
	{
		public const string PREFERENCES_EXTENSION =
			Preferences.APP_FSPOT + "extension/openwithexternaldeveloper/";
		public const string DEVELOPER_KEY = PREFERENCES_EXTENSION + "developer";

		public static IEnumerable<string> GetAvaliableDevelopers()
		{
			return AppInfoAdapter.GetAllForType("image/x-dcraw").Select(info => info.Name);
		}

		protected static string[] GetSupportedDevelopers()
		{
			string[] supported = { "UFRaw", "Rawstudio" };
			return supported;
		}

		public static bool IsSupported(string name)
		{
			return GetSupportedDevelopers().Contains(name);
		}

		public static bool CanSetJpegQualityForDeveloper(string name)
		{
			return IsSupported(name) && (name != "Rawstudio");
		}

		public static void SetPreferredDeveloper(string name)
		{
			Preferences.Set(ExternalDeveloperFactory.DEVELOPER_KEY, name);
		}

		public static AppInfo GetAppInfo(string name)
		{
			return AppInfoAdapter.GetAllForType("image/x-dcraw").FirstOrDefault(
				info => info.Name == name);
		}

		public static Icon GetIcon(string name)
		{
			return GetAppInfo(name).Icon;
		}

		public static string GetExecutable(string name)
		{
			return GetAppInfo(name).Executable;
		}

		public static AbstractExternalDeveloper Get()
		{
			string name = Preferences.Get<string>(DEVELOPER_KEY);
			switch (name)
			{
				case "UFRaw":
					return new UFRaw();
				case "Rawstudio":
					if (Rawstudio.SupportedVersionIsInstalled())
						return new Rawstudio();
					else
						return new GenericExternalDeveloper(GetExecutable(name));
				default:
					return new GenericExternalDeveloper(GetExecutable(name));
			}
		}
	}
	
}

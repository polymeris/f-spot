using System;
using SemWeb;

namespace FSpot {
	public class InfoDisplay : Gtk.HTML {
		public InfoDisplay () 
		{

		}

		private Exif.ExifData exif_info;

		private IBrowsableItem photo;
		public IBrowsableItem Photo {
			get {
				return photo;
			}
			set {
				photo = value;

				if (exif_info != null)
					exif_info.Dispose ();

				if (photo != null) {
					exif_info = new Exif.ExifData (photo.DefaultVersionUri.LocalPath);
				} else {
					exif_info = null;
				}
				this.Update ();
			}
		}
		
		protected override void OnStyleSet (Gtk.Style previous)
		{
			this.Update ();
		}
		
		private static string Color (Gdk.Color color)
		{
			Byte r = (byte)(color.Red / 256);
			Byte b = (byte)(color.Blue / 256);
			Byte g = (byte)(color.Green / 256);
			string value =  r.ToString ("x2") + g.ToString ("x2") + b.ToString ("x2");
			return value;
		}

		protected override void OnUrlRequested (string url, Gtk.HTMLStream stream)
		{
			if (url == "exif:thumbnail") {
				byte [] data = exif_info.Data;
				
				if (data.Length > 0) {
					stream.Write (data, data.Length);
					stream.Close (Gtk.HTMLStreamStatus.Ok);
				} else 
					stream.Close (Gtk.HTMLStreamStatus.Error);
			} 
		}

		private static string Escape (string value)
		{
			if (value == null) 
				return "(null)";

			value = value.Replace ("&", "&amp;");
			value = value.Replace (">", "&gt;");
			value = value.Replace ("<", "&lt;");
			value = value.Replace ("\n", "<br>");
			return value;
		}

		private void Update ()
		{
			Gtk.HTMLStream stream = this.Begin (null, "text/html; charset=utf-8", Gtk.HTMLBeginFlags.Scroll);
			
			string bg = Color (this.Style.Background (Gtk.StateType.Active));
			string fg = Color (this.Style.Foreground (Gtk.StateType.Active));
			string ig = Color (this.Style.Base (Gtk.StateType.Active));

			stream.Write ("<table width=100% cellpadding=5 cellspacing=0>");
			bool empty = true;
			bool missing = false;

			if (exif_info != null) {
				foreach (Exif.ExifContent content in exif_info.GetContents ()) {
					Exif.ExifEntry [] entries = content.GetEntries ();
					if (entries.Length > 0) {
						empty = false;
						break;
					}
				}

				if (exif_info.Data.Length > 0)
					stream.Write (String.Format ("<tr><td colspan=2 align=\"center\" bgcolor=\"{0}\">" + 
								     "<img center src=\"exif:thumbnail\"></td></tr>", ig));

				int i = 0;
				foreach (Exif.ExifContent content in exif_info.GetContents ()) {
					Exif.ExifEntry [] entries = content.GetEntries ();
					
					i++;
					if (entries.Length < 1)
						continue;
					
					stream.Write ("<tr><th align=left bgcolor=\"" + ig + "\" colspan=2>" 
						      + Exif.ExifUtil.GetIfdNameExtended ((Exif.Ifd)i - 1) + "</th><tr>");
					
					foreach (Exif.ExifEntry entry in entries) {
						stream.Write ("<tr><td valign=top align=right bgcolor=\""+ bg + "\"><font color=\"" + fg + "\">");
						if (entry.Title != null)
							stream.Write (entry.Title);
						else
							stream.Write ("&lt;Unknown Tag ID=" + entry.Tag.ToString () + "&gt;");
						stream.Write ("</font></td><td>");
						string s = entry.Value;
						if (s != null && s != "")
							stream.Write (s);
						stream.Write ("</td><tr>");
					}
				}
			}
			
			if (photo != null) {
				MetadataStore store = new MetadataStore ();
				try {
					ImageFile img = ImageFile.Create (photo.DefaultVersionUri.LocalPath);
					if (img is SemWeb.StatementSource) {
						StatementSource source = (StatementSource)img;
						source.Select (store);
					}
				} catch (System.IO.FileNotFoundException nf) {
					missing = true;
				} 
				
				if (store.StatementCount > 0) {
					empty = false;
					stream.Write ("<tr><th align=left bgcolor=\"" + ig + "\" colspan=2>" 
						      + Mono.Posix.Catalog.GetString ("Extended Metadata") + "</th><tr>");
					
					foreach (Statement stmt in store) {
						// Skip anonymous subjects because they are
						// probably part of a collection
						if (stmt.Subject.Uri == null) 
							continue;
						
						string title;
						string value;

						Description.GetDescription (store, stmt, out title, out value);

						stream.Write ("<tr><td valign=top align=right bgcolor=\""+ bg + "\"><font color=\"" + fg + "\">");
						stream.Write (title);
						stream.Write ("</font></td><td>");
						
					        if (value != null)
							value = Escape (value);
						else {
							MemoryStore substore = store.Select (new Statement ((Entity)stmt.Object, null, null, null));
							WriteCollection (substore, stream);
						}
						
						if (value != null && value != "")
							stream.Write (value);
						
						stream.Write ("</td><tr>");
					}
				}
			}
			
			if (empty) {
				string msg;
				if (photo == null) {
					// FIXME we should pass the full selection to the info display and let it
					// handle multiple items however it wants.
					msg = String.Format ("<tr><td valign=top align=center bgcolor=\"{0}\">" 
							     + "<b>{1}</b></td></tr>", ig,
							     Mono.Posix.Catalog.GetString ("No active image"));
				} else if (missing) {
					string text = String.Format (Mono.Posix.Catalog.GetString ("The image \"{0}\" does not exist"), photo.DefaultVersionUri);
					msg = String.Format ("<tr><td valign=top align=center bgcolor=\"{0}\">" 
							     + "<b>{1}</b></td></tr>", ig, text);
				} else {
					msg = String.Format ("<tr><td valign=top align=center bgcolor=\"{0}\">" 
							     + "<b>{1}</b></td></tr>", ig,
							     Mono.Posix.Catalog.GetString ("No metadata available"));
				}
				stream.Write (msg);
			}

			stream.Write ("</table>");
			End (stream, Gtk.HTMLStreamStatus.Ok);
		}

		private void WriteCollection (MemoryStore substore, Gtk.HTMLStream stream)
		{
			string s = "";
			bool first = true;
			string type;

			foreach (Statement stmt in substore) {
				if (stmt.Predicate == MetadataStore.Namespaces.Resolve ("rdf:type")) {
					string prefix;
					MetadataStore.Namespaces.Normalize (stmt.Object.ToString (), out prefix, out type);
				}
			}
			
			stream.Write ("<table cellpadding=5 cellspacing=0>");
			foreach (Statement sub in substore) {
				string predicate = sub.Predicate.ToString ();
				string title = System.IO.Path.GetFileName (predicate);
				string prefix;
				string ns;

				MetadataStore.Namespaces.Normalize (predicate, out prefix, out title);

			
				if (sub.Object is Literal) {
					if (prefix != "rdf")
						 s += String.Format ("<tr bgcolor={3}><td bgcolor={2}>{0}</td><td>{1}</td></tr>",  
								     Escape (title), 
								     Escape (((Literal)(sub.Object)).Value),
								     Color (Style.MidColors [(int)Gtk.StateType.Normal]),
								     Color (Style.Backgrounds [(int)Gtk.StateType.Normal]));
					else
						s += String.Format ("<tr><td bgcolor={1}>{0}</td></tr>", 
								    Escape (((Literal)(sub.Object)).Value),
								    Color (Style.Backgrounds [(int)Gtk.StateType.Normal]));
				} else {
					try {
						s += System.String.Format ("RDF Type: ({0})<br>", 
									   Escape (new Uri (sub.Object.ToString ()).Fragment));
					} catch {
						//s += System.String.Format ("Type: ({0})<br>", Escape (sub.ToString ()));
						MemoryStore substore2 = substore.Select (new Statement ((Entity)sub.Object, null, null, null));
						if (substore.StatementCount > 0)
							WriteCollection (substore2, stream);
					}
				}
			}
			
			if (s != "")
				stream.Write (s);
			
			stream.Write ("</table>");
		}

		private class StreamSink : SemWeb.StatementSink
		{
			Gtk.HTMLStream stream;
			SemWeb.StatementSource source;
			InfoDisplay info;

			public StreamSink (SemWeb.StatementSource source, Gtk.HTMLStream stream, InfoDisplay info)
			{
				this.stream = stream;
				this.info = info;
				this.source = source;
			}

			public bool Add (SemWeb.Statement stmt)
			{
				string predicate = stmt.Predicate.ToString ();
				string title = System.IO.Path.GetFileName (predicate);
				string bg = InfoDisplay.Color (info.Style.Background (Gtk.StateType.Active));
				string fg = InfoDisplay.Color (info.Style.Foreground (Gtk.StateType.Active));

				//if (MetadataStore.Namespaces.GetPrefix (path) != null) {
				if (stmt.Object is Literal) {
					stream.Write ("<tr><td valign=top align=right bgcolor=\""+ bg + "\"><font color=\"" + fg + "\">");
					stream.Write (title);
					stream.Write ("</font></td><td>");

					string s = ((SemWeb.Literal)(stmt.Object)).Value;
					/*
					else {
						MemoryStore store = source.Select (stmt.Invert (), new SelectPartialFilter (true, false, false, false));
						s = "";
						foreach (Statement sub in store) {
							s += sub.Object.ToString () + "/n";
						}
					}
					*/
					if (s != null && s != "")
						stream.Write (s);
					stream.Write ("</td><tr>");
				}
				return true;
			}
		}
	}
}


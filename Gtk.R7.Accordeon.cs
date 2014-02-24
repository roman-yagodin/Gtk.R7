//
//  Gtk.R7.Accordeon.cs
//
//  Author: Roman M. Yagodin <roman.yagodin@gmail.com>
//  Version: 1.0.0
//  Last-Modified: 2013/01/18
//
//  Copyright (c) 2013 Roman M. Yagodin
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Lesser General Public License for more details.
//
//  You should have received a copy of the GNU Lesser General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections;
using System.Collections.Generic;
using Gtk;

namespace Gtk.R7
{
	/// <summary>
	/// Accordeon manages a set of <see cref="Gtk.Expander" />'s almost like radiogroup -
	/// then user activates (opens) one expander, others get closed automatically.
	/// </summary>
	/// <example>
 	/// using Gtk.R7;
	/// ...
	/// class MainWindow : Gtk.Window 
	/// {
	/// 	private Accordeon accordeon;
	/// 
	///     public MainWindow () : base(Gtk.WindowType.Toplevel) 
	///     {
	///     	  this.Build();
	///         ...
	///         // assuming expander1, expander2, expander3 
	///         // already created, by Stetic or manually
	///         accordeon = new Accordeon (expander1, expander2, expander3);
	///         
	///         // activate first expander and close others
	///         accordeon.CollapseAllBut (expander1);
	///     } 
	/// }
	/// </example>		
	public class Accordeon : IEnumerable<Expander>
	{
		/// <summary>
		/// Expander list - protected for agreater good.
		/// To guarantee that <see cref="Gtk.R7.Accordeon"/> contains unique items, 
		/// using <see cref="System.Collection.Generic.SortedSet<T>" instead of
		/// <see cref="System.Collection.Generic.List<T>" may be better, 
		/// but this is not very important in general use cases
		/// </summary>
		/// <value>
		/// List of the expanders to manage
		/// </value>
		protected List<Expander> Expanders { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Gtk.R7.Accordeon"/> class.
		/// </summary>
		public Accordeon ()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Gtk.R7.Accordeon"/> class
		/// using parameter list of <see cref="Gtk.Expander" />'s
		/// </summary>
		/// <param name='expanders'>
		/// Expanders to add
		/// </param>
		public Accordeon (params Expander[] expanders)
		{
			Expanders = new List<Expander> (expanders);
			foreach (var expander in Expanders)
				expander.Activated += new EventHandler (OnExpanderActivated);
		}

		/// <summary>
		/// Add the specified enumerable collection of <see cref="Gtk.Expander" />'s 
		/// to <see cref="Gtk.R7.Accordeon"/>
		/// </summary>
		/// <param name='expanders'>
		/// Expanders enumerable collection
		/// </param>
		public void Add (IEnumerable<Expander> expanders)
		{
			Expanders.AddRange(expanders);
			foreach (var expander in expanders)
				expander.Activated += new EventHandler (OnExpanderActivated);
		}

		/// <summary>
		/// Add the specified <see cref="Gtk.Expander" /> to <see cref="Gtk.R7.Accordeon"/>
		/// </summary>
		/// <param name='expander'>
		/// Expander reference to add
		/// </param>
		public void Add (Expander expander)
		{
			// add expander, if not already added
			if (Expanders.IndexOf (expander) == -1)
			{
				Expanders.Add (expander);
				expander.Activated += new EventHandler (OnExpanderActivated);
			}
		}

		/// <summary>
		/// Remove the specified expander from <see cref="Gtk.R7.Accordeon"/> 
		/// </summary>
		/// <param name='expander'>
		/// Expander.
		/// </param>
		public void Remove (Expander expander)
		{
			if (Expanders.IndexOf (expander) != -1)
			{
				expander.Activated -= OnExpanderActivated;
				Expanders.Remove (expander);
			}
		}

		/// <summary>
		/// Collapses all expanders
		/// </summary>
		public void CollapseAll ()
		{
			foreach (var expander in Expanders)
				expander.Expanded = false;
		}

		/// <summary>
		/// Expands all expanders - 
		/// not really radiogroup behaviour, but may be usefull
		/// </summary>
		public void ExpandAll ()
		{
			foreach (var expander in Expanders)
				expander.Expanded = true;
		}

		/// <summary>
		/// Collapses all but one expander
		/// </summary>
		/// <param name='index'>
		/// Expander index
		/// </param>
		public void CollapseAllBut (int index)
		{
			var i = 0;
			foreach (var expander in Expanders)
				expander.Expanded = (i++ == index);
		}

		/// <summary>
		/// Collapses all but one expander
		/// </summary>
		/// <param name='exp'>
		/// Expander reference
		/// </param>
		public void CollapseAllBut (Expander exp)
		{
			foreach (var expander in Expanders)
				expander.Expanded = (expander == exp);
		}

		/// <summary>
		/// Gets the <see cref="Gtk.Expander"/> at the specified index.
		/// </summary>
		/// <param name='index'>
		/// Index of 
		/// </param>
		public Expander this [int index]
		{ 
			get { return Expanders [index]; }
		}
		
		/// <summary>
		/// Count of expanders in <see cref="Gtk.R7.Accordeon"/>
		/// </summary>
		public int Count
		{
			get { return Expanders.Count; }
		}

		/// <summary>
		/// Handles the expander Activated event
		/// </summary>
		/// <param name='sender'>
		/// Event source, assume <see cref="Gtk.Expander"/>
		/// </param>
		/// <param name='e'>
		/// Event agruments, not really used
		/// </param>
		private void OnExpanderActivated (object sender, EventArgs e)
		{
			if (sender is Expander)
			{
				if ((sender as Expander).Expanded)
					foreach (var expander in Expanders)
						expander.Expanded = (expander == sender);
			}
		}

		#region IEnumerable implementation

		/// <summary>
		/// Gets the enumerator.
		/// </summary>
		/// <returns>
		/// The enumerator.
		/// </returns>
		IEnumerator IEnumerable.GetEnumerator ()
		{
			return Expanders.GetEnumerator(); 
		}

		/// <summary>
		/// Gets the enumerator.
		/// </summary>
		/// <returns>
		/// The enumerator.
		/// </returns>
		public IEnumerator<Expander> GetEnumerator ()
		{
			return Expanders.GetEnumerator(); 
		}

		#endregion
	}
}

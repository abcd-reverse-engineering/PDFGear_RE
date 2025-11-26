using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;
using pdfeditor.Controls;

namespace pdfeditor.Utils
{
	// Token: 0x020000AC RID: 172
	public static class TreeViewExtensions
	{
		// Token: 0x06000ABC RID: 2748 RVA: 0x000380C1 File Offset: 0x000362C1
		public static TreeViewItem TreeViewItemFromElement(this TreeView treeView, object item, Func<object, object> parentSelector)
		{
			return TreeViewExtensions.GetContainerFast(treeView, item, parentSelector);
		}

		// Token: 0x06000ABD RID: 2749 RVA: 0x000380CB File Offset: 0x000362CB
		public static TreeViewItem TreeViewItemFromElement(this TreeView treeView, ITreeViewNode item)
		{
			return TreeViewExtensions.GetContainerFast(treeView, item, delegate(object o)
			{
				ITreeViewNode treeViewNode = o as ITreeViewNode;
				if (treeViewNode == null)
				{
					return null;
				}
				return treeViewNode.Parent;
			});
		}

		// Token: 0x06000ABE RID: 2750 RVA: 0x000380F4 File Offset: 0x000362F4
		public static async Task ScrollIntoViewAsync(this TreeView treeView, ITreeViewNode item, ScrollIntoViewOrientation orientation = ScrollIntoViewOrientation.Both)
		{
			await treeView.ScrollIntoViewAsync(item, delegate(object o)
			{
				ITreeViewNode treeViewNode = o as ITreeViewNode;
				if (treeViewNode == null)
				{
					return null;
				}
				return treeViewNode.Parent;
			}, orientation).ConfigureAwait(false);
		}

		// Token: 0x06000ABF RID: 2751 RVA: 0x00038148 File Offset: 0x00036348
		public static async Task ScrollIntoViewAsync(this TreeView treeView, object item, Func<object, object> parentSelector, ScrollIntoViewOrientation orientation = ScrollIntoViewOrientation.Both)
		{
			if (treeView != null)
			{
				if (item != null)
				{
					if (treeView != null)
					{
						ItemContainerGenerator generator = treeView.ItemContainerGenerator;
						if (generator != null)
						{
							if (parentSelector == null)
							{
								if (!(item is ITreeViewNode))
								{
									return;
								}
								parentSelector = delegate(object i)
								{
									ITreeViewNode treeViewNode = i as ITreeViewNode;
									if (treeViewNode == null)
									{
										return null;
									}
									return treeViewNode.Parent;
								};
							}
							TreeViewItem containerFast = TreeViewExtensions.GetContainerFast(treeView, item, parentSelector);
							if (containerFast != null)
							{
								containerFast.BringIntoView();
							}
							else
							{
								Stack<object> stack = new Stack<object>();
								object obj = item;
								for (object obj2 = parentSelector(obj); obj2 != null; obj2 = parentSelector(obj))
								{
									stack.Push(obj);
									obj = obj2;
								}
								stack.Push(obj);
								await TreeViewExtensions.UpdateLayoutAsync(treeView.Dispatcher, generator, treeView);
								TreeViewItem lastContainer = null;
								while (stack.Count > 0)
								{
									object cur = stack.Pop();
									TreeViewItem treeViewItem = await TreeViewExtensions.GetContainerAsync(treeView.Dispatcher, generator, cur);
									if (treeViewItem == null)
									{
										ReadOnlyCollection<object> items = generator.Items;
										int num = ((items != null) ? items.IndexOf(cur) : (-1));
										if (num >= 0)
										{
											VirtualizingPanel panel = UIElementExtension.FindVisualChild<VirtualizingPanel>(lastContainer ?? treeView);
											if (panel != null)
											{
												await TreeViewExtensions.BringIndexIntoViewPublicAsync(treeView.Dispatcher, generator, panel, num, orientation);
												await TreeViewExtensions.UpdateLayoutAsync(treeView.Dispatcher, generator, panel);
												treeViewItem = await TreeViewExtensions.GetContainerAsync(treeView.Dispatcher, generator, cur);
											}
											panel = null;
										}
									}
									if (treeViewItem == null)
									{
										break;
									}
									treeViewItem.IsExpanded = true;
									lastContainer = treeViewItem;
									if (stack.Count == 0)
									{
										treeViewItem.BringIntoView();
									}
									generator = treeViewItem.ItemContainerGenerator;
									await TreeViewExtensions.UpdateLayoutAsync(treeView.Dispatcher, generator, treeViewItem);
									cur = null;
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06000AC0 RID: 2752 RVA: 0x000381A4 File Offset: 0x000363A4
		private static ScrollViewer GetNearestScrollViewer(UIElement element)
		{
			ItemsControl itemsControl = element as ItemsControl;
			if (itemsControl != null)
			{
				ScrollViewer scrollViewer = TreeViewExtensions.<GetNearestScrollViewer>g__FindChild|4_0(itemsControl);
				if (scrollViewer != null)
				{
					return scrollViewer;
				}
			}
			UIElement uielement = element;
			ScrollViewer scrollViewer2;
			do
			{
				uielement = VisualTreeHelper.GetParent(uielement) as UIElement;
				scrollViewer2 = uielement as ScrollViewer;
			}
			while (uielement != null && scrollViewer2 == null);
			return scrollViewer2;
		}

		// Token: 0x06000AC1 RID: 2753 RVA: 0x000381E8 File Offset: 0x000363E8
		private static async Task UpdateLayoutAsync(Dispatcher dispatcher, ItemContainerGenerator generator, UIElement element)
		{
			if (dispatcher == null)
			{
				throw new ArgumentNullException("dispatcher");
			}
			if (generator == null)
			{
				throw new ArgumentNullException("generator");
			}
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			if (generator.Status == GeneratorStatus.ContainersGenerated)
			{
				try
				{
					element.UpdateLayout();
					return;
				}
				catch
				{
					return;
				}
			}
			try
			{
				await dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(delegate
				{
					try
					{
						element.UpdateLayout();
					}
					catch
					{
					}
				}));
			}
			catch
			{
			}
		}

		// Token: 0x06000AC2 RID: 2754 RVA: 0x0003823C File Offset: 0x0003643C
		private static async Task BringIndexIntoViewPublicAsync(Dispatcher dispatcher, ItemContainerGenerator generator, VirtualizingPanel panel, int index, ScrollIntoViewOrientation orientation)
		{
			if (dispatcher == null)
			{
				throw new ArgumentNullException("dispatcher");
			}
			if (generator == null)
			{
				throw new ArgumentNullException("generator");
			}
			if (panel == null)
			{
				throw new ArgumentNullException("panel");
			}
			if (generator.Status == GeneratorStatus.ContainersGenerated)
			{
				try
				{
					panel.BringIndexIntoViewPublic(index);
					return;
				}
				catch
				{
					return;
				}
			}
			try
			{
				await dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(delegate
				{
					try
					{
						panel.BringIndexIntoViewPublic(index);
					}
					catch
					{
					}
				}));
			}
			catch
			{
			}
		}

		// Token: 0x06000AC3 RID: 2755 RVA: 0x00038298 File Offset: 0x00036498
		private static async Task<TreeViewItem> GetContainerAsync(Dispatcher dispatcher, ItemContainerGenerator generator, object item)
		{
			if (dispatcher == null)
			{
				throw new ArgumentNullException("dispatcher");
			}
			if (generator == null)
			{
				throw new ArgumentNullException("generator");
			}
			TreeViewItem treeViewItem;
			if (item == null)
			{
				treeViewItem = null;
			}
			else if (generator.Status == GeneratorStatus.ContainersGenerated)
			{
				treeViewItem = generator.ContainerFromItem(item) as TreeViewItem;
			}
			else
			{
				TreeViewItem container = null;
				try
				{
					await dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(delegate
					{
						container = generator.ContainerFromItem(item) as TreeViewItem;
					}));
				}
				catch
				{
				}
				treeViewItem = container;
			}
			return treeViewItem;
		}

		// Token: 0x06000AC4 RID: 2756 RVA: 0x000382EC File Offset: 0x000364EC
		private static TreeViewItem GetContainerFast(TreeView treeView, object item, Func<object, object> parentSelector)
		{
			if (treeView == null || item == null || parentSelector == null)
			{
				return null;
			}
			Stack<object> stack = new Stack<object>();
			while (item != null)
			{
				stack.Push(item);
				item = parentSelector(item);
			}
			ItemsControl itemsControl = treeView;
			while (stack.Count > 0 && itemsControl != null)
			{
				itemsControl = itemsControl.ItemContainerGenerator.ContainerFromItem(stack.Pop()) as ItemsControl;
			}
			return itemsControl as TreeViewItem;
		}

		// Token: 0x06000AC5 RID: 2757 RVA: 0x0003834C File Offset: 0x0003654C
		[CompilerGenerated]
		internal static ScrollViewer <GetNearestScrollViewer>g__FindChild|4_0(UIElement _element)
		{
			if (_element == null)
			{
				return null;
			}
			for (int i = 0; i < VisualTreeHelper.GetChildrenCount(_element); i++)
			{
				DependencyObject child = VisualTreeHelper.GetChild(_element, i);
				ScrollViewer scrollViewer = child as ScrollViewer;
				if (scrollViewer != null)
				{
					return scrollViewer;
				}
				UIElement uielement = child as UIElement;
				if (uielement != null)
				{
					ScrollViewer scrollViewer2 = TreeViewExtensions.<GetNearestScrollViewer>g__FindChild|4_0(uielement);
					if (scrollViewer2 != null)
					{
						return scrollViewer2;
					}
				}
			}
			return null;
		}
	}
}

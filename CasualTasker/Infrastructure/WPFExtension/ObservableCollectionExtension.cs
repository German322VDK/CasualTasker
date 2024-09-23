using CasualTasker.DTO.Base;
using System.Collections.ObjectModel;
using System.Windows.Data;

namespace CasualTasker.Infrastructure.WPFExtension
{
    /// <summary>
    /// Provides extension methods for working with <see cref="ObservableCollection{T}"/>.
    /// These methods include batch addition of items, searching for items, and updating the view.
    /// </summary>
    public static class ObservableCollectionExtension
    {
        /// <summary>
        /// Adds a range of items to the specified <see cref="ObservableCollection{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the items in the collection.</typeparam>
        /// <param name="collect">The collection to which items will be added.</param>
        /// <param name="data">The items to add to the collection.</param>
        public static void AddRange<T>(this ObservableCollection<T> collect, IEnumerable<T> data)
        {
            foreach (T item in data)
            {
                collect.Add(item);
            }
        }

        /// <summary>
        /// Clears the collection and adds a range of items to the specified <see cref="ObservableCollection{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the items in the collection.</typeparam>
        /// <param name="collect">The collection to clear and add items to.</param>
        /// <param name="data">The items to add to the collection.</param>
        public static void AddRangeWithClear<T>(this ObservableCollection<T> collect, IEnumerable<T> data)
        {
            collect.Clear();
            collect.AddRange(data);
        }

        /// <summary>
        /// Finds an item in the collection by its ID.
        /// </summary>
        /// <typeparam name="T">The type of the items in the collection, which must derive from <see cref="NamedEntity"/>.</typeparam>
        /// <param name="collect">The collection to search.</param>
        /// <param name="id">The ID of the item to find.</param>
        /// <returns>The index of the found item, or -1 if not found.</returns>
        public static int FindItem<T>(this ObservableCollection<T> collect, int id) where T : NamedEntity
        {
            var findItem = collect.FirstOrDefault(el => el.Id == id);
            if (findItem == null)
                return -1;
            return collect.IndexOf(findItem);
        }

        /// <summary>
        /// Finds an item in the collection by the item itself.
        /// </summary>
        /// <typeparam name="T">The type of the items in the collection, which must derive from <see cref="NamedEntity"/>.</typeparam>
        /// <param name="collect">The collection to search.</param>
        /// <param name="item">The item to find.</param>
        /// <returns>The index of the found item, or -1 if not found.</returns>
        public static int FindItem<T>(this ObservableCollection<T> collect, T item) where T : NamedEntity
        {
            if (item == null)
                return -1;
            return collect.FindItem(item.Id);
        }

        /// <summary>
        /// Updates the view associated with the specified <see cref="ObservableCollection{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the items in the collection, which must derive from <see cref="NamedEntity"/>.</typeparam>
        /// <param name="collect">The collection whose view will be updated.</param>
        /// <returns><c>true</c> if the view was successfully refreshed; otherwise, <c>false</c>.</returns>
        public static bool UpdateView<T>(this ObservableCollection<T> collect) where T : NamedEntity
        {
            var view = CollectionViewSource.GetDefaultView(collect);
            if (view == null)
                return false;
            view.Refresh();
            return true;
        }
    }
}

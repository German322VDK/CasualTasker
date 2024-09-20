using CasualTasker.DTO.Base;
using System.Collections.ObjectModel;
using System.Windows.Data;

namespace CasualTasker.Infrastructure.WPFExtension
{
    public static class ObservableCollectionExtension
    {
        public static void AddRange<T>(this ObservableCollection<T> collect, IEnumerable<T> data)
        {
            foreach (T item in data)
            {
                collect.Add(item);
            }
        }

        public static void AddRangeWithClear<T>(this ObservableCollection<T> collect, IEnumerable<T> data)
        {
            collect.Clear();
            collect.AddRange(data);
        }

        public static int FindItem<T>(this ObservableCollection<T> collect, int id) where T : NamedEntity
        {
            var findItem = collect.FirstOrDefault(el => el.Id == id);
            if (findItem == null)
                return -1;
            return collect.IndexOf(findItem);
        }

        public static int FindItem<T>(this ObservableCollection<T> collect, T item) where T : NamedEntity
        {
            if (item == null)
                return -1;
            return collect.FindItem(item.Id);
        }

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

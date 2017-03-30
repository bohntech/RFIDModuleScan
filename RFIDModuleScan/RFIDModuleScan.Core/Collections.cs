//Licensed under MIT License see LICENSE.TXT in project root folder
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace RFIDModuleScan.Core
{
    public class RangeObservableCollection<T> : ObservableCollection<T>
    {
        private List<T> addedItems = new List<T>();
        private List<T> removedItems = new List<T>();
        
        public void AddWithoutNotify(T item)
        {
            this.CheckReentrancy();
            this.Items.Add(item);
            addedItems.Add(item);
        }

        public void RemoveWithoutNotify(T item)
        {
            this.CheckReentrancy();
            this.Items.Remove(item);
            removedItems.Add(item);
        }

        public void ApplyUpdates()
        {
            if (addedItems.Count() > 0)
            {
                this.CheckReentrancy();
                List<T> temp = new List<T>();               
                temp.AddRange(addedItems);
                addedItems.Clear();
                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, temp));                
            }

            if (removedItems.Count() > 0)
            {
                this.CheckReentrancy();
                List<T> tempDeletes = new List<T>();
                tempDeletes.AddRange(removedItems);
                removedItems.Clear();
                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, tempDeletes));
            }
        }
    }
}

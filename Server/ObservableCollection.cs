using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Server
{
    internal class ObservableCollection : ObservableCollection<Process>
    {
        public ObservableCollection(IEnumerable<Process> collection) : base(collection)
        {
        }
    }
}
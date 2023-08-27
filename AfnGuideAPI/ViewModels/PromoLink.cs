using System.Collections;

namespace AfnGuideAPI.ViewModels
{
    public class PromoLink
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public string? LinkUrl { get; set; }
        public bool IsPromoB { get; set; }
        public PromoLinkSchedules Schedules { get; set; } = new();
    }

    public class PromoLinkSchedules : IList<PromoLinkSchedule>, ICollection<PromoLinkSchedule>
    {
        private readonly List<PromoLinkSchedule> innerList;

        public PromoLinkSchedules()
        {
            innerList = new();
        }

        public PromoLinkSchedules(IEnumerable<PromoLinkSchedule> collection)
        {
            innerList = new();
            foreach (var item in collection)
            {
                Add(item);
            }
        }

        #region IList Implementation

        public PromoLinkSchedule this[int index] { get => ((IList<PromoLinkSchedule>)innerList)[index]; set => ((IList<PromoLinkSchedule>)innerList)[index] = value; }

        public int IndexOf(PromoLinkSchedule item)
        {
            return ((IList<PromoLinkSchedule>)innerList).IndexOf(item);
        }

        public void Insert(int index, PromoLinkSchedule item)
        {
            ((IList<PromoLinkSchedule>)innerList).Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            ((IList<PromoLinkSchedule>)innerList).RemoveAt(index);
        }

        #endregion

        #region ICollection Implementation

        public int Count => ((ICollection<PromoLinkSchedule>)innerList).Count;

        public bool IsReadOnly => ((ICollection<PromoLinkSchedule>)innerList).IsReadOnly;

        public void Add(PromoLinkSchedule item)
        {
            if (innerList.Any(s 
                => s.TimeZoneId == item.TimeZoneId 
                && s.Date == item.Date 
                && s.Channel == item.Channel))
            {
                return;
            }

            ((ICollection<PromoLinkSchedule>)innerList).Add(item);
        }

        public void Clear()
        {
            ((ICollection<PromoLinkSchedule>)innerList).Clear();
        }

        public bool Contains(PromoLinkSchedule item)
        {
            return ((ICollection<PromoLinkSchedule>)innerList).Contains(item);
        }

        public void CopyTo(PromoLinkSchedule[] array, int arrayIndex)
        {
            ((ICollection<PromoLinkSchedule>)innerList).CopyTo(array, arrayIndex);
        }

        public bool Remove(PromoLinkSchedule item)
        {
            return ((ICollection<PromoLinkSchedule>)innerList).Remove(item);
        }

        #endregion

        #region IEnumerable Implementation

        public IEnumerator<PromoLinkSchedule> GetEnumerator()
        {
            return ((IEnumerable<PromoLinkSchedule>)innerList).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)innerList).GetEnumerator();
        }

        #endregion
    }

    public class PromoLinkSchedule
    {
        public string? Channel { get; set; }
        public string? Date { get; set; }
        public string? Time { get; set; }
        public int? TimeZoneId { get; set; }
    }
}
